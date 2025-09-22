// LibrAI/Features/Books/BooksController.cs
using LibrAI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrAI.Features.Books;

public record BookListItemDto(int Id, string? Title, string? Author, string? Isbn, string? ImageUrl, bool Available);
public record BookDetailDto(
    int Id, string? Title, string? Author, string? Isbn, string? Publisher,
    int? PageCount, string? Language, string? Description, string? ImageUrl,
    DateTime? PublishDate, decimal? Price, bool Available);

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly LibrAiDbContext _db;
    public BooksController(LibrAiDbContext db) { _db = db; }

    [HttpGet]
    public async Task<ActionResult<PagedResult<BookListItemDto>>> Get(string? q, string filter = "all",
        int page = 1, int pageSize = 24, string? sort = null, CancellationToken ct = default)
    {
        var books = _db.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var like = $"%{q}%";
            books = filter switch
            {
                "title" => books.Where(b => EF.Functions.Like(b.title!, like)),
                "author" => books.Where(b => EF.Functions.Like(b.author!, like)),
                "isbn" => books.Where(b => EF.Functions.Like(b.isbn!, like)),
                _ => books.Where(b =>
                               EF.Functions.Like(b.title!, like) ||
                               EF.Functions.Like(b.author!, like) ||
                               EF.Functions.Like(b.isbn!, like))
            };
        }

        // sıralama örnekleri
        books = sort switch
        {
            "new" => books.OrderByDescending(b => b.publish_date),
            "popular" => books.OrderBy(b => b.title), // TODO: popülerlik metrik varsa değiştir
            _ => books.OrderBy(b => b.title)
        };

        var total = await books.CountAsync(ct);

        var pageItems = await books
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookListItemDto(
                b.id,
                b.title,
                b.author,
                b.isbn,
                b.image_url,
                !_db.Loans.Any(l => l.BookId == b.id && l.ReturnedAt == null)
            ))
            .ToListAsync(ct);

        return Ok(new PagedResult<BookListItemDto>(pageItems, total, page, pageSize));
    }

    [HttpGet("{isbn}")]
    public async Task<ActionResult<BookDetailDto>> GetByIsbn(string isbn, CancellationToken ct)
    {
        var b = await _db.Books.AsNoTracking().FirstOrDefaultAsync(x => x.isbn == isbn, ct);
        if (b is null) return NotFound();

        var available = !await _db.Loans.AnyAsync(l => l.BookId == b.id && l.ReturnedAt == null, ct);

        return new BookDetailDto(
            b.id, b.title, b.author, b.isbn, b.publisher,
            b.page_count, b.language, b.description, b.image_url,
            TryParseDate(b.publish_date), TryParsePrice(b.price), available
        );
    }

    private static DateTime? TryParseDate(string? s) =>
        DateTime.TryParse(s, out var d) ? d : null;

    private static decimal? TryParsePrice(string? s) =>
        decimal.TryParse(s, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : null;
}

public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
