using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SME_WEB_News.DAO;
using SME_WEB_News.Models;
using SME_WEB_News.Services;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
namespace SME_WEB_News.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICallAPIService _callAPIService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        protected static string API_Path_Main;
        protected static string API_Path_Sub;
        protected static string API_Path_Trigger;
        protected static string API_Path_Sub_Trigger;
        protected int currentPageNumber;
        protected static int PageSize;
        protected static int PageSizMedium;
        public HomeController(ILogger<HomeController> logger, ICallAPIService callAPIService
            , IConfiguration configuration
            , IWebHostEnvironment webHostEnvironment
            
            )
        {
            _logger = logger;
                _callAPIService = callAPIService;
            _configuration = configuration;
            API_Path_Main = _configuration.GetValue<string>("API_Path_Main");
            API_Path_Sub = _configuration.GetValue<string>("API_Path_Sub");
            API_Path_Trigger = _configuration.GetValue<string>("API_Path_Trigger");
            API_Path_Sub_Trigger = _configuration.GetValue<string>("API_Path_Sub_Trigger");
            PageSize = _configuration.GetValue<Int32>("PageSize");
            PageSizMedium = _configuration.GetValue<Int32>("PageSizMedium");
            currentPageNumber = 1;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            EmployeeModels model = new EmployeeModels();

            var claimsJson = HttpContext.Session.GetString("UserClaims");
            Dictionary<string, string>? claimsDict = null;
            if (!string.IsNullOrEmpty(claimsJson))
            {
                claimsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);
                string GetClaim(Dictionary<string, string>? dict, string key)
                    => dict != null && dict.ContainsKey(key) ? dict[key] : string.Empty;

                // Use nameidentifier as primary, fallback to userprincipalname, then email
                string email = GetClaim(claimsDict, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier: sso01@osmep.org
                //if (string.IsNullOrEmpty(email))
                //{
                //    email = GetClaim(claimsDict, "userprincipalname");
                //    if (string.IsNullOrEmpty(email))
                //    {
                //        email = GetClaim(claimsDict, "email");
                //    }
                //}
                model.Email = email;
              

                var EmpDetail = await EmployeeDAO.GetDetaiEmp(model, API_Path_Main + API_Path_Sub);
                // Serialize EmpDetail to JSON before storing in session (if needed)
                var empDetailJson = JsonSerializer.Serialize(EmpDetail);
                HttpContext.Session.SetString("EmpDetail", empDetailJson);



                ViewBag.EmpDetail = EmpDetail;
                ViewBag.claimsDict = claimsJson;

            }
            else
            {
                //HttpContext.Session.Remove("EmpDetai");
                //ViewBag.EmpDetai = null;
                //ViewBag.claimsDict = null;

                RedirectToAction("login", "account");

            }
            // ดึง popup ที่ต้องแสดง (ตัวอย่างใช้อันล่าสุดหรือ active)
            ViewHomeModels vhome = new ViewHomeModels();
            PopupModels mpopup = new PopupModels();
            mpopup.StartDateTime = DateTime.Now;
            mpopup.FlagActive = true;
            var popup = PopupDAO.GetPopup(mpopup, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);
            if (popup.listPopupModels == null)
            {
                ViewBag.Popup = null;

            }
            else
            {
                ViewBag.Popup = popup.listPopupModels[0];
            }


            #region news
            //var result = NewsDAO.GetNews(model, API_Path_Main + API_Path_Sub, "", 0, 0, null);
            MNewsModels mNews = new MNewsModels();
            mNews.IsPin = true;
            mNews.IsPublished = true;
            mNews.FlagPage = "PIN";
            mNews.StartDate = DateTime.Now;
            mNews.rowOFFSet = 0;
            mNews.rowFetch = 5;



            var result = NewsDAO.GetNews(mNews, API_Path_Main + API_Path_Sub, "Y", 1, 5, null);
            if (result.ListTMNewsModels == null)
            {

            }
            else
            {
                var firstNews = new MNewsModels
                {
                    ArticlesTitle = result.ListTMNewsModels[0].ArticlesTitle,
                    CoverFilePath = result.ListTMNewsModels[0].CoverFilePath,
                    ArticlesShortDescription = result.ListTMNewsModels[0].ArticlesShortDescription,
                    Id = result.ListTMNewsModels[0].Id,

                };
                if (vhome.viewMNewsModels == null)
                {
                    vhome.viewMNewsModels = new ViewMNewsModels();
                    vhome.viewMNewsModels.MNewsModels = firstNews;
                }

                vhome.viewMNewsModels.ListTMNewsModels = result.ListTMNewsModels;
                vhome.viewMNewsModels.ListTMNewsModels.RemoveAt(0);
            }
            #endregion news

            #region cateNews

            CategoryNewsModels cmodel = new CategoryNewsModels();
            var resultCate = CategoryNewsDAO.GetCategoryNews(cmodel, API_Path_Main + API_Path_Sub, "Y", 1, 5, null);
            if (resultCate.listMCategoryModels == null)
            {
            }
            else
            {
                if (vhome.viewCategoryNewsModels == null)
                {
                    vhome.viewCategoryNewsModels = new ViewCategoryNewsModels();
                    vhome.viewCategoryNewsModels.listMCategoryModels = resultCate.listMCategoryModels
                        .OrderBy(e => e.OrderId)
                        .ToList();
                }
                MNewsModels mAllNews = new MNewsModels();


                mNews.StartDate = DateTime.Now;
                mNews.rowOFFSet = 1;
                mNews.rowFetch = 100;

                var NewsAll = NewsDAO.GetNews(mAllNews, API_Path_Main + API_Path_Sub, "Y", 1, 100, null);
                vhome.viewCategoryNewsModels.listNewsCategoryModels = NewsAll.ListTMNewsModels;

                // Group news by category
                foreach (var cat in vhome.viewCategoryNewsModels.listMCategoryModels)
                {
                    if (cat.CategorieCode == "C000")
                    {
                        cat.NewsList = vhome.viewCategoryNewsModels.listNewsCategoryModels

                          .OrderByDescending(n => n.StartDate)
                          .ToList();
                    }
                    else
                    {
                        cat.NewsList = vhome.viewCategoryNewsModels.listNewsCategoryModels
                           .Where(n => n.CatagoryCode == cat.CategorieCode)
                           .OrderByDescending(n => n.StartDate)
                           .ToList();
                    }

                }
            }

            #endregion

            #region Banner
            ViewBannerNewsModels vBanner = new ViewBannerNewsModels();
            BannerModels mbanner = new BannerModels();
            mbanner.FlagActive = true;


            var resultBanner = BannerNewsDAO.GetBannerNews(mbanner, API_Path_Main + API_Path_Sub, "Y", 1, 10, null);
            if (resultBanner.listBannerModels == null)
            {

            }
            else
            {
                if (vhome.viewBannerNewsModels == null)
                {
                    vhome.viewBannerNewsModels = new ViewBannerNewsModels();
                    vhome.viewBannerNewsModels.listBannerModels = resultBanner.listBannerModels.ToList();
                }
            }
            #endregion Banner


            return View(vhome);

          
        }
        public async Task<IActionResult> admin()
        {
            EmployeeModels model = new EmployeeModels();

            var claimsJson = HttpContext.Session.GetString("UserClaims");
            Dictionary<string, string>? claimsDict = null;
            if (!string.IsNullOrEmpty(claimsJson))
            {
                claimsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);
                string GetClaim(Dictionary<string, string>? dict, string key)
                    => dict != null && dict.ContainsKey(key) ? dict[key] : string.Empty;

                // Use nameidentifier as primary, fallback to userprincipalname, then email
                string email = GetClaim(claimsDict, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier: sso01@osmep.org
                //if (string.IsNullOrEmpty(email))
                //{
                //    email = GetClaim(claimsDict, "userprincipalname");
                //    if (string.IsNullOrEmpty(email))
                //    {
                //        email = GetClaim(claimsDict, "email");
                //    }
                //}
                model.Email = "sso01@osmep.org";
               // Use "username" for Thai name if appropriate

                var EmpDetail = await EmployeeDAO.GetDetaiEmp(model, API_Path_Main + API_Path_Sub);
                // Serialize EmpDetail to JSON before storing in session (if needed)
                var empDetailJson = JsonSerializer.Serialize(EmpDetail);
                HttpContext.Session.SetString("EmpDetail", empDetailJson);



                ViewBag.EmpDetail = EmpDetail;
                ViewBag.claimsDict = claimsJson;
            }
            else
            {
                //HttpContext.Session.Remove("EmpDetai");
                //ViewBag.EmpDetai = null;
                ViewBag.claimsDict = null;
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Admin()
        {
            return View();
        }
    }
}
