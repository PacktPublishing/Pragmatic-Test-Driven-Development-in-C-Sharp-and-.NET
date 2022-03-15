using Uqs.Blog.Domain.Repositories;

namespace Uqs.Blog.Domain.Services;

public class UpdateTitleService
{
    private readonly IPostRepository _postRepository;
    private const int TITLE_MAX_LENGTH = 90;
    public UpdateTitleService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public void UpdateTitle(int postId, string title)
    {
        if (title is null)
        {
            title = string.Empty;
        }
        title = title.Trim();
        if (title.Length > TITLE_MAX_LENGTH)
        {
            throw new ArgumentOutOfRangeException(nameof(title), 
                $"The title can be a max of {TITLE_MAX_LENGTH} letters");
        }
        var post = _postRepository.GetById(postId);
        if (post is null)
        {
            throw new ArgumentException($"Unable to find a post of Id {postId}", 
                nameof(post));
        }
        post.Title = title;
        _postRepository.Update(post);
    }
}
