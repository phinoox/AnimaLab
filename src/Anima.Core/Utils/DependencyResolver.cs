using System.Reflection;



namespace Anima.Core.Utils;

/// <summary>
/// Builds a dependency graph from [ModelDependency] attributes and produces
/// a topologically sorted list of types (FK-order: parents before children).
/// </summary>
// Gadema.Core/Seeders/DependencyResolver.cs (updated)
public static class DependencyResolver
{
    /// <summary>
    /// Scans only Gadema.Core.Models assembly for types with [ModelDependency] attributes.
    /// Returns a tuple containing the topologically sorted types and any cyclic types.
    /// </summary>
    public static (List<Type> SortedTypes, List<Type> CyclicTypes) ResolveDependencies()
    {
        // Explicitly target Gadema.Core.Models — not the current assembly (Gadema.Tests.dll)
        var modelsAssembly = typeof(DependencyResolver).Assembly;

        return ResolveDependencies(modelsAssembly);
    }

    /// <summary>
    /// Generic overload for scanning any assembly.
    /// Useful if you want to scan a different project later (e.g., Gadema.Api.Models).
    /// Returns a tuple containing the topologically sorted types and any cyclic types.
    /// </summary>
    public static (List<Type> SortedTypes, List<Type> CyclicTypes) ResolveDependencies(Assembly assembly, string? targetNamespace = null)
    {

        var allExportedTypes = assembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract).ToList();

        // 1. Define the entry points
        IEnumerable<Type> initialSet = targetNamespace != null
            ? allExportedTypes.Where(t => t.Namespace != null && t.Namespace.StartsWith(targetNamespace))
            : allExportedTypes;

        var typeMap = new Dictionary<Type, List<ModelDependencyAttribute>>();
        var traversalQueue = new Queue<Type>(initialSet);
        var visited = new HashSet<Type>();

        // 2. Build the graph via traversal (Single Pass)
        while (traversalQueue.Count > 0)
        {
            var current = traversalQueue.Dequeue();
            if (!visited.Add(current)) continue;

            var attrs = current.GetCustomAttributes<ModelDependencyAttribute>(false).ToList();
            if (attrs.Count == 0) continue;
            // We add the type to the map if it has dependencies OR if it's part of the initial set 
            // (to ensure we track even 'leaf' nodes in our graph)
            typeMap[current] = attrs;

            foreach (var attr in attrs)
            {
                foreach (var depType in attr.DependentTypes)
                {
                    if (depType == typeof(RootMarker) || depType == current) continue;

                    if (!visited.Contains(depType))
                    {
                        traversalQueue.Enqueue(depType);
                    }
                }
            }
        }

        var allTypes = typeMap.Keys;

        // ─────────────────────────────────────────────────────────────────────
        // STEP 3: Build directed graph (A depends on B → edge A→B, meaning A seeds AFTER B)
        // Only include edges where both endpoints are in the graph.
        // ─────────────────────────────────────────────────────────────────────
        var adjacency = new Dictionary<Type, List<Type>>();
        foreach (var type in allTypes) adjacency[type] = new List<Type>();

