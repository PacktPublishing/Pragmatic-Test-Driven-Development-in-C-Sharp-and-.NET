namespace Uqs.Blog.Contract;

public record PostPage(int Id, string Content, Author Author, DateTime CreatedDate, int NumberOfComments, int NumberOfViews);

