namespace LibrAI.Models
{
    public class Book
    {
        public int Id { get; set; } // Kitap ID'si
        public string Title { get; set; } // Kitap başlığı
        public string Author { get; set; } // Yazar
        public string Publisher { get; set; } // Yayıncı
        public string ISBN { get; set; } // ISBN numarası
        public int PageCount { get; set; } // Sayfa sayısı
        public string Language { get; set; } // Dil
        public DateTime PublishDate { get; set; } // Yayın tarihi
        public decimal Price { get; set; } // Fiyat
        public string Description { get; set; } // Kitap açıklaması
        public string ImageUrl { get; set; } // Kitap görseli URL'si
    }


}
