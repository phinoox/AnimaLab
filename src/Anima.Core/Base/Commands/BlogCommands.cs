namespace Anima.Core.Base.Commands;

using Anima.Core.Base.Commands;
using Anima.Core.Models.Blog;
using Anima.Core.Models.ContentBase;

public record CreateBlogCommand(ContentMetaInfo Meta) : ICommand;

public record GetBlogCommand() : ICommand;
public record UpdateBlogBodyCommand(BlogPost Data) : ICommand;
public record UpdateBlogMetaCommand(ContentMetaInfo Meta) : ICommand;
public record DeleteBlogCommand() : ICommand;
