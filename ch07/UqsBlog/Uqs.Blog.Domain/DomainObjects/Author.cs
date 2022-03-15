namespace Uqs.Blog.Domain.DomainObjects;

public class Author
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsLocked { get; set; }
    public Post[]? Posts { get;set; }
}
