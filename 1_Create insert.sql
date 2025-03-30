CREATE TABLE Kullan�c�lar (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Ad NVARCHAR(100) NOT NULL,
    Soyad NVARCHAR(100) NOT NULL,
    �nvan NVARCHAR(100) NULL
);
GO

CREATE TABLE GirisCikisKayitlari (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Kullan�c�ID INT NOT NULL,
    Y�n NVARCHAR(50) NOT NULL CHECK (Y�n IN ('Giri�', '��k��')),
    ��lemTarihi DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (Kullan�c�ID) REFERENCES Kullan�c�lar(ID) ON DELETE CASCADE
);
GO

INSERT INTO Kullan�c�lar (Ad, Soyad, �nvan) VALUES 
('Admin', 'Admin', 'Admin'),
('Kullan�c�1', 'Kullan�c�1', 'Kullan�c�1'),
('Kullan�c�2', 'Kullan�c�2', 'Kullan�c�2');

INSERT INTO GirisCikisKayitlari (Kullan�c�ID, Y�n) VALUES
(1, 'Giri�'),
(1, '��k��'),
(1, 'Giri�');
GO
