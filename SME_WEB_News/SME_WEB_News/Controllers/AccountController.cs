using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace SME_WEB_News.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        public AccountController(ILogger<AccountController> logger
            ,IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return View();
        }

        [HttpPost("Account/Login")]
        public async Task<IActionResult> Loginx(string returnUrl = "/")
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                try
                {
                    var claimsDict = User.Claims.ToDictionary(c => c.Type, c => c.Value);

                    // Log claims for debug
                    foreach (var claim in claimsDict)
                    {
                        Log.Information("Claim: {Type} = {Value}", claim.Key, claim.Value);
                    }

                    // Store in session
                    HttpContext.Session.SetString("UserClaims", JsonSerializer.Serialize(claimsDict));

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Login Exception");
                    return RedirectToAction("Login", new { error = "SAML login failed: " + ex.Message });
                }
            }

            // If not authenticated, challenge SAML2
            await HttpContext.ChallengeAsync("Saml2");
            return new EmptyResult(); // Required after challenge
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User is logging out.");

            await HttpContext.SignOutAsync("Saml2");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // ดึงค่า SingleLogoutServiceUrl
            var logoutUrl = _configuration["Saml2:Saml2:SingleLogoutServiceUrl"];
            return Redirect(logoutUrl);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ViewClaims()
        {
            var claims = User.Claims;
            ViewBag.Claims = claims;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SAMLLogin([FromForm] string samlResponse)
        {
            try
            {
                _logger.LogInformation("SAMLLogin called with SAML response.");

                // Use the Sustainsys.Saml2 library to handle the SAML response
                var result = await HttpContext.AuthenticateAsync("Saml2");

                if (!result.Succeeded || result.Principal == null)
                {
                    throw new Exception("SAML authentication failed or no principal returned.");
                }

                // Log all claims
                foreach (var claim in result.Principal.Claims)
                {
                    _logger.LogInformation("Claim Type: {Type}, Value: {Value}", claim.Type, claim.Value);
                }

                // Sign in with cookie authentication
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    result.Principal);

                ViewBag.Claims = result.Principal.Claims;
                return RedirectToAction("ViewClaims", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SAMLLogin");
                ViewBag.ErrorMessage = "SAML login failed: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SAMLLogin()
        {
            return View();
        }
    }
}