using LibrAI.Data;
using LibrAI.Data.Auth;
using LibrAI.Features.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrAI.Features.Loans;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly LibrAiDbContext _db;
    private readonly UserManager<AppUser> _um;

    public LoansController(LibrAiDbContext db, UserManager<AppUser> um)
    {
        _db = db;
        _um = um;
    }

    [HttpPost("borrow/{bookId:int}")]
    public async Task<IActionResult> Borrow(int bookId, CancellationToken ct)
    {
        var user = await _um.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var active = await _db.Loans.AnyAsync(l => l.BookId == bookId && l.ReturnedAt == null, ct);
        if (active) return Conflict("Bu kitap zaten ödünçte.");

        var loan = new Loan
        {
            UserId = user.Id,
            BookId = bookId,
            BorrowedAt = DateTime.UtcNow,
            DueAt = DateTime.UtcNow.AddDays(14)
        };
        _db.Loans.Add(loan);
        await _db.SaveChangesAsync(ct);
        return Ok(loan.Id);
    }

    [HttpPost("return/{bookId:int}")]
    public async Task<IActionResult> Return(int bookId, CancellationToken ct)
    {
        var user = await _um.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var loan = await _db.Loans
            .FirstOrDefaultAsync(l => l.BookId == bookId && l.UserId == user.Id && l.ReturnedAt == null, ct);
        if (loan is null) return NotFound("Aktif ödünç kaydı bulunamadı.");

        loan.ReturnedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpGet("my")]
    public async Task<IActionResult> MyLoans(CancellationToken ct)
    {
        var user = await _um.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var list = await _db.Loans
            .Where(l => l.UserId == user.Id)
            .OrderByDescending(l => l.BorrowedAt)
            .ToListAsync(ct);

        return Ok(list);
    }
}
