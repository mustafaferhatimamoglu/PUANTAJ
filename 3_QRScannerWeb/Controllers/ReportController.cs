using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace QRScannerWeb.Controllers
{
    public class ReportController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Database=PUANTAJ;Trusted_Connection=True;TrustServerCertificate=True;";

        public IActionResult Daily()
        {
            List<DailyEntry> entries = new List<DailyEntry>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
SELECT k.KullaniciAdi, g.Yon, g.IslemTarihi
FROM GirisCikisKayitlari g
INNER JOIN Kullanicilar k ON k.ID = g.KullaniciID
WHERE CAST(g.IslemTarihi AS DATE) = CAST(GETDATE() AS DATE)
ORDER BY g.IslemTarihi DESC
";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entries.Add(new DailyEntry
                        {
                            AdSoyad = reader.GetString(0),
                            Yon = reader.GetString(1),
                            IslemTarihi = reader.GetDateTime(2)
                        });
                    }
                }
            }

            return View(entries);
        }

        public class DailyEntry
        {
            public string AdSoyad { get; set; }
            public string Yon { get; set; }
            public DateTime IslemTarihi { get; set; }
        }
    }
}
