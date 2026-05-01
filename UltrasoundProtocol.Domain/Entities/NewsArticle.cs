namespace UltrasoundProtocol.Domain.Entities;

public class NewsArticle : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string Author { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
}
