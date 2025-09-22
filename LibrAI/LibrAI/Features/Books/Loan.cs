using LibrAI.Data.Entities;

namespace LibrAI.Features.Books
{
    public class Loan
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!; // AspNetUsers.Id (string)
        public int BookId { get; set; }                // books.id
        public DateTime BorrowedAt { get; set; }
        public DateTime DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }

        public Book? Book { get; set; }                // Navi (FK books)
    }

}
