public record BookListItemDto(
    int Id, string? Title, string? Author, string? Isbn,
    string? ImageUrl, bool Available);

public record BookDetailDto(
    int Id, string? Title, string? Author, string? Isbn,
    string? Publisher, int? PageCount, string? Language,
    string? Description, string? ImageUrl, DateTime? PublishDate, decimal? Price,
    bool Available);

public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
