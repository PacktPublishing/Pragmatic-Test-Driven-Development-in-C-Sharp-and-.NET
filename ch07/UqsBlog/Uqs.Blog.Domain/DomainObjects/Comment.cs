namespace Uqs.Blog.Domain.DomainObjects;

public class Comment
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
}