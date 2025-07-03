using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SME_WEB_News.Controllers;
using SME_WEB_News.DAO;
using SME_WEB_News.Models;
using SME_WEB_News.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public class CategoryNewsController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CategoryNewsController> _logger;
    protected static string API_Path_Main;
    protected static string API_Path_Sub;
    protected static string API_Path_Trigger;
    protected static string API_Path_Sub_Trigger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly CallAPIService _callAPIService;
    protected int currentPageNumber;
    protected static int PageSize;
    protected static int PageSizMedium;
    public CategoryNewsController(ILogger<CategoryNewsController> logger, IConfiguration configuration,
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
    private static List<CategoryNewsModels> categories = new List<CategoryNewsModels>
    {
        new CategoryNewsModels { Id = 1, CategorieCode = "GEN", CategorieNameTh = "ทั่วไป", CategorieNameEn = "General", Description = "ข่าวทั่วไป", CreateDate = DateTime.Now, CreateBy = "admin" }
    };

    public async Task< IActionResult> Index(ViewCategoryNewsModels vm, string previous, string first, string next, string last, string hidcurrentpage, string hidtotalpage
        ,string searchCatNews,string saveCatNews
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
        ViewCategoryNewsModels result = new ViewCategoryNewsModels();

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

            if (vm.SearchCategoryNewsModels.Id == 0 && vm.SearchCategoryNewsModels != null)
            {

                result = CategoryNewsDAO.GetCategoryNews(vm.SearchCategoryNewsModels, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);
        
            }


        }
       
        else
        {
            result = CategoryNewsDAO.GetCategoryNews(vm.SearchCategoryNewsModels, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);

           
        }
        if (result.listMCategoryModels != null && result.listMCategoryModels.Count != 0)
        {
            vm.listMCategoryModels = result.listMCategoryModels;
            vm.PageModel = ServiceCenter.LoadPagingViewModel(result.TotalRowsList ?? 0, currentPageNumber, PageSize);

        }
        // คำนวณจำนวนหน้า
   
        result.DDLStatus = ServiceCenter.GetLookups("STATUS", API_Path_Main + API_Path_Sub, null);
        ViewBag.DDLStatus = new SelectList(result.DDLStatus.DropdownList.OrderBy(x => x.Code), "Code", "Name");
        return View(vm);
    }

    [HttpPost]
    public JsonResult AddOrEdit(CategoryNewsModels model)
    {
        if (model.Id == 0)
        {
            // Add
            //   model.Id = categories.Any() ? categories.Max(c => c.Id) + 1 : 1;
            model.CategorieCode = Guid.NewGuid().ToString(); 
            model.CreateDate = DateTime.Now;
            model.CreateBy = HttpContext.Session.GetString("EmployeeId") ?? "admin"; // Use session or default to "admin"

            var inset = CategoryNewsDAO.CreateCategoryNews(model, API_Path_Main + API_Path_Sub, null);
        }
        else
        {
            // Edit
            //var cat = categories.FirstOrDefault(c => c.Id == model.Id);
            //if (cat == null) return Json(new { success = false, message = "Not found" });
       
            model.CategorieNameTh = model.CategorieNameTh;
            model.CategorieNameEn = model.CategorieNameEn;
            model.Description = model.Description;
            model.UpdateDate = DateTime.Now;
            model.UpdateBy = HttpContext.Session.GetString("EmployeeId") ?? "admin";
            var inset = CategoryNewsDAO.CreateCategoryNews(model, API_Path_Main + API_Path_Sub, null);
        }
        return Json(new { success = true });
    }

    [HttpGet]
    public JsonResult GetCategory(int id)
    {
        try
        {
            CategoryNewsModels xdata = new CategoryNewsModels
            {
                Id = id,
            };
            var result = CategoryNewsDAO.GetCategoryNews(xdata, API_Path_Main + API_Path_Sub, "", currentPageNumber, PageSize, null);

            var cat = result.listMCategoryModels.FirstOrDefault(c => c.Id == id);
         
            if (cat == null) return Json(new { success = false, data = (object)null });
         
            return Json(new { success = true, data = cat });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, data = (object)null }); // Explicitly cast null to object
        }
    }

    [HttpPost]
    public JsonResult Delete(int id)
    {
        var cat = categories.FirstOrDefault(c => c.Id == id);
        if (cat != null) categories.Remove(cat);
        return Json(new { success = true });
    }
}
