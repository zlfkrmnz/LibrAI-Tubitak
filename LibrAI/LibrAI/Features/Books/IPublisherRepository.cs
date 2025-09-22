using LibrAI.Data.Entities;

public interface IPublisherRepository
{
    Task<List<Publisher>> GetAllAsync(CancellationToken ct = default);
}
