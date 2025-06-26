using Microsoft.AspNetCore.Mvc;
using SME_WEB_News.DAO;

namespace SME_WEB_News.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
      
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
            
        }
        [HttpGet("error")]
        public IActionResult Index(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
        }
    }
}