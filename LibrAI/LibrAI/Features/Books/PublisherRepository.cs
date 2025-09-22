using LibrAI.Data;
using LibrAI.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class PublisherRepository : IPublisherRepository
{
    private readonly LibrAiDbContext _db;
    public PublisherRepository(LibrAiDbContext db) => _db = db;
    public Task<List<Publisher>> GetAllAsync(CancellationToken ct = default) =>
        _db.Publishers.AsNoTracking().OrderBy(p => p.name).ToListAsync(ct);
}
