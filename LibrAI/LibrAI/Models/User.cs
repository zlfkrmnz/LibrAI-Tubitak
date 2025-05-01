namespace LibrAI.Models
{
    public class User
    {
        public int Id { get; set; } // Kullanıcı ID'si
        public string FullName { get; set; } // Kullanıcı adı
        public string Email { get; set; } // E-posta
        public DateTime DateOfBirth { get; set; } // Doğum tarihi
        public DateTime RegistrationDate { get; set; } // Kayıt tarihi
        public string PasswordHash { get; set; } // Şifre hash'i
    }

}
