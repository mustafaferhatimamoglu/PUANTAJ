using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace QRScannerWeb.Controllers
{
    public class HomeController : Controller
    {
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
                        Expires = DateTime.Now.AddMinutes(10) // Store for 10 minutes
                    });
                }
                return RedirectToAction("Login", "Auth");
            }

            // Retrieve QR data from cookie if available
            if (string.IsNullOrEmpty(qrData))
            {
                qrData = Request.Cookies["QRData"];
            }

            ViewBag.UserName = username;
            ViewBag.QRData = qrData;

            return View();
        }
    }
}
