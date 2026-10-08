using Anima.Core.Models.Blog;
using Anima.Core.Models.ContentBase;
using Anima.Core.Models.Tagging;
using Microsoft.EntityFrameworkCore;

namespace Anima.Data.Contexts;

public partial class AppDbContext
{
    public DbSet<AssetLink> AssetLinks { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<ContentMetaInfo> ContentMetaInfos { get; set; }
    public DbSet<ContentTagRelation> ContentTagRelations { get; set; }
    public DbSet<ExternalReference> ExternalReferences { get; set; }
    public DbSet<MediaAttachment> MediaAttachments { get; set; }

    public DbSet<BlogPost> BlogPosts { get; set; }
}
