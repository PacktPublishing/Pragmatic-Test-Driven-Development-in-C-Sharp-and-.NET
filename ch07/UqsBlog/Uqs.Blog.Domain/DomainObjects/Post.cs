namespace Uqs.Blog.Domain.DomainObjects;

public class Post
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Excerpt { get; set; }
    public bool IsPublished { get; set; }
    public Tag[]? Tags { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public int AuthorId { get; set; }
    public Author? Author { get;set; }
}