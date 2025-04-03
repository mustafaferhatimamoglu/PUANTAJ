using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace QRScannerWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Database=PUANTAJ;Trusted_Connection=True;TrustServerCertificate=True;";

        public IActionResult Index(string qrData)
        {
            // Check if the user is logged in
            string username = Request.Cookies["UserName"];
            if (string.IsNullOrEmpty(username))
            {
                // Store QR data in a cookie before redirecting
                if (!string.IsNullOrEmpty(qrData))
                {
                    Response.Cookies.Append("QRData", qrData, new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(10)
                    });
                }
                return RedirectToAction("Login", "Auth");
            }

            // Retrieve QR data from cookie if available
            if (string.IsNullOrEmpty(qrData))
            {
                qrData = Request.Cookies["QRData"];
            }

            // Validate QR Code
            if (!string.IsNullOrEmpty(qrData) && !IsValidQRCode(qrData))
            {
                ViewBag.Error = "Invalid or expired QR code.";
                qrData = null; // Reset invalid QR data
            }

            ViewBag.UserName = username;
            ViewBag.QRData = qrData;

            return View();
        }

        private bool IsValidQRCode(string qrData)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM QRCodeLog WHERE QRCode = @qrData AND OlusturmaTarihi > DATEADD(SECOND, -120, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@qrData", qrData);

                int qrCount = (int)cmd.ExecuteScalar();
                return qrCount > 0;
            }
        }
    }
}
