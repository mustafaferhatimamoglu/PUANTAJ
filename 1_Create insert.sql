CREATE TABLE Kullanýcýlar (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Ad NVARCHAR(100) NOT NULL,
    Soyad NVARCHAR(100) NOT NULL,
    Ünvan NVARCHAR(100) NULL
);
GO

CREATE TABLE GirisCikisKayitlari (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    KullanýcýID INT NOT NULL,
    Yön NVARCHAR(50) NOT NULL CHECK (Yön IN ('Giriþ', 'Çýkýþ')),
    ÝþlemTarihi DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (KullanýcýID) REFERENCES Kullanýcýlar(ID) ON DELETE CASCADE
);
GO

INSERT INTO Kullanýcýlar (Ad, Soyad, Ünvan) VALUES 
('Admin', 'Admin', 'Admin'),
('Kullanýcý1', 'Kullanýcý1', 'Kullanýcý1'),
('Kullanýcý2', 'Kullanýcý2', 'Kullanýcý2');

INSERT INTO GirisCikisKayitlari (KullanýcýID, Yön) VALUES
(1, 'Giriþ'),
(1, 'Çýkýþ'),
(1, 'Giriþ');
GO
