#!/bin/bash
export PATH="$PATH:/home/mindcrash/.dotnet/tools"
# Stop execution if any command fails
set -e

# --- Configuration ---
SOLUTION_FILE="Gadema.sln"
DOCFX_CONFIG="docfx.json"
SITE_OUTPUT_DIR="docs/api/_site"

echo "================================================"
echo "🚀 Starting GaDeMa API Documentation Build"
echo "================================================"

# 1. Clean up old site output to prevent stale artifacts
if [ -d "$SITE_OUTPUT_DIR" ]; then
    echo "🧹 Cleaning previous build directory: $SITE_OUTPUT_DIR"
    rm -rf "$SITE_OUTPUT_DIR"
fi

# 2. Build the solution
# This is CRITICAL. DocFX reads the XML files generated during compilation.
# If you don't build, your API reference will be empty or outdated.
echo "🔨 Building solution to generate XML documentation..."
dotnet build "$SOLUTION_FILE"

# 3. Run DocFX
# We check if the config exists before trying to run it
if [ -f "$DOCFX_CONFIG" ]; then
    echo "📚 Running DocFX metadata generation and building site..."
    # Using --serve allows you to view the results immediately at localhost:8080
    docfx "$DOCFX_CONFIG" --serve
else
    echo "❌ ERROR: DocFX configuration not found at $DOCFX_CONFIG"
    echo "Please ensure you have created the docfx.json file."
    exit 1
fi

echo "================================================"
echo "✅ API Documentation build complete!"
echo "🌐 View your API docs at: http://localhost:8080"
echo "================================================"