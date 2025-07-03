using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SME_WEB_News.DAO;
using SME_WEB_News.Models;
using SME_WEB_News.Services;
using System.Text.Json;

namespace SME_WEB_News.Controllers
{
    public class PopupController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PopupController> _logger;
        protected static string API_Path_Main;
        protected static string API_Path_Sub;
        protected static string API_Path_Trigger;
        protected static string API_Path_Sub_Trigger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly CallAPIService _callAPIService;
        protected int currentPageNumber;
        protected static int PageSize;
        protected static int PageSizMedium;
        public PopupController(ILogger<PopupController> logger, IConfiguration configuration,
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
        public async Task<IActionResult> Index(ViewPopupModels vm, string previous, string first, string next, string last, string hidcurrentpage, string hidtotalpage

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
            ViewPopupModels result = new ViewPopupModels();

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

                if (vm.SearchPopupModels.Id == 0 && vm.SearchPopupModels != null)
                {

                    result = PopupDAO.GetPopup(vm.SearchPopupModels, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);

                }


            }

            else
            {
                result = PopupDAO.GetPopup(vm.SearchPopupModels, API_Path_Main + API_Path_Sub, "Y", currentPageNumber, PageSize, null);


            }
            if (result.listPopupModels != null && result.listPopupModels.Count != 0)
            {
                vm.listPopupModels = result.listPopupModels;
                vm.PageModel = ServiceCenter.LoadPagingViewModel(result.TotalRowsList ?? 0, currentPageNumber, PageSize);

            }
            // คำนวณจำนวนหน้า

            result.DDLStatus = ServiceCenter.GetLookups("STATUS", API_Path_Main + API_Path_Sub, null);
            ViewBag.DDLStatus = new SelectList(result.DDLStatus.DropdownList.OrderBy(x => x.Code), "Code", "Name");
            return View(vm);

        }

        [HttpGet]
        public JsonResult GetData(int id)
        {
            try
            {
                PopupModels xdata = new PopupModels
                {
                    Id = id,
                };
                var result = PopupDAO.GetPopup(xdata, API_Path_Main + API_Path_Sub, "", currentPageNumber, PageSize, null);

                var cat = result.listPopupModels.FirstOrDefault(c => c.Id == id);

                if (cat == null) return Json(new { success = false, data = (object)null });

                return Json(new { success = true, data = cat });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = (object)null }); // Explicitly cast null to object
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddOrEdit(PopupModels model)
        {
            if (model.PopupFile != null && model.PopupFile.Length > 0)
            {
                // สร้างชื่อไฟล์ใหม่เพื่อป้องกันชื่อซ้ำ
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.PopupFile.FileName)}";
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "Popup");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PopupFile.CopyToAsync(stream);
                }

                // เก็บ path สำหรับแสดงผล (เช่น /uploads/banners/xxxx.jpg)
                model.ImageUrl = $"/uploads/Popup/{fileName}";
            }

            if (model.Id == 0)
            {
                model.CreateDate = DateTime.Now;
                model.CreateBy = HttpContext.Session.GetString("EmployeeId") ?? "admin";
                model.PopupFile = null;
                var inset = PopupDAO.CreatePopup(model, API_Path_Main + API_Path_Sub, null);
            }
            else
            {
                model.UpdateDate = DateTime.Now;
                model.UpdateBy = HttpContext.Session.GetString("EmployeeId") ?? "admin";
                model.PopupFile = null;
                var inset = PopupDAO.CreatePopup(model, API_Path_Main + API_Path_Sub, null);
            }
            return Json(new
            {
                success = true,
                data = new
                {
                    title = model.Title,
                    fileName = model.PopupFile,
                    fileUrl = model.ImageUrl,
                    startDateTime = model.StartDateTime,
                    endDateTime =model.EndDateTime,
                    flagActive = model.FlagActive
                }
            });
        }

        [HttpPost]
        public async Task<JsonResult> DeletePopup(int id)
        {
            // ลบข้อมูลจากฐานข้อมูลหรือ DAO
            // สมมติ EmployeeDAO.DeleteUserById(id);
            bool result = await PopupDAO.DeletePopupAsync(id, API_Path_Main + API_Path_Sub, null);
            if (result)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "ไม่พบผู้ใช้งานหรือเกิดข้อผิดพลาด" });
        }
        [HttpPost]
        public async Task<JsonResult> ToggleActive(int id, bool isActive)
        {
            try
            {
                // Update the IsPublished status in the database
                PopupModels model = new PopupModels
                {
                    Id = id,
                    FlagActive = isActive
                   ,
                    UpdateBy = HttpContext.Session.GetString("EmployeeId")

                };
                var result = await PopupDAO.UpdateStatusActivePopup(model, API_Path_Main + API_Path_Sub);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

}
