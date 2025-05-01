namespace LibrAI.Models
{
    public class UserFavorite
    {
        public int UserID { get; set; } // Kullanıcı ID'si
        public int BookID { get; set; } // Kitap ID'si
        public User User { get; set; }  // User ile ilişki
        public Book Book { get; set; }  // Book ile ilişki
    }

}