        foreach (var kvp in typeMap)
        {
            foreach (var depType in kvp.Value.SelectMany(a => a.DependentTypes).Distinct())
            {
                if (depType == kvp.Key) continue; // skip self-references
                if (allTypes.Contains(depType))
                    adjacency[kvp.Key].Add(depType);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // STEP 4: Topological sort via Kahn's algorithm
        // ─────────────────────────────────────────────────────────────────────
        // Initialize in-degree for each node (count of direct dependencies within the graph)
        var inDegree = new Dictionary<Type, int>();
        foreach (var type in allTypes)
            inDegree[type] = adjacency[type].Count;

        // Enqueue all nodes with zero in-degree (no dependencies within the graph — these are "roots")
        var queue = new Queue<Type>();
        foreach (var kvp in inDegree)
        {
            if (kvp.Value == 0)
                queue.Enqueue(kvp.Key);
        }

        // Process nodes in order: remove edges, enqueue nodes whose in-degree becomes zero
        var sorted = new List<Type>();
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            sorted.Add(current);

            foreach (var kvp in typeMap)
            {
                if (adjacency.TryGetValue(kvp.Key, out var neighbors) && neighbors.Contains(current))
                {
                    inDegree[kvp.Key]--;
                    if (inDegree[kvp.Key] == 0 && !sorted.Contains(kvp.Key))
                        queue.Enqueue(kvp.Key);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // STEP 5: Cycle detection — unvisited nodes means a cycle exists
        // ─────────────────────────────────────────────────────────────────────
        var cyclicTypes = new List<Type>();
        foreach (var t in allTypes)
        {
            if (!sorted.Contains(t))
            {
                cyclicTypes.Add(t);
                // Console.WriteLine($"[DependencyResolver] ⚠ Cycle detected involving: '{t.Name}'");
            }
        }


        return (sorted, cyclicTypes);
    }


    /// <summary>
    /// Returns a map of Type → list of [ModelDependencyAttribute] for inspection/debugging.
    /// Useful for test assertions on graph correctness.
    /// </summary>
    public static Dictionary<Type, List<ModelDependencyAttribute>> GetGraph(Assembly assembly)
    {
        var typeMap = new Dictionary<Type, List<ModelDependencyAttribute>>();

        foreach (var type in assembly.GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract))
        {
            var attrs = type.GetCustomAttributes(typeof(ModelDependencyAttribute), false);
            if (attrs.Length > 0)
                typeMap[type] = attrs.Cast<ModelDependencyAttribute>().ToList();
        }

        return typeMap;
    }


    /// <summary>
    /// Prints a visual tree representation of the dependency hierarchy.
    /// </summary>
    public static void PrintHierarchy(Assembly assembly, string? domainNamespace = null)
    {
        var (sorted, cyclic) = ResolveDependencies(assembly, domainNamespace);
        var typeMap = GetGraph(assembly);

        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("      GADEMA DEPENDENCY HIERARCHY");
        Console.WriteLine(new string('=', 50));

        // Add explicit Root anchor
        Console.ForegroundColor = ConsoleColor.DarkGray;

        Console.ResetColor();

        if (cyclic.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n⚠ DETECTED CYCLES:");
            Console.ResetColor();
            PrintDetailedCycles(assembly);
        }


        // Calculate depths for indentation
        var depths = new Dictionary<Type, int>();
        foreach (var type in sorted)
        {
            depths[type] = CalculateDepth(type, typeMap, depths);
        }

        Console.WriteLine("\nSTRUCTURE:");
        Console.WriteLine("ROOT");
        foreach (var type in sorted)
        {
            int depth = depths[type];
            string indent = new string(' ', depth * 4);
            string prefix = "└── "; // Indent children relative to root

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{indent}{prefix}");
            Console.ResetColor();
            Console.WriteLine(type.FullName);
        }

        Console.WriteLine(new string('=', 50) + "\n");
    }

    /// <summary>
    /// Prints the actual paths of detected cycles using a visual arrow.
    /// </summary>
    private static void PrintDetailedCycles(Assembly assembly)
    {
        var (sorted, cyclic) = ResolveDependencies(assembly);
        var typeMap = GetGraph(assembly);

        foreach (var startNode in cyclic)
        {
            var path = new List<Type> { startNode };
            if (TryFindCycle(startNode, typeMap, path, new HashSet<Type>()))
            {
                // The TryFindCycle method now handles the printing of the path itself
            }
        }
    }

    private static bool TryFindCycle(Type current, Dictionary<Type, List<ModelDependencyAttribute>> typeMap, List<Type> path, HashSet<Type> visited)
    {
        if (path.Contains(current))
        {
            // Found the loop! Extract the cycle segment from the path.
            int startIndex = path.IndexOf(current);
            var cyclePath = path.Skip(startIndex).ToList();
            cyclePath.Add(current); // Close the loop: A -> B -> A

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [Cycle] {string.Join(" ➔ ", cyclePath.Select(t => t.Name))}");
            Console.ResetColor();
            return true;
        }

        if (visited.Contains(current)) return false;
        visited.Add(current);

        if (typeMap.TryGetValue(current, out var attrs))
        {
            foreach (var attr in attrs)
            {
                foreach (var depType in attr.DependentTypes)
                {
                    path.Add(depType);
                    if (TryFindCycle(depType, typeMap, path, visited)) return true;
                    path.RemoveAt(path.Count - 1);
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Finds only the types required to satisfy the dependencies of the target type.
    /// </summary>
    public static List<Type> GetRequiredAncestors(Assembly assembly, Type targetType)
    {
        var requiredTypes = new HashSet<Type>();
        var queue = new Queue<Type>();
        queue.Enqueue(targetType);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!requiredTypes.Add(current)) continue;

            // Get the dependencies of the current type from the graph
            var graph = GetGraph(assembly);
            if (graph.TryGetValue(current, out var attrs))
                foreach (var attr in attrs)
                    foreach (var depType in attr.DependentTypes)
                        queue.Enqueue(depType);
        }

        // Now we have all ancestors. We must sort them so parents are seeded before children.
        // We use the existing topological sort logic on this subset.
        return ResolveDependencies(assembly).SortedTypes
            .Where(t => requiredTypes.Contains(t))
            .ToList();
    }

    private static int CalculateDepth(Type type, Dictionary<Type, List<ModelDependencyAttribute>> typeMap, Dictionary<Type, int> computedDepths)
    {
        if (computedDepths.TryGetValue(type, out var depth)) return depth;

        int maxParentDepth = -1;
        if (typeMap.TryGetValue(type, out var attrs))
        {
            foreach (var attr in attrs)
            {
                foreach (var depType in attr.DependentTypes)
                {
                    // We check if the dependency is part of our graph to determine depth
                    if (typeMap.ContainsKey(depType) || depType == typeof(RootMarker))
                    {
                        int d = CalculateDepth(depType, typeMap, computedDepths);
                        if (d > maxParentDepth) maxParentDepth = d;
                    }
                }
            }
        }

        int currentDepth = maxParentDepth + 1;
        computedDepths[type] = currentDepth;
        return currentDepth;
    }


    private static bool IsNullableReferenceType(this Type t) =>
        typeof(object).IsAssignableFrom(t) && !t.IsValueType;
}