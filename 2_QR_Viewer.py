#pip install pyqrcode pypng pyqt5

import sys
import pyqrcode
import png
from PyQt5.QtWidgets import QApplication, QLabel, QVBoxLayout, QWidget
from PyQt5.QtGui import QPixmap
from PyQt5.QtCore import Qt

# Belirli bir ID ile açılacak sayfa (Örnek ID = 1234)
PAGE_URL = "http://example.com/user?id=1234"

class QRApp(QWidget):
    def __init__(self):
        super().__init__()

        self.setWindowTitle("QR Kod Gösterici")
        self.showFullScreen()  # Tam ekran aç

        layout = QVBoxLayout()

        # QR Kod Oluşturma
        qr = pyqrcode.create(PAGE_URL)
        qr.png("qrcode.png", scale=30)  # QR kodunu büyük oluştur

        # QR Kod Görseli
        self.qr_label = QLabel(self)
        pixmap = QPixmap("qrcode.png")
        self.qr_label.setPixmap(pixmap)
        self.qr_label.setAlignment(Qt.AlignCenter)
        self.qr_label.setScaledContents(True)  # Görseli pencereye sığdır

        layout.addWidget(self.qr_label)
        self.setLayout(layout)

if __name__ == "__main__":
    app = QApplication(sys.argv)
    window = QRApp()
    window.show()
    sys.exit(app.exec_())
