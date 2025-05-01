using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LibrAI.Models;
using LibrAI.Services;

namespace LibrAI.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookService _bookService;

        // Dependency Injection ile BookService alıyoruz
        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        // GET: Books (Kitap listesini gösterme)
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksAsync();
            return View(books);  // Kitapları View'a gönderiyoruz
        }

        // GET: Books/Create (Yeni kitap ekleme sayfası)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create (Yeni kitap ekleme işlemi)
        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                await _bookService.AddBookAsync(book);  // Kitap ekleme işlemi
                return RedirectToAction(nameof(Index));  // Kitap eklendikten sonra liste sayfasına yönlendirme
            }
            return View(book);  // Geçersiz model durumunda aynı sayfada kalır
        }

        // GET: Books/Edit/5 (Kitap düzenleme sayfası)
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();  // Kitap bulunmazsa hata sayfasına yönlendirme
            }
            return View(book);  // Kitap düzenleme sayfasına gönderme
        }

        // POST: Books/Edit/5 (Kitap güncelleme işlemi)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id)
            {
                return NotFound();  // ID eşleşmezse hata sayfasına yönlendirme
            }

            if (ModelState.IsValid)
            {
                await _bookService.UpdateBookAsync(book);  // Kitap güncelleme işlemi
                return RedirectToAction(nameof(Index));  // Güncelleme sonrası liste sayfasına yönlendirme
            }
            return View(book);  // Geçersiz model durumunda düzenleme sayfasına geri dönme
        }

        // GET: Books/Delete/5 (Kitap silme sayfası)
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();  // Kitap bulunmazsa hata sayfasına yönlendirme
            }
            return View(book);  // Silme sayfasına gönderme
        }

        // POST: Books/Delete/5 (Kitap silme işlemi)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookService.DeleteBookAsync(id);  // Kitap silme işlemi
            return RedirectToAction(nameof(Index));  // Silme sonrası liste sayfasına yönlendirme
        }
    }
}
