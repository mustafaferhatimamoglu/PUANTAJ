using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace QRScannerWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Database=PUANTAJ;Trusted_Connection=True;TrustServerCertificate=True;";

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (IsValidUser(username, password))
            {
                // Store username in a cookie (valid for 7 days)
                Response.Cookies.Append("UserName", username, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7)
                });

                // Retrieve QRData from cookies (if available)
                string qrData = Request.Cookies["QRData"];
                if (!string.IsNullOrEmpty(qrData))
                {
                    return RedirectToAction("Index", "Home", new { qrData });
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        private bool IsValidUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Kullanicilar WHERE KullaniciAdi = @username AND Sifre = @password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                int userCount = (int)cmd.ExecuteScalar();
                return userCount > 0;
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserName"); // Remove username cookie
            Response.Cookies.Delete("QRData");   // Remove QRData cookie
            return RedirectToAction("Login");
        }
    }
}
