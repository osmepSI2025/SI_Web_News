using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SME_WEB_News.DAO;
using SME_WEB_News.Models;
using SME_WEB_News.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public class BannerDisplayController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BannerDisplayController> _logger;
    protected static string API_Path_Main;
    protected static string API_Path_Sub;
    protected static string API_Path_Trigger;
    protected static string API_Path_Sub_Trigger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly CallAPIService _callAPIService;
    protected int currentPageNumber;
    protected static int PageSize;
    protected static int PageSizMedium;
   
    public BannerDisplayController(ILogger<BannerDisplayController> logger, IConfiguration configuration,
      IWebHostEnvironment webHostEnvironment
      , CallAPIService callAPIService)
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

    }
    // In-memory list for demo; replace with your data access logic
    private static List<BannerModels> banners = new List<BannerModels>
    {
        new BannerModels { Id = 1, Title = "Banner 1", Description = "Desc 1", ImageUrl = "/img/banner1.jpg", LinkUrl = "https://example.com", SortOrder = 1, FlagActive = true }
    };
    public async Task<IActionResult> Index(ViewBannerNewsModels vm, string previous, string first, string next, string last, string hidcurrentpage, string hidtotalpage
        , string searchCatNews, string saveCatNews
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
        ViewBannerNewsModels result = new ViewBannerNewsModels();

        if (!string.IsNullOrEmpty(hidcurrentpage)) curpage = Convert.ToInt32(hidcurrentpage);
        if (!string.IsNullOrEmpty(hidtotalpage)) totalpage = Convert.ToInt32(hidtotalpage);
        if (!string.IsNullOrEmpty(first)) currentPageNumber = 1;
        else if (!string.IsNullOrEmpty(previous)) currentPageNumber = (curpage == 1) ? 1 : curpage - 1;
        else if (!string.IsNullOrEmpty(next)) currentPageNumber = (curpage == totalpage) ? totalpage : curpage + 1;
        else if (!string.IsNullOrEmpty(last)) currentPageNumber = totalpage;

        int PageSizeDummy = PageSize;
        int totalCount = 0;
        PageSize = PageSizeDummy;
        #endregion
        if (!string.IsNullOrEmpty(searchCatNews))
        {
            var data = vm;

            if (vm.SearchBannerModels.Id == 0 && vm.SearchBannerModels != null)
            {

                result = BannerNewsDAO.GetBannerNews(vm.SearchBannerModels, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);

            }


        }

        else
        {
            result = BannerNewsDAO.GetBannerNews(vm.SearchBannerModels, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);


        }
        if (result.listBannerModels != null && result.listBannerModels.Count != 0)
        {
            vm.listBannerModels = result.listBannerModels;
            vm.PageModel = ServiceCenter.LoadPagingViewModel(result.TotalRowsList ?? 0, currentPageNumber, PageSize);

        }
        // คำนวณจำนวนหน้า

        result.DDLStatus = ServiceCenter.GetLookups("STATUS", API_Path_Main + API_Path_Sub, null);
        ViewBag.DDLStatus = new SelectList(result.DDLStatus.DropdownList.OrderBy(x => x.Code), "Code", "Name");
        return View(vm);

    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(BannerModels model)
    {
        if (ModelState.IsValid)
        {
            model.Id = banners.Any() ? banners.Max(b => b.Id) + 1 : 1;
            if (model.BannerFile != null)
            {
                model.ImageUrl = "/img/banner-uploaded.jpg";
            }
            banners.Add(model);
            return RedirectToAction("Index");
        }
        return View(model);
    }

    public IActionResult Edit(int id)
    {
        var banner = banners.FirstOrDefault(b => b.Id == id);
        if (banner == null) return NotFound();
        return View(banner);
    }

    [HttpPost]
    public IActionResult Edit(BannerModels model)
    {
        var banner = banners.FirstOrDefault(b => b.Id == model.Id);
        if (banner == null) return NotFound();
        banner.Title = model.Title;
        banner.Description = model.Description;
        banner.LinkUrl = model.LinkUrl;
        banner.SortOrder = model.SortOrder;
        banner.FlagActive = model.FlagActive;
        if (model.BannerFile != null)
        {
            banner.ImageUrl = "/img/banner-uploaded.jpg";
        }
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var banner = banners.FirstOrDefault(b => b.Id == id);
        if (banner == null) return NotFound();
        return View(banner);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var banner = banners.FirstOrDefault(b => b.Id == id);
        if (banner != null) banners.Remove(banner);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public JsonResult GetData(int id)
    {
        try
        {
            BannerModels xdata = new BannerModels
            {
                Id = id,
            };
            var result = BannerNewsDAO.GetBannerNews(xdata, API_Path_Main + API_Path_Sub, "", currentPageNumber, PageSize, null);

            var cat = result.listBannerModels.FirstOrDefault(c => c.Id == id);

            if (cat == null) return Json(new { success = false, data = (object)null });

            return Json(new { success = true, data = cat });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, data = (object)null }); // Explicitly cast null to object
        }
    }

    [HttpPost]
    public async Task<JsonResult> AddOrEdit(BannerModels model)
    {
        if (model.BannerFile != null && model.BannerFile.Length > 0)
        {
            // สร้างชื่อไฟล์ใหม่เพื่อป้องกันชื่อซ้ำ
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.BannerFile.FileName)}";
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "banners");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.BannerFile.CopyToAsync(stream);
            }

            // เก็บ path สำหรับแสดงผล (เช่น /uploads/banners/xxxx.jpg)
            model.ImageUrl = $"/uploads/banners/{fileName}";
        }

        if (model.Id == 0)
        {
            model.CreateDate = DateTime.Now;
            model.CreateBy = "admin";
            var inset = BannerNewsDAO.CreateBanner(model, API_Path_Main + API_Path_Sub, null);
        }
        else
        {
            model.UpdateDate = DateTime.Now;
            model.UpdateBy = "admin";
            var inset = BannerNewsDAO.CreateBanner(model, API_Path_Main + API_Path_Sub, null);
        }
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<JsonResult> DeleteBanner(int id)
    {
        try
        {
     
            bool result = await BannerNewsDAO.DeleteBannerAsync(id, API_Path_Main + API_Path_Sub, null);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}