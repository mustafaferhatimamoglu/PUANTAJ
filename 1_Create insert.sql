-- 1️⃣ Users Table: Stores user login info
CREATE TABLE Kullanicilar (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    KullaniciAdi NVARCHAR(100) UNIQUE NOT NULL,
    Sifre NVARCHAR(255) NOT NULL -- Store hashed passwords
);

-- 2️⃣ QR Code Log Table: Stores generated QR codes and approval status
CREATE TABLE QRCodeLog (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    QRCode NVARCHAR(255) NOT NULL,
    KullaniciID INT FOREIGN KEY REFERENCES Kullanicilar(ID) ON DELETE CASCADE,
    YoneticiOnayi BIT DEFAULT 0,  -- 0 = Pending, 1 = Approved
    OlusturmaTarihi DATETIME DEFAULT GETDATE() -- Timestamp when QR is generated
);

-- 3️⃣ Entry-Exit Records Table: Stores attendance logs
CREATE TABLE GirisCikisKayitlari (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    KullaniciID INT FOREIGN KEY REFERENCES Kullanicilar(ID) ON DELETE CASCADE,
    Yon NVARCHAR(50) NOT NULL CHECK (Yon IN ('Giris', 'Cikis')), -- Entry or Exit
    IslemTarihi DATETIME DEFAULT GETDATE() -- Timestamp of the action
);
