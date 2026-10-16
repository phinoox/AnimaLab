namespace Anima.Core.Dtos.Shared;

public class SearchRequestDto
{
    public string Query { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
    public string? Type { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
