namespace Anima.Core.Models.Sandbox;

public record ProjectDto(Guid Id, string Name, string Slug);
public record BlogPostDto(Guid Id, string Title, string Content, Guid ProjectId);
