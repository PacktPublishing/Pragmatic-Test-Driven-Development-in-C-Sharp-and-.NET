using Uqs.Blog.Domain.DomainObjects;

namespace Uqs.Blog.Domain.Repositories;

public interface IAuthorRepository
{
    Author? GetById(int id);
}

public class AuthorRepository : IAuthorRepository
{
    public Author? GetById(int id)
    {
        return null;
    }
}