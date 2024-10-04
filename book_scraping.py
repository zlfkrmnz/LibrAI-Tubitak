import sqlite3
import requests
from bs4 import BeautifulSoup

# Kitap sınıfı
class Book:
    def __init__(self, title, author, publisher, isbn, page_count, language, publish_date, price, description, image_url):
        self.title = title
        self.author = author
        self.publisher = publisher
        self.isbn = isbn
        self.page_count = page_count
        self.language = language
        self.publish_date = publish_date
        self.price = price
        self.description = description
        self.image_url = image_url

# Scraper sınıfı
class Scraper:
    def __init__(self, book_url):
        self.book_url = book_url
        self.headers = {
            "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3"
        }

    def get_book_info(self):
        response = requests.get(self.book_url, headers=self.headers)
        soup = BeautifulSoup(response.content, 'html.parser')

        # Kitap bilgilerini HTML'den çekme
        title = soup.find('h1', class_='pr_header__heading').text.strip()
        
        # Yazar bilgisi kontrolü
        author_div = soup.find('div', class_='pr_producers__item')
        author = author_div.text.strip() if author_div else 'Yazar bilgisi bulunamadı'

        # Yayıncı bilgisi kontrolü
        publisher_div = soup.find('div', class_='pr_producers__publisher')
        publisher = publisher_div.text.strip() if publisher_div else 'Yayıncı bilgisi bulunamadı'

        # ISBN kontrolü
        isbn_tag = soup.find('td', string='ISBN:')
        isbn = isbn_tag.find_next_sibling().text.strip() if isbn_tag else 'ISBN bulunamadı'

        # Sayfa sayısı kontrolü ve doğrulama
        page_count_tag = soup.find('td', string='Sayfa Sayısı:')
        page_count_text = page_count_tag.find_next_sibling().text.strip() if page_count_tag else '0'
        
        try:
            page_count = int(page_count_text)
        except ValueError:
            page_count = 0  # Sayfa sayısı bulunamadıysa veya sayısal değilse 0 olarak ayarla

        # Dil kontrolü
        language_tag = soup.find('td', string='Dil:')
        language = language_tag.find_next_sibling().text.strip() if language_tag else 'Dil bilgisi bulunamadı'

        # Yayın tarihi kontrolü
        publish_date_tag = soup.find('td', string='Yayın Tarihi:')
        publish_date = publish_date_tag.find_next_sibling().text.strip() if publish_date_tag else 'Yayın tarihi bulunamadı'

        # Fiyat bilgisi
        price = soup.find('div', class_='price__item').text.strip() if soup.find('div', class_='price__item') else 'Fiyat bilgisi bulunamadı'

        # Kitap açıklamasını çekme (description_text)
        description_div = soup.find('div', id='description_text')
        description = description_div.text.strip() if description_div else "Açıklama bulunamadı"

        # Kitap kapak resmi URL'si kontrolü
        image_div = soup.find('div', class_='book-front')
        image_url = image_div.find('img')['src'] if image_div else "Resim bulunamadı"
        
        return Book(title, author, publisher, isbn, page_count, language, publish_date, price, description, image_url)


# Veritabanı bağlantısı oluşturma ve tablo oluşturma
class Database:
    def __init__(self, db_name):
        self.connection = sqlite3.connect(db_name)
        self.cursor = self.connection.cursor()
        self.create_table()

    def create_table(self):
        self.cursor.execute('''CREATE TABLE IF NOT EXISTS books
                            (id INTEGER PRIMARY KEY AUTOINCREMENT,
                             title TEXT,
                             author TEXT,
                             publisher TEXT,
                             isbn TEXT UNIQUE,
                             page_count INTEGER,
                             language TEXT,
                             publish_date TEXT,
                             price TEXT,
                             description TEXT,
                             image_url TEXT)''')
        self.connection.commit()

    def book_exists(self, isbn):
        self.cursor.execute('''SELECT id FROM books WHERE isbn = ?''', (isbn,))
        return self.cursor.fetchone() is not None

    def insert_book(self, book):
        if not self.book_exists(book.isbn):
            self.cursor.execute('''INSERT INTO books (title, author, publisher, isbn, page_count, language, publish_date, price, description, image_url)
                                   VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)''', 
                                   (book.title, book.author, book.publisher, book.isbn, book.page_count, book.language, book.publish_date, book.price, book.description, book.image_url))
            self.connection.commit()
            print(f"Kitap veritabanına eklendi: {book.title}")
        else:
            print(f"Kitap zaten veritabanında mevcut: {book.title}")

    def close(self):
        self.connection.close()

# Publisher sınıfı
class Publisher:
    def __init__(self, url):
        self.url = url
        self.headers = {
            "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3"
        }

    def extract_publisher_id(self):
        return self.url.split('/')[-1].split('.')[0]

    def get_books_from_publisher(self):
        books = []
        page = 1
        publisher_id = self.extract_publisher_id()
        
        while True:
            url = f"https://www.kitapyurdu.com/index.php?route=product/publisher_products/all&publisher_id={publisher_id}&sort=purchased_365&order=DESC&filter_in_stock=1&page={page}"
            response = requests.get(url, headers=self.headers)
            soup = BeautifulSoup(response.text, 'html.parser')
            
            # Ürün div'lerini bul
            products = soup.find_all("div", class_="product-cr")
            if not products:
                break  # Eğer ürün yoksa döngüyü kır, sayfa bitmiş demektir
            
            for product in products:
                book_url = product.find("a", class_="pr-img-link")['href']
                image_url = product.find("img")['src']
                books.append({
                    "book_url": book_url,
                    "image_url": image_url
                })
            
            # Sayfa ilerletme
            page += 1

        return books

# Veritabanından yayıncıları çekme
class PublisherDatabase:
    def __init__(self, db_name):
        self.db_name = db_name

    def get_publishers_from_db(self):
        conn = sqlite3.connect(self.db_name)
        cursor = conn.cursor()
        cursor.execute("SELECT url FROM publishers")
        publishers = cursor.fetchall()
        conn.close()
        return [pub[0] for pub in publishers]

# Ana uygulama sınıfı
class Application:
    def __init__(self, db_name):
        self.db = Database(db_name)
        self.publisher_db = PublisherDatabase(db_name)

    def run(self):
        # Yayıncı URL'lerini veritabanından al
        publishers = self.publisher_db.get_publishers_from_db()

        # Her yayıncı için kitap bilgilerini topla
        for pub_url in publishers[43:]:
            publisher = Publisher(pub_url)
            books_info = publisher.get_books_from_publisher()
            
            # Her kitap için bilgileri çek ve veritabanına kaydet
            for book_info in books_info:
                scraper = Scraper(book_info['book_url'])
                book = scraper.get_book_info()
                book.image_url = book_info['image_url']  # image_url'i ekle
                self.db.insert_book(book)

        # Veritabanı bağlantısını kapat
        self.db.close()

if __name__ == "__main__":
    app = Application('librai.db')
    app.run()