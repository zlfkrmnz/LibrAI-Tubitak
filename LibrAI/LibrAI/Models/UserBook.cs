namespace LibrAI.Models
{
    public class UserBook
    {
        public int UserID { get; set; } // Kullanıcı ID'si
        public int BookID { get; set; } // Kitap ID'si
        public DateTime BorrowDate { get; set; } // Ödünç alma tarihi
        public DateTime? ReturnDate { get; set; } // İade tarihi (isteğe bağlı)
    }

}
