using Microsoft.EntityFrameworkCore;
using Anima.Core.Models.Tagging;

namespace Anima.Data.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Populates the MetaTag entity with enum values for a given type.
    /// </summary>
    public static void PopulateTags<T>(this ModelBuilder modelBuilder) where T : struct, Enum
    {
         var tags = Enum.GetValues<T>().Select(value => new MetaTag
        {
            Id = value.ToGuid(),
            Name = value.ToString()
        }).ToList();

        modelBuilder.Entity<MetaTag>().HasData(tags);
    }

   /*old implementation
      private void PopulateTags<T>(ModelBuilder modelBuilder) where T : struct,Enum
    {
        List<MetaTag> tags = new List<MetaTag>();
        foreach(var value in Enum.GetValues<T>())
        {
            var tagId = value.ToGuid();
            string tagName = value.ToString();
            MetaTag tag = new MetaTag()
            {
              Id =tagId,
              Name = tagName  
            };
            tags.Add(tag);
        }

        modelBuilder.Entity<MetaTag>().HasData(tags);
    }*/
}
