using Microsoft.AspNetCore.Mvc;
using Uqs.Blog.Domain.Services;
using Uqs.Blog.Contract;

namespace Uqs.Blog.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IAddPostService _addPostService;

    public PostController(ILogger<PostController> logger, IAddPostService addPostService)
    {
        _logger = logger;
        _addPostService = addPostService;
    }

    [HttpPost]
    public IActionResult Add([FromBody] PostPage post)
    {
        throw new NotImplementedException();
    }

}
