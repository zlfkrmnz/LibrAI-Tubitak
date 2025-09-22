using System;
using System.Collections.Generic;
using LibrAI.Data.Entities;
using LibrAI.Features.Books;
using Microsoft.EntityFrameworkCore;

namespace LibrAI.Data;

public partial class LibrAiDbContext : DbContext
{
    public LibrAiDbContext(DbContextOptions<LibrAiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<Book>(b =>
        {
            b.ToTable("books");
            b.HasKey(x => x.id);
            b.Property(x => x.id).HasColumnName("id");
            b.Property(x => x.title).HasColumnName("title");
            b.Property(x => x.author).HasColumnName("author");
            b.Property(x => x.publisher).HasColumnName("publisher");
            b.Property(x => x.isbn).HasColumnName("isbn");
            b.Property(x => x.page_count).HasColumnName("page_count");
            b.Property(x => x.language).HasColumnName("language");
            b.Property(x => x.publish_date).HasColumnName("publish_date");
            b.Property(x => x.price).HasColumnName("price");
            b.Property(x => x.description).HasColumnName("description");
            b.Property(x => x.image_url).HasColumnName("image_url");
        });

        modelBuilder.Entity<Publisher>(b =>
        {
            b.ToTable("publishers");
            b.HasKey(x => x.id);
            b.Property(x => x.id).HasColumnName("id");
            b.Property(x => x.name).HasColumnName("name");
            b.Property(x => x.url).HasColumnName("url");
            b.Property(x => x.image_url).HasColumnName("image_url");
        });

        modelBuilder.Entity<Loan>(b =>
        {
            b.ToTable("loans");
            b.HasKey(x => x.Id);
            b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            b.Property(x => x.BookId).HasColumnName("book_id").IsRequired();
            b.Property(x => x.BorrowedAt).HasColumnType("TEXT").HasColumnName("borrowed_at");
            b.Property(x => x.DueAt).HasColumnType("TEXT").HasColumnName("due_at");
            b.Property(x => x.ReturnedAt).HasColumnType("TEXT").HasColumnName("returned_at");

            b.HasIndex(x => x.UserId).HasDatabaseName("IX_loans_user");
            b.HasIndex(x => x.BookId).HasDatabaseName("IX_loans_book");
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
