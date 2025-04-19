#pip install pyqrcode pypng pyqt5
#pip install pyodbc

import sys
import pyqrcode
import random
import string
import pyodbc
from PyQt5.QtWidgets import QApplication, QLabel, QVBoxLayout, QWidget
from PyQt5.QtGui import QPixmap
from PyQt5.QtCore import Qt, QTimer

# MS SQL Database Connection
DB_CONNECTION = "DRIVER={SQL Server};SERVER=DESKTOP-O4TO547;DATABASE=PUANTAJ;Trusted_Connection=yes;"

class QRApp(QWidget):
    def __init__(self):
        super().__init__()

        self.setWindowTitle("QR Kod Gösterici")
        self.showFullScreen()  # Tam ekran aç

        layout = QVBoxLayout()

        # QR Kod Görseli
        self.qr_label = QLabel(self)
        self.qr_label.setAlignment(Qt.AlignCenter)
        self.qr_label.setScaledContents(True)  # Görseli pencereye sığdır
        layout.addWidget(self.qr_label)

        self.setLayout(layout)

        # Timer: QR kodu her 3 saniyede bir güncelle
        self.timer = QTimer(self)
        self.timer.timeout.connect(self.update_qr_code)
        self.timer.start(3000)  

        # İlk QR kodunu oluştur
        self.update_qr_code()

    def generate_random_id(self, length=10):
        """Rastgele ID oluştur"""
        return ''.join(random.choices(string.ascii_letters + string.digits, k=length))

    def update_qr_code(self):
        """QR kodu güncelle ve veritabanına kaydet"""
        random_id = self.generate_random_id()
        #page_url = f"http://example.com/user?id={random_id}"  # Yeni URL
        page_url = f"http://192.168.1.104/Home/Index?QRData={random_id}"  # Yeni URL

        # QR Kod Oluşturma
        qr = pyqrcode.create(page_url)
        qr.png("qrcode.png", scale=30)

        # QR Kodu Güncelle
        pixmap = QPixmap("qrcode.png")
        self.qr_label.setPixmap(pixmap)

        # QR kodunu veritabanına kaydet
        self.save_qr_to_db(random_id)

    def save_qr_to_db(self, qr_code):
        """MS SQL Veritabanına QR Kodunu Kaydet"""
        try:
            conn = pyodbc.connect(DB_CONNECTION)
            cursor = conn.cursor()

            cursor.execute("""
                INSERT INTO QRCodeLog (QRCode) 
                VALUES (?)
            """, (qr_code))  # KullaniciID = 1 (Example)

            conn.commit()
            conn.close()
            print(f"QR Code saved: {qr_code}")

        except Exception as e:
            print(f"Database Error: {e}")
                
        try:
            conn = pyodbc.connect(DB_CONNECTION)
            cursor = conn.cursor()

            cursor.execute("DELETE FROM QRCodeLog WHERE DATEDIFF(SECOND, OlusturmaTarihi, GETDATE()) > 120;")
            conn.commit()
            conn.close()
            print("Old QR codes deleted.")

        except Exception as e:
            print(f"Error deleting old QR codes: {e}")

if __name__ == "__main__":
    app = QApplication(sys.argv)
    window = QRApp()
    window.show()
    sys.exit(app.exec_())
