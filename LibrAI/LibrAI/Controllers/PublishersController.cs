using LibrAI.Data.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PublishersController : ControllerBase
{
    private readonly IPublisherRepository _repo;
    public PublishersController(IPublisherRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<List<Publisher>>> GetAll(CancellationToken ct) =>
        Ok(await _repo.GetAllAsync(ct));
}
