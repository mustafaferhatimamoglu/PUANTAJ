using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;

namespace QRScannerWeb.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username)
        {
            if (!string.IsNullOrEmpty(username))
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

            ViewBag.Error = "Please enter a valid username.";
            return View();
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserName"); // Remove username cookie
            Response.Cookies.Delete("QRData");   // Remove QRData cookie
            return RedirectToAction("Login");
        }
    }
}
