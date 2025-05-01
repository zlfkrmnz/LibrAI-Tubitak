namespace LibrAI.Models
{
    public class UserPreference
    {
        public int UserID { get; set; } // Kullanıcı ID'si
        public string FavoriteGenres { get; set; } // Favori türler (virgülle ayrılmış)
        public string FavoriteAuthors { get; set; } // Favori yazarlar (virgülle ayrılmış)
        public string LanguagePreferences { get; set; } // Dil tercihler (virgülle ayrılmış)
    }

}
