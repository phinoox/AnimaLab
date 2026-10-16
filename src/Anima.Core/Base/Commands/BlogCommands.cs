namespace Anima.Core.Base.Commands;

using Anima.Core.Base.Commands;
using Anima.Core.Models.Blog;
using Anima.Core.Models.ContentBase;

public record CreateBlogCommand(ContentMetaInfo Meta) : IApiCommand;

public record GetBlogCommand() : IApiCommand;
public record UpdateBlogBodyCommand(BlogPost Data) : IApiCommand;
public record UpdateBlogMetaCommand(ContentMetaInfo Meta) : IApiCommand;
public record DeleteBlogCommand() : IApiCommand;
