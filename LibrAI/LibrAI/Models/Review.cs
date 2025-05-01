namespace LibrAI.Models
{
    public class Review
    {
        public int Id { get; set; } // Yorum ID'si
        public int UserID { get; set; } // Kullanıcı ID'si
        public int BookID { get; set; } // Kitap ID'si
        public DateTime ReviewDate { get; set; } // Yorum tarihi
        public int Rating { get; set; } // Yorum puanı (1-5)
        public string ReviewText { get; set; } // Yorum metni
    }

}
