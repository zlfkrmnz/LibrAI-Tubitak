using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrAI.Data;

namespace LibrAI.Controllers
{
    public record StatsDto(int TotalBooks, int AvailableNow, int Loaned);

    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly LibrAiDbContext _db;

        public StatsController(LibrAiDbContext db) => _db = db;

        /// <summary>Kütüphane özet istatistikleri.</summary>
        [HttpGet]
        public async Task<ActionResult<StatsDto>> Get()
        {
            var totalBooks = await _db.Books.CountAsync();

            var loanedBooks = await _db.Loans
                .Where(l => l.ReturnedAt == null)
                .Select(l => l.BookId)
                .Distinct()
                .CountAsync();

            var availableNow = Math.Max(0, totalBooks - loanedBooks);
            return Ok(new StatsDto(totalBooks, availableNow, loanedBooks));
        }
    }
}
