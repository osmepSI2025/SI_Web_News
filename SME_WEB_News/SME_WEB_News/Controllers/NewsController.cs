using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SME_WEB_News.DAO;
using SME_WEB_News.Models;
using SME_WEB_News.Services;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SME_WEB_News.Controllers
{
    public class NewsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NewsController> _logger;
        protected static string API_Path_Main;
        protected static string API_Path_Sub;
        protected static string API_Path_Trigger;
        protected static string API_Path_Sub_Trigger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly CallAPIService _callAPIService;
        protected int currentPageNumber;
        protected static int PageSize;
        protected static int PageSizMedium;

        protected static string UrlDefualt;
        
        public NewsController(ILogger<NewsController> logger, IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment
            ,CallAPIService callAPIService)
        {
            _logger = logger;
            _configuration = configuration;
            API_Path_Main = _configuration.GetValue<string>("API_Path_Main");
            API_Path_Sub = _configuration.GetValue<string>("API_Path_Sub");
            API_Path_Trigger = _configuration.GetValue<string>("API_Path_Trigger");
            API_Path_Sub_Trigger = _configuration.GetValue<string>("API_Path_Sub_Trigger");
            PageSize = _configuration.GetValue<Int32>("PageSize");
            PageSizMedium = _configuration.GetValue<Int32>("PageSizMedium");
            currentPageNumber = 1;
            _webHostEnvironment = webHostEnvironment;

            _callAPIService = callAPIService;
            UrlDefualt = _configuration.GetValue<string>("UrlDefualt");
        }
     
        public async Task<IActionResult> PinNews(ViewMNewsModels vm, string previous, string first, string next, string last, string hidcurrentpage, string hidtotalpage,

            string searchNews = null, string DeleteNews = null, string saveNews = null, string cancelNews = null, string editNews = null
            , string submitAction = null
            )
        {
            ViewBag.EmpDetail = HttpContext.Session.GetString("EmpDetail");
            var empDetailJson = HttpContext.Session.GetString("EmpDetail");

            if (!string.IsNullOrEmpty(empDetailJson))
            {
                var empDetailObj = JsonSerializer.Deserialize<EmployeeRoleModels>(empDetailJson);
                if (empDetailObj == null || string.IsNullOrEmpty(empDetailObj.RoleCode))
                {
                    if (empDetailObj.RoleCode == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            #region panging
            int curpage = 0;
            int totalpage = 0;
            ViewMNewsModels result = new ViewMNewsModels();

            if (!string.IsNullOrEmpty(hidcurrentpage)) curpage = Convert.ToInt32(hidcurrentpage);
            if (!string.IsNullOrEmpty(hidtotalpage)) totalpage = Convert.ToInt32(hidtotalpage);
            if (!string.IsNullOrEmpty(first)) currentPageNumber = 1;
            else if (!string.IsNullOrEmpty(previous)) currentPageNumber = (curpage == 1) ? 1 : curpage - 1;
            else if (!string.IsNullOrEmpty(next)) currentPageNumber = (curpage == totalpage) ? totalpage : curpage + 1;
            else if (!string.IsNullOrEmpty(last)) currentPageNumber = totalpage;

            int PageSizeDummy = PageSize;
            int totalCount = 0;
            PageSize = PageSizeDummy;


            if (!string.IsNullOrEmpty(saveNews))
            {
                var data = vm;
                foreach (var item in vm.ListUpdateRangingNewsModels)
                {
                    // ใช้งาน item.Id และ item.OrderId ได้เลย
                    var updateResult = NewsDAO.UpdateRangeNews(item, API_Path_Main + API_Path_Sub, null);
                }

            }
            else if (!string.IsNullOrEmpty(cancelNews))
            {
                MNewsModels smodel = new MNewsModels();
                smodel.IsPin = true;
                smodel.FlagPage = "PIN";
                result =    await NewsDAO.GetNews(smodel, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);

            }
            else
            {
                MNewsModels smodel = new MNewsModels();
                smodel.IsPin = true;
                smodel.FlagPage = "PIN";
                smodel.StartDate = vm.SearchMNewsModels.StartDate;
                smodel.EndDate = vm.SearchMNewsModels.EndDate;
                smodel.CatagoryCode = vm.SearchMNewsModels.CatagoryCode;
                smodel.IsPublished = vm.SearchMNewsModels.IsPublished;
                smodel.ArticlesTitle = vm.SearchMNewsModels.ArticlesTitle;
                smodel.PublishDate = vm.SearchMNewsModels.PublishDate;
                result = await NewsDAO.GetNews(smodel, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);

            }
            // dropdown 
            if (result.ListTMNewsModels != null)
            {
                result.PageModel = ServiceCenter.LoadPagingViewModel(result.TotalRowsList ?? 0, currentPageNumber, PageSize);
            }
            else
            {
                result.PageModel = ServiceCenter.LoadPagingViewModel(0, currentPageNumber, PageSize);
            }

            result.DDLpin = ServiceCenter.GetLookups("STATUS", API_Path_Main + API_Path_Sub, null);
            result.DDLpublish = ServiceCenter.GetLookups("STATUS", API_Path_Main + API_Path_Sub, null);

            result.DDLCategory = ServiceCenter.GetLookups("GetDropdownCategory", API_Path_Main + API_Path_Sub);


            var serviceCenter = new ServiceCenter(_configuration, _callAPIService);
            result.DDLDepartment = await serviceCenter.GetDdlDepartment(API_Path_Main + API_Path_Sub, "business-units");
            ViewBag.DDLDepartment = new SelectList(result.DDLDepartment.DropdownList.OrderBy(x => x.Code), "Code", "Name");

            ViewBag.DDLCategory = new SelectList(result.DDLCategory.DropdownList.OrderBy(x => x.Code), "Code", "Name");
            ViewBag.DDLpin = new SelectList(result.DDLpin.DropdownList.OrderBy(x => x.Code), "Code", "Name");
            ViewBag.DDLpublish = new SelectList(result.DDLpublish.DropdownList.OrderBy(x => x.Code), "Code", "Name");
            //

            return View(result);
            #endregion End panging


        }
        public async Task<IActionResult> CreateNews(ViewMNewsModels vm, string previous, string first, string next, string last, string hidcurrentpage, string hidtotalpage,

            string searchNews = null, string DeleteNews = null, string saveNews = null, string cancelNews = null,string editNews=null
            , string submitAction =null 
            )
        {
            ViewBag.EmpDetail = HttpContext.Session.GetString("BusinessUnitId");
            ViewBag.EmpDetail = HttpContext.Session.GetString("EmpDetail");
            var empDetailJson = HttpContext.Session.GetString("EmpDetail");
            EmployeeRoleModels empDetailObj = new EmployeeRoleModels();
            if (!string.IsNullOrEmpty(empDetailJson))
            {
                 empDetailObj = JsonSerializer.Deserialize<EmployeeRoleModels>(empDetailJson);
                if (empDetailObj == null || string.IsNullOrEmpty(empDetailObj.RoleCode))
                {
                    if (empDetailObj.RoleCode == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            #region panging
            int curpage = 0;
            int totalpage = 0;
            ViewMNewsModels result = new ViewMNewsModels();

            if (!string.IsNullOrEmpty(hidcurrentpage)) curpage = Convert.ToInt32(hidcurrentpage);
            if (!string.IsNullOrEmpty(hidtotalpage)) totalpage = Convert.ToInt32(hidtotalpage);
            if (!string.IsNullOrEmpty(first)) currentPageNumber = 1;
            else if (!string.IsNullOrEmpty(previous)) currentPageNumber = (curpage == 1) ? 1 : curpage - 1;
            else if (!string.IsNullOrEmpty(next)) currentPageNumber = (curpage == totalpage) ? totalpage : curpage + 1;
            else if (!string.IsNullOrEmpty(last)) currentPageNumber = totalpage;

            int PageSizeDummy = PageSize;
            int totalCount = 0;
            PageSize = PageSizeDummy;


            if (!string.IsNullOrEmpty(submitAction))
            {
                var data = vm;
                if (vm.MNewsModels.Id == 0)
                {
                    // Handle file uploads
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "news");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Cover File
                    if (vm.MNewsModels.CoverFile != null && vm.MNewsModels.CoverFile.Length > 0)
                    {
                        var coverFileName = Guid.NewGuid() + Path.GetExtension(vm.MNewsModels.CoverFile.FileName);
                        var coverFilePath = Path.Combine(uploadsFolder, coverFileName);
                        using (var stream = new FileStream(coverFilePath, FileMode.Create))
                        {
                            vm.MNewsModels.CoverFile.CopyTo(stream);
                        }
                        // Save relative path to DB
                        vm.MNewsModels.CoverFilePath = $"/uploads/news/{coverFileName}";
                    }

                    // PicNews Files (Multiple images support)
                    if (vm.MNewsModels.PicNewsFile != null && vm.MNewsModels.PicNewsFile.Count > 0)
                    {
                        var picFilePaths = new List<string>();
                        foreach (var file in vm.MNewsModels.PicNewsFile)
                        {
                            if (file != null && file.Length > 0)
                            {
                                var picFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                                var picFilePath = Path.Combine(uploadsFolder, picFileName);
                                using (var stream = new FileStream(picFilePath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                                picFilePaths.Add($"/uploads/news/{picFileName}");
                            }
                        }
                        // Store as comma-separated string, or adjust as needed for your DB/model
                        vm.MNewsModels.PicNewsFilePath = string.Join(",", picFilePaths);
                    }

                    // News File (Multiple files support)
                    if (vm.MNewsModels.NewsFile != null && vm.MNewsModels.NewsFile.Count > 0)
                    {
                        var newsFilePaths = new List<string>();
                        foreach (var file in vm.MNewsModels.NewsFile)
                        {
                            if (file != null && file.Length > 0)
                            {
                                var newsFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                                var newsFilePath = Path.Combine(uploadsFolder, newsFileName);
                                using (var stream = new FileStream(newsFilePath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                                newsFilePaths.Add($"/uploads/news/{newsFileName}");
                            }
                        }
                        // Store as comma-separated string, or adjust as needed for your DB/model
                        vm.MNewsModels.NewsFilePath = string.Join(",", newsFilePaths);
                    }
                    if (vm.MNewsModels.NewsFile != null && vm.MNewsModels.NewsFile.Count > 0)
                    {
                        var originalFileNames = vm.MNewsModels.NewsFile
                            .Where(f => f != null && f.Length > 0)
                            .Select(f => Path.GetFileName(f.FileName))
                            .ToList();

                        vm.MNewsModels.FileNameOriginal = string.Join(",", originalFileNames);
                    }

                    //save news
                    var CreateNwesx = NewsDAO.CreateNews(vm.MNewsModels, API_Path_Main + API_Path_Sub, null);

                    if (submitAction == "saveAndSendMail")
                    {
                        var mailTo = Request.Form["MailTo"];
                        var mailSubject = CreateNwesx.ArticlesTitle;
                        string encode = ServiceCenter.EncodeBase64(CreateNwesx.Id.ToString());
                        var mailBody = "เรียนทุกท่าน " +
                                       "<br><br>" + // ขึ้นบรรทัดใหม่เพื่อให้อ่านง่าย
                                       "จากข่าว " + CreateNwesx.ArticlesTitle +
 " **<a href=" + UrlDefualt + "/News/PreviewNews/" + encode + ">คลิกที่นี่</a>** เพื่อดูรายละเอียดข่าว"; // <--- แก้ไขแล้ว!
                        var mailService = new MailService(_configuration);
                        await mailService.SendMailAsync(mailTo, mailSubject, mailBody);
                    }

                    //  result = await NewsDAO.GetNews(vm.SearchMNewsModels, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);
                    // หลังบันทึกสำเร็จ
                    var displayModel = new MNewsDisplayModel
                    {
                        Id = vm.MNewsModels.Id,
                        CatagoryName = vm.MNewsModels.CatagoryName,
                        ArticlesTitle = vm.MNewsModels.ArticlesTitle,
                        ArticlesContent = vm.MNewsModels.ArticlesContent,
                        PublishDate = vm.MNewsModels.PublishDate,
                        StartDate = vm.MNewsModels.StartDate,
                        EndDate = vm.MNewsModels.EndDate,
                        IsPublished = vm.MNewsModels.IsPublished ?? false,
                    };
                    TempData["SavedNews"] = JsonSerializer.Serialize(displayModel);
                    return RedirectToAction("CreateNews");
                }
                else
                {

                    // Handle file uploads
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "news");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Cover File
                    if (vm.MNewsModels.CoverFile != null && vm.MNewsModels.CoverFile.Length > 0)
                    {
                        var coverFileName = Guid.NewGuid() + Path.GetExtension(vm.MNewsModels.CoverFile.FileName);
                        var coverFilePath = Path.Combine(uploadsFolder, coverFileName);
                        using (var stream = new FileStream(coverFilePath, FileMode.Create))
                        {
                            vm.MNewsModels.CoverFile.CopyTo(stream);
                        }
                        // Save relative path to DB
                        vm.MNewsModels.CoverFilePath = $"/uploads/news/{coverFileName}";
                    }

                  
                    // PicNews Files (Multiple images support)
                    if (vm.MNewsModels.PicNewsFile != null && vm.MNewsModels.PicNewsFile.Count > 0)
                    {
                        var picFilePaths = new List<string>();
                        foreach (var file in vm.MNewsModels.PicNewsFile)
                        {
                            if (file != null && file.Length > 0)
                            {
                                var picFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                                var picFilePath = Path.Combine(uploadsFolder, picFileName);
                                using (var stream = new FileStream(picFilePath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                                picFilePaths.Add($"/uploads/news/{picFileName}");
                            }
                        }
                        // Store as comma-separated string, or adjust as needed for your DB/model
                        vm.MNewsModels.PicNewsFilePath = string.Join(",", picFilePaths);
                    }

                    // News File (Multiple files support)
                    if (vm.MNewsModels.NewsFile != null && vm.MNewsModels.NewsFile.Count > 0)
                    {
                        var newsFilePaths = new List<string>();
                        foreach (var file in vm.MNewsModels.NewsFile)
                        {
                            if (file != null && file.Length > 0)
                            {
                                var newsFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                                var newsFilePath = Path.Combine(uploadsFolder, newsFileName);

                                using (var stream = new FileStream(newsFilePath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                                newsFilePaths.Add($"/uploads/news/{newsFileName}");
                            }
                        }
                        // Store as comma-separated string, or adjust as needed for your DB/model
                        vm.MNewsModels.NewsFilePath = string.Join(",", newsFilePaths);

                    }
                    if (vm.MNewsModels.NewsFile != null && vm.MNewsModels.NewsFile.Count > 0)
                    {
                        var originalFileNames = vm.MNewsModels.NewsFile
                            .Where(f => f != null && f.Length > 0)
                            .Select(f => Path.GetFileName(f.FileName))
                            .ToList();

                        vm.MNewsModels.FileNameOriginal = string.Join(",", originalFileNames);
                    }

                    var CreateNwesx = NewsDAO.EditNews(vm.MNewsModels, API_Path_Main + API_Path_Sub, null);

                    if (submitAction== "saveAndSendMail") 
                    {
           
                        var mailTo = Request.Form["MailTo"];
                        var mailSubject = vm.MNewsModels.ArticlesTitle;
                        string encode = ServiceCenter.EncodeBase64(vm.MNewsModels.Id.ToString());

                        var mailBody = "เรียนทุกท่าน " + 
                                       "<br><br>" + // ขึ้นบรรทัดใหม่เพื่อให้อ่านง่าย
                                       "จากข่าว " + vm.MNewsModels.ArticlesTitle +
                                       " **<a href="+ UrlDefualt + "/News/PreviewNews/" + encode + ">คลิกที่นี่</a>** เพื่อดูรายละเอียดข่าว"; // <--- แก้ไขแล้ว!
                        var mailService = new MailService(_configuration);
                        await mailService.SendMailAsync(mailTo, mailSubject, mailBody);

                     //   result = await NewsDAO.GetNews(vm.SearchMNewsModels, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);
                    
                    }

                    // หลังบันทึกสำเร็จ
                    var displayModel = new MNewsDisplayModel
                    {
                        Id = CreateNwesx.Id,
                        CatagoryName = CreateNwesx.CatagoryName,
                        ArticlesTitle = CreateNwesx.ArticlesTitle,
                        ArticlesContent = CreateNwesx.ArticlesContent,
                        PublishDate = CreateNwesx.PublishDate,
                        StartDate = CreateNwesx.StartDate,
                        EndDate = CreateNwesx.EndDate,
                        IsPublished = CreateNwesx.IsPublished??false,
                    };
                    TempData["SavedNews"] = JsonSerializer.Serialize(displayModel);
                    return RedirectToAction("CreateNews");
                }

            }
                
          
            else if (!string.IsNullOrEmpty(cancelNews))
            {
                result =await NewsDAO.GetNews(vm.SearchMNewsModels, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);

            }
            else if (!string.IsNullOrEmpty(DeleteNews))
            {
                var DeleteNwesx = NewsDAO.DeleteNewsAsync(vm.MNewsModels.Id, API_Path_Main + API_Path_Sub, null);
                return RedirectToAction("CreateNews");
            }
            else
            {
                var mnews = new MNewsModels();
                mnews.CatagoryCode = vm.SearchMNewsModels.CatagoryCode;
                mnews.StartDate = vm.SearchMNewsModels.StartDate;
                mnews.EndDate = vm.SearchMNewsModels.EndDate;
                mnews.ArticlesTitle = vm.SearchMNewsModels.ArticlesTitle;
                mnews.IsPublished = vm.SearchMNewsModels.IsPublished;
                mnews.PublishDate = vm.SearchMNewsModels.PublishDate;
                mnews.FlagPage = "SEARCH";
              
                if (empDetailObj.RoleCode == "ADMIN")
                {
                
                    mnews.BusinessUnitId = empDetailObj.BusinessUnitId; // Set to "ALL" for ADMIN role
                    result = await NewsDAO.GetNews(mnews, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);

                }
                else 
                {
                    result = await NewsDAO.GetNews(mnews, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);

                }

            }
            // dropdown 
            if( result.ListTMNewsModels != null)
            {
                result.PageModel = ServiceCenter.LoadPagingViewModel(result.TotalRowsList ?? 0, currentPageNumber, PageSize);
            }else
            {
                result.PageModel = ServiceCenter.LoadPagingViewModel(0, currentPageNumber, PageSize);
            }
           
            result.DDLpin = ServiceCenter.GetLookups("STATUS", API_Path_Main + API_Path_Sub, null);
            result.DDLpublish = ServiceCenter.GetLookups("STATUS", API_Path_Main + API_Path_Sub, null);
         
            result.DDLCategory = ServiceCenter.GetLookups("GetDropdownCategory", API_Path_Main + API_Path_Sub);


            var serviceCenter = new ServiceCenter(_configuration, _callAPIService);
            result.DDLDepartment = await serviceCenter.GetDdlDepartment(API_Path_Main + API_Path_Sub, "business-units");
            ViewBag.DDLDepartment = new SelectList(result.DDLDepartment.DropdownList.OrderBy(x => x.Code), "Code", "Name");

            ViewBag.DDLCategory = new SelectList(result.DDLCategory.DropdownList.OrderBy(x => x.Code), "Code", "Name");
            ViewBag.DDLpin = new SelectList(result.DDLpin.DropdownList.OrderBy(x => x.Code), "Code", "Name");
            ViewBag.DDLpublish = new SelectList(result.DDLpublish.DropdownList.OrderBy(x => x.Code), "Code", "Name");
            //
            //HttpContext.Session.SetString("EmployeeRole", EmpDetail.RoleCode);
            if (empDetailObj.RoleCode == "ADMIN") 
            {
                var mnews = new MNewsModels();
                mnews.BusinessUnitId = empDetailObj.BusinessUnitId; // Set to "ALL" for ADMIN role
                result.MNewsModels = mnews;
            }

            if (result.MNewsModels==null)
            {
                result.MNewsModels = new MNewsModels();
            }

            return View(result);
            #endregion End panging


        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // สร้าง Path เก็บไฟล์
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // ส่ง URL กลับไปให้ Quill
            var url = $"/uploads/{fileName}";
            return Json(new { url });
        }
        public async Task<IActionResult> TestEditor(ViewMNewsModels vm, string previous, string first, string next, string last, string hidcurrentpage, string hidtotalpage,

           string searchNews = null, string Delete = null, string saveNews = null, string cancelNews = null)
        {
            //  var tokenStr = HttpContext.Session.GetString("Token");
            //if (string.IsNullOrEmpty(tokenStr))
            //{
            //    return RedirectToAction("Logout", "Authentication");
            //}
            //panging
            #region panging
            int curpage = 0;
            int totalpage = 0;
            ViewMNewsModels result = new ViewMNewsModels();

            if (!string.IsNullOrEmpty(hidcurrentpage)) curpage = Convert.ToInt32(hidcurrentpage);
            if (!string.IsNullOrEmpty(hidtotalpage)) totalpage = Convert.ToInt32(hidtotalpage);
            if (!string.IsNullOrEmpty(first)) currentPageNumber = 1;
            else if (!string.IsNullOrEmpty(previous)) currentPageNumber = (curpage == 1) ? 1 : curpage - 1;
            else if (!string.IsNullOrEmpty(next)) currentPageNumber = (curpage == totalpage) ? totalpage : curpage + 1;
            else if (!string.IsNullOrEmpty(last)) currentPageNumber = totalpage;

            int PageSizeDummy = PageSize;
            int totalCount = 0;
            PageSize = PageSizeDummy;
            if (!string.IsNullOrEmpty(saveNews))
            {
                var data = vm;
                //save news
            }
            else if (!string.IsNullOrEmpty(cancelNews))
            {
                result = await NewsDAO.GetNews(vm.SearchMNewsModels, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);
                result.MNewsModels = new MNewsModels
                {
                    ArticlesContent = "<p>เนื้อหาข่าวตัวอย่าง</p>"
                };
            }
            else
            {

                result =await NewsDAO.GetNews(vm.SearchMNewsModels, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);


                //result.MNewsModels = new MNewsModels
                //{
                //    ArticlesContent = "<p>เนื้อหาข่าวตัวอย่าง</p>"
                //};

            }


            return View(result);
            #endregion End panging


        }
        [HttpGet]
        public async Task<IActionResult> PreviewNews(string id)
        {
            try
            {
                int NewsId = int.Parse(ServiceCenter.DecodeBase64(id));

                #region panging
                int curpage = 0;
                int totalpage = 0;

                ViewMNewsModels result = new ViewMNewsModels();

                int PageSizeDummy = PageSize;
                MNewsModels param = new MNewsModels();
                param.Id = NewsId; // Fixed variable name from 'id' to 'NewsId'
                param.FlagPage = "SEARCH";
                result = await NewsDAO.GetNews(param, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);

                var newsItem = result.ListTMNewsModels[0];
                if (!string.IsNullOrEmpty(newsItem.FileNameOriginal) && !string.IsNullOrEmpty(newsItem.NewsFilePath))
                {
                    var fileNames = newsItem.FileNameOriginal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var filePaths = newsItem.NewsFilePath.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    // Ensure both arrays have the same length
                    int count = Math.Min(fileNames.Length, filePaths.Length);

                    result.DownloadList = new List<DownloadItem>();
                    for (int i = 0; i < count; i++)
                    {
                        var fileName = fileNames[i].Trim();
                        var filePath = filePaths[i].Trim();

                        result.DownloadList.Add(new DownloadItem
                        {
                            FileName = fileName,
                            FileUrl = filePath, // Use the actual saved path
                            FileSize = GetFileSizeString(filePath),
                            DownloadCount = GetDownloadCount(filePath),
                            FileType = Path.GetExtension(fileName).TrimStart('.').ToUpper() // e.g., PDF, DOCX
                        });
                    }
                }
                else
                {
                    result.DownloadList = new List<DownloadItem>();
                }

                return View(result);
                #endregion End panging
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PreviewNews");
                return RedirectToAction("Index", "Home"); // Ensure a return statement here
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditNews(int id,ViewMNewsModels vm, string saveNews = null)
        {
    
            #region panging
            int curpage = 0;
            int totalpage = 0;

            ViewMNewsModels result = new ViewMNewsModels();

            int PageSizeDummy = PageSize;
            MNewsModels param = new MNewsModels();
            MNewsModels md = new MNewsModels();
            param.Id = id;
          
            if (!string.IsNullOrEmpty(saveNews) )
            {
                var data = vm;
            }
            else 
            {
                result = await NewsDAO.GetNews(param, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);
                //result.DDLCategory = ServiceCenter.GetLookups("GetDropdownCategory", API_Path_Main + API_Path_Sub, null);
                //ViewBag.DDLCategory = new SelectList(result.DDLCategory.DropdownList.OrderBy(x => x.Code), "Code", "Name");
                if (result.ListTMNewsModels.Count > 0)
                {
                    md.ArticlesTitle = result.ListTMNewsModels[0].ArticlesTitle;
                    md.ArticlesContent = result.ListTMNewsModels[0].ArticlesContent;
                    md.StartDate = result.ListTMNewsModels[0].StartDate;
                    md.EndDate = result.ListTMNewsModels[0].EndDate;
                    md.PublishDate = result.ListTMNewsModels[0].PublishDate;
                    md.EndDate = result.ListTMNewsModels[0].EndDate;
                    md.CatagoryCode = result.ListTMNewsModels[0].CatagoryCode;
                    md.IsPin = result.ListTMNewsModels[0].IsPin;
                    md.IsPublished = result.ListTMNewsModels[0].IsPublished;

                    result.MNewsModels = md;
                }
            }
            // dropdown 
            result.DDLpin = ServiceCenter.GetLookups("STATUS", API_Path_Main + API_Path_Sub, null);
            result.DDLpublish = ServiceCenter.GetLookups("STATUS", API_Path_Main + API_Path_Sub, null);

            result.DDLCategory = ServiceCenter.GetLookups("GetDropdownCategory", API_Path_Main + API_Path_Sub, null);
            ViewBag.DDLCategory = new SelectList(result.DDLCategory.DropdownList.OrderBy(x => x.Code), "Code", "Name");
            ViewBag.DDLpin = new SelectList(result.DDLpin.DropdownList.OrderByDescending(x => x.Code), "Code", "Name");
            ViewBag.DDLpublish = new SelectList(result.DDLpublish.DropdownList.OrderByDescending(x => x.Code), "Code", "Name");
            // dropdown 
            return View(result);
            #endregion End panging
        }
        [HttpPost]
        public IActionResult EditNews(ViewMNewsModels model)
        {
            // โค้ดการประมวลผลและบันทึกข้อมูล
            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetNewsData(string id)
        {
       //     var tokenStr = HttpContext.Session.GetString("Token");
            MNewsModels model = new MNewsModels();
            MNewsModels modelResult = new MNewsModels();
            model.Id = int.Parse(id);
            var result = await NewsDAO.GetNews(model, API_Path_Main + API_Path_Sub, "", 0, 0, null);
            if (result != null && result.ListTMNewsModels.Count > 0)
            {
                modelResult.Id = result.ListTMNewsModels[0].Id;
                modelResult.CatagoryCode = result.ListTMNewsModels[0].CatagoryCode;

                modelResult.ArticlesTitle = result.ListTMNewsModels[0].ArticlesTitle;
                modelResult.ArticlesContent = result.ListTMNewsModels[0].ArticlesContent;
                //modelResult.StartDate = result.ListTMNewsModels[0].StartDate;
                //modelResult.EndDate = result.ListTMNewsModels[0].EndDate;
                //modelResult.PublishDate = result.ListTMNewsModels[0].PublishDate;
                modelResult.IsPublished = result.ListTMNewsModels[0].IsPublished;
                modelResult.IsPin = result.ListTMNewsModels[0].IsPin;
                modelResult.EditStartDate = result.ListTMNewsModels[0].StartDate?.ToString("yyyy-MM-dd");
                modelResult.EditEndDate = result.ListTMNewsModels[0].EndDate?.ToString("yyyy-MM-dd");
                modelResult.EditPublishDate = result.ListTMNewsModels[0].PublishDate?.ToString("yyyy-MM-dd");
                modelResult.FileNameOriginal = result.ListTMNewsModels[0].FileNameOriginal;

            }


            return Json(modelResult);
        }


        // POST: /news/edit
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] MNewsModels model)
        {
            // ตรวจสอบข้อมูลที่ส่งมา
            if (model == null)
            {
                return BadRequest("ข้อมูลไม่ถูกต้อง");
            }

            // ตัวอย่างการประมวลผลข้อมูล (แก้ไขข่าวสารในฐานข้อมูล)
            try
            {
                //// สมมติว่าใช้บริการเพื่ออัปเดตข่าวสาร
                //var result = await _newsService.UpdateNewsAsync(model); // ตัวอย่างบริการที่คุณอาจใช้

                //if (result)
                //{
                return Ok(new { message = "แก้ไขข่าวสารสำเร็จ!" });
                //}

                //return StatusCode(500, "ไม่สามารถแก้ไขข่าวสารได้");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        // POST: /news/create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MNewsModels model)
        {
            // ตรวจสอบข้อมูลที่ส่งมา
            if (model == null)
            {
                return BadRequest("ข้อมูลไม่ถูกต้อง");
            }

            // ตัวอย่างการประมวลผลข้อมูล (สร้างข่าวสารใหม่ในฐานข้อมูล)
            try
            {
                //// สมมติว่าใช้บริการเพื่อสร้างข่าวสารใหม่
                //var result = await _newsService.CreateNewsAsync(model); // ตัวอย่างบริการที่คุณอาจใช้

                //if (result)
                //{
                return Ok(new { message = "สร้างข่าวสารสำเร็จ!" });
                //}

                return StatusCode(500, "ไม่สามารถสร้างข่าวสารได้");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<JsonResult> DeleteNews(int id)
        {
            try
            {
                // ลบข้อมูลข่าวสาร
               await NewsDAO.DeleteNewsAsync(id, API_Path_Main + API_Path_Sub, null);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetNewsById(int id)
        {
            MNewsModels model = new MNewsModels();
            model.Id = id;
            model.FlagPage = "SEARCH";
            var news = await NewsDAO.GetNews(model, API_Path_Main + API_Path_Sub, "", 0, 0, null);

            if (news != null && news.ListTMNewsModels.Count > 0)
            {
                var item = news.ListTMNewsModels[0];
                // Split PicNewsFilePath into an array for easier use on the frontend
                var picNewsFileArray = string.IsNullOrEmpty(item.PicNewsFilePath)
                    ? Array.Empty<string>()
                    : item.PicNewsFilePath.Split(',', StringSplitOptions.RemoveEmptyEntries);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = item.Id,
                        articlesTitle = item.ArticlesTitle,
                        articlesShortDescription = item.ArticlesShortDescription,
                        catagoryCode = item.CatagoryCode,
                        businessUnitId = item.BusinessUnitId,
                        publishDate = item.PublishDate?.ToString("yyyy-MM-dd"),
                        startDate = item.StartDate?.ToString("yyyy-MM-dd"),
                        endDate = item.EndDate?.ToString("yyyy-MM-dd"),
                        isPublished = item.IsPublished,
                        isPin = item.IsPin,
                        articlesContent = item.ArticlesContent,
                        coverFilePath = item.CoverFilePath,
                        picNewsFilePath = item.PicNewsFilePath, // keep for backward compatibility
                        picNewsFileList = picNewsFileArray,      // new: array of image paths
                        newsFilePath = item.NewsFilePath,

                        fileNameOriginal = item.FileNameOriginal,

                    }
                });
            }
            return Json(new { success = false, message = "ไม่พบข้อมูลข่าว" });
        }

        private string GetFileSizeString(string fileName)
        {
            // Implement logic to get file size as a string
            return "0 KB"; // Placeholder
        }

        private int GetDownloadCount(string fileName)
        {
            // Implement logic to get download count
            return 0; // Placeholder
        }

        [HttpPost]
        public async Task<JsonResult> ToggleActive(int id, bool isActive)
        {
            try
            {
                // Update the IsPublished status in the database
                 MNewsModels model = new MNewsModels
                {
                    Id = id,
                    IsPublished = isActive
                    ,UpdateBy = HttpContext.Session.GetString("EmployeeId"),
                     FlagPage = "ACTIVE"
                 };
                var result = await NewsDAO.UpdateStatusActiveNews(model, API_Path_Main + API_Path_Sub);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ToggleActivePin(int id, bool isActive)
        {
            try
            {
                // Update the IsPublished status in the database
                MNewsModels model = new MNewsModels
                {
                    Id = id,
                    IsPin = isActive
                   ,
                    UpdateBy = HttpContext.Session.GetString("EmployeeId"),
                    FlagPage ="PIN"
                };
                var result = await NewsDAO.UpdateStatusActiveNews(model, API_Path_Main + API_Path_Sub);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
