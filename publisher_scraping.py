import requests
from bs4 import BeautifulSoup
import sqlite3

# Yayıncı sınıfı (Veritabanına kaydedilecek bilgiler)
class Publisher:
    def __init__(self, name, url, image_url):
        self.name = name
        self.url = url
        self.image_url = image_url

    def __repr__(self):
        return f"Publisher(name='{self.name}', url='{self.url}', image_url='{self.image_url}')"

# Veritabanı işlemleri için sınıf
class PublisherDatabase:
    def __init__(self, db_name="librai.db"):
        self.connection = sqlite3.connect(db_name)
        self.cursor = self.connection.cursor()
        self.create_publishers_table()

    def create_publishers_table(self):
        # publishers tablosunu oluştur
        self.cursor.execute('''
            CREATE TABLE IF NOT EXISTS publishers (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                url TEXT NOT NULL,
                image_url TEXT NOT NULL
            )
        ''')
        self.connection.commit()

    def insert_publisher(self, publisher):
        # Yayıncıyı veritabanına ekle
        self.cursor.execute('''INSERT INTO publishers (name, url, image_url)
                               VALUES (?, ?, ?)''', (publisher.name, publisher.url, publisher.image_url))
        self.connection.commit()

    def close(self):
        # Veritabanı bağlantısını kapat
        self.connection.close()

# Yayıncı bilgilerini çekme işlemi için sınıf
class PublisherScraper:
    def __init__(self, base_url):
        self.base_url = base_url
        self.publishers = []
        self.headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3'}

    def get_html(self, url):
        response = requests.get(url, headers=self.headers)
        if response.status_code == 200:
            return response.content
        else:
            raise Exception(f"Failed to retrieve content from {url}")

    def parse_publisher_page(self):
        html = self.get_html(self.base_url)
        soup = BeautifulSoup(html, 'html.parser')

        # Yayıncıların bulunduğu div'leri bul
        publishers = soup.find_all("div", class_="column list-item")

        # Her bir yayıncının URL'sini ve resim URL'sini çek
        for publisher in publishers:
            a_tag = publisher.find("a", class_="alt list-item-link")
            img_tag = publisher.find("img", class_="list-item-logo")
            name_tag = publisher.find("div", class_="list-item-name")

            if a_tag and img_tag and name_tag:
                publisher_name = name_tag.text.strip()
                publisher_url = a_tag['href']
                publisher_image_url = img_tag['src']
                self.publishers.append(Publisher(publisher_name, publisher_url, publisher_image_url))

    def get_publishers(self):
        self.parse_publisher_page()
        return self.publishers

if __name__ == "__main__":
    base_url = "https://www.kitapyurdu.com/yayincilar"
    scraper = PublisherScraper(base_url)
    
    # Yayıncıları çek
    publishers = scraper.get_publishers()

    # Veritabanı bağlantısını başlat (librai.db)
    db = PublisherDatabase()

    # Yayıncıları veritabanına ekle
    for publisher in publishers:
        db.insert_publisher(publisher)

    # Veritabanı bağlantısını kapat
    db.close()

    # Yayıncı bilgilerini yazdır
    for publisher in publishers:
        print(publisher)
