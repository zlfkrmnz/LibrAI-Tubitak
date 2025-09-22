using LibrAI.Data;
using LibrAI.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class BookRepository : IBookRepository
{
    private readonly LibrAiDbContext _db;
    public BookRepository(LibrAiDbContext db) => _db = db;

    public async Task<(IReadOnlyList<Book> Items, int Total)> SearchAsync(
        string? q, string filter, int page, int pageSize, CancellationToken ct = default)
    {
        IQueryable<Book> query = _db.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var like = $"%{q.Trim()}%";
            query = filter switch
            {
                "title" => query.Where(b => EF.Functions.Like(b.title!, like)),
                "author" => query.Where(b => EF.Functions.Like(b.author!, like)),
                "isbn" => query.Where(b => EF.Functions.Like(b.isbn!, like)),
                _ => query.Where(b =>
                        EF.Functions.Like(b.title!, like) ||
                        EF.Functions.Like(b.author!, like) ||
                        EF.Functions.Like(b.isbn!, like))
            };
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(b => b.title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct = default) =>
        _db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.isbn == isbn, ct);

    public Task<Book?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.id == id, ct);
}
