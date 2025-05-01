using LibrAI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrAI.Data
{
    public class LibrAIDbContext : DbContext
    {
        public LibrAIDbContext(DbContextOptions<LibrAIDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // Model ilişkileri ve veritabanı konfigürasyonu
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPreference>()
                .HasKey(up => up.UserID);  // UserID'yi birincil anahtar olarak tanımlıyoruz

            modelBuilder.Entity<UserFavorite>()
                .HasKey(uf => new { uf.UserID, uf.BookID });  // Birincil anahtar olarak UserID ve BookID

            modelBuilder.Entity<UserBook>()
                .HasKey(ub => new { ub.UserID, ub.BookID });  // UserBook için de birincil anahtar

            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasColumnType("decimal(18,2)");  // SQL Server'da decimal(18,2) olarak tanımla

            base.OnModelCreating(modelBuilder);
        }
    }
}
