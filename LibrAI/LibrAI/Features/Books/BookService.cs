using LibrAI.Data.Entities;

public interface IBookService
{
    Task<PagedResult<BookListItemDto>> SearchAsync(string? q, string filter, int page, int pageSize, CancellationToken ct = default);
    Task<BookDetailDto?> GetByIsbnAsync(string isbn, CancellationToken ct = default);
}

public class BookService : IBookService
{
    private readonly IBookRepository _repo;
    public BookService(IBookRepository repo) => _repo = repo;

    public async Task<PagedResult<BookListItemDto>> SearchAsync(string? q, string filter, int page, int pageSize, CancellationToken ct = default)
    {
        var (items, total) = await _repo.SearchAsync(q, filter, page, pageSize, ct);
        var mapped = items.Select(MapToList).ToList();
        return new(mapped, total, page, pageSize);
    }

    public async Task<BookDetailDto?> GetByIsbnAsync(string isbn, CancellationToken ct = default)
    {
        var b = await _repo.GetByIsbnAsync(isbn, ct);
        return b is null ? null : MapToDetail(b);
    }

    private static BookListItemDto MapToList(Book b) =>
        new(b.id, b.title, b.author, b.isbn, b.image_url, /*Available*/ true);

    private static BookDetailDto MapToDetail(Book b) =>
        new(b.id, b.title, b.author, b.isbn, b.publisher, b.page_count, b.language,
            b.description, b.image_url,
            TryDate(b.publish_date), TryPrice(b.price), /*Available*/ true);

    private static DateTime? TryDate(string? s) =>
        DateTime.TryParse(s, out var d) ? d : null;

    private static decimal? TryPrice(string? s) =>
        decimal.TryParse(s, out var p) ? p : null;
}
