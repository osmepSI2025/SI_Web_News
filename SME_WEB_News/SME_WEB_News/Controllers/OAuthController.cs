using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SME_WEB_News.DAO;
using System.Reflection.Metadata.Ecma335; // เพิ่มบรรทัดนี้

namespace SME_WEB_News.Controllers
{
   // [Route("oauth")]
    public class OAuthController : Controller
    {
        private readonly ILogger<OAuthController> _logger;
        private readonly ServiceCenter _serviceCenter; // Assuming you have a ServiceCenter class for service operations
        public OAuthController(ILogger<OAuthController> logger,
            ServiceCenter serviceCenter)
        {
            _logger = logger;
            _serviceCenter = serviceCenter; // Initialize the service center
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            }, "MiniOrange");
        }
        [HttpGet("signin")]
        public IActionResult signin()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            }, "MiniOrange");
        }
        [Authorize]
        [HttpGet("secure")]
        public async Task<IActionResult> Secure()
        {
            var name = User.Identity?.Name;
            var email = User.FindFirst("email")?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");

            // เก็บ access token ใน session
            HttpContext.Session.SetString("AccessToken", accessToken ?? string.Empty);
            HttpContext.Session.SetString("IdToken", idToken ?? string.Empty);

            // return Content($"Hello {name}, your email is {email}\nAccess Token: {accessToken}");
            return RedirectToAction("Index", "Home");
        }
    }
}