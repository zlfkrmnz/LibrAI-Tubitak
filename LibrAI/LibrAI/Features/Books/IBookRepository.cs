using LibrAI.Data.Entities;

public interface IBookRepository
{
    Task<(IReadOnlyList<Book> Items, int Total)> SearchAsync(
        string? q, string filter, int page, int pageSize, CancellationToken ct = default);
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct = default);
    Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);
}
