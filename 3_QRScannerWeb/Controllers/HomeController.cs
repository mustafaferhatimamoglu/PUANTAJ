using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace QRScannerWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Database=PUANTAJ;Trusted_Connection=True;TrustServerCertificate=True;";

        public IActionResult Index(string qrData)
        {
            string username = Request.Cookies["UserName"];
            if (string.IsNullOrEmpty(username))
            {
                if (!string.IsNullOrEmpty(qrData))
                {
                    Response.Cookies.Append("QRData", qrData, new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(10)
                    });
                }
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(qrData))
            {
                qrData = Request.Cookies["QRData"];
            }

            string message = "QR Code not found.";
            if (!string.IsNullOrEmpty(qrData) && !IsValidQRCode(qrData))
            {
                ViewBag.Message = "Invalid or expired QR code.";
                qrData = null; // Reset invalid QR data

                return View();
            }
            if (!string.IsNullOrEmpty(qrData))
            {
                message = ProcessQRScan(qrData);
            }

            ViewBag.UserName = username;
            ViewBag.QRData = qrData;
            ViewBag.Message = message;

            return View();
        }

        private string ProcessQRScan(string qrData)
        {
            string username = Request.Cookies["UserName"];
            if (string.IsNullOrEmpty(username))
            {
                if (!string.IsNullOrEmpty(qrData))
                {
                    Response.Cookies.Append("QRData", qrData, new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(10)
                    });
                }
                RedirectToAction("Login", "Auth");
            }
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string userIdQuery = "SELECT ID FROM Kullanicilar WHERE KullaniciAdi = @QRCode";
                int userId = -1;

                using (SqlCommand cmd = new SqlCommand(userIdQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@QRCode", username);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        userId = Convert.ToInt32(result);
                    }
                }

                if (userId == -1)
                {
                    return "User not found.";
                }

                string lastRecordQuery = "SELECT [Yon] FROM GirisCikisKayitlari WHERE [KullaniciID] = @UserID ORDER BY [IslemTarihi] DESC";
                string lastDirection = "";

                using (SqlCommand cmd = new SqlCommand(lastRecordQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        lastDirection = result.ToString();
                    }
                }

                string newDirection = (lastDirection == "Giris") ? "Cikis" : "Giris";
                string insertQuery = "INSERT INTO GirisCikisKayitlari (KullaniciID, Yon, IslemTarihi) VALUES (@UserID, @Direction, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Direction", newDirection);
                    cmd.ExecuteNonQuery();
                }

                return $"User {userId} has been marked as {newDirection}.";
            }
        }

        private bool IsValidQRCode(string qrData)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM QRCodeLog WHERE QRCode = @qrData -- AND OlusturmaTarihi > DATEADD(SECOND, -120, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@qrData", qrData);

                int qrCount = (int)cmd.ExecuteScalar();
                return qrCount > 0;
            }
        }
    }
}
