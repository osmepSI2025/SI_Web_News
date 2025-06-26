using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SME_WEB_News.DAO;
using SME_WEB_News.Models;
using SME_WEB_News.Services;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace SME_WEB_News.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserManagementController> _logger;
        protected static string API_Path_Main;
        protected static string API_Path_Sub;
        protected static string API_Path_Trigger;
        protected static string API_Path_Sub_Trigger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly CallAPIService _callAPIService;
        protected int currentPageNumber;
        protected static int PageSize;
        protected static int PageSizMedium;
        public UserManagementController(ILogger<UserManagementController> logger, IConfiguration configuration,
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
        // For demonstration, use static lists. Replace with your DAO or database logic.
    

        public async Task<IActionResult> Index()
        {
            ViewBag.EmpDetail = HttpContext.Session.GetString("EmpDetail");
            var empDetailJson = HttpContext.Session.GetString("EmpDetail");

            if (!string.IsNullOrEmpty(empDetailJson))
            {
                var empDetailObj = JsonSerializer.Deserialize<EmployeeRoleModels>(empDetailJson);
                if (empDetailObj == null || string.IsNullOrEmpty(empDetailObj.RoleCode))
                {
                    if (empDetailObj.RoleCode==null) 
                    {
                        return RedirectToAction("Index", "Home");
                    }
                   
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ViewEmployeeRoleModels();
            try
            {
                var EmpRolelist = EmployeeDAO.GetEmpAllRole(API_Path_Main + API_Path_Sub, null);

                if (EmpRolelist == null || !EmpRolelist.Any())
                {
                    _logger.LogWarning("No employee roles found.");
                    return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
                }
                else
                {
                    model.AdminUsers = EmpRolelist.Where(u => u.EmployeeRole == "ADMIN").ToList();
                    model.SuperAdminUsers = EmpRolelist.Where(u => u.EmployeeRole == "SUPERADMIN").ToList();
                }
                //searchEmployeeRoleModels modelSearch = new searchEmployeeRoleModels
                //{
                //    rowOFFSet = 0,
                //    rowFetch = PageSize
                //};
                //var EmpRolelistPopup = await EmployeeDAO.GetEmpByBu(modelSearch, API_Path_Main + API_Path_Sub, null);
                //model.listEmployeeRoleModels = EmpRolelistPopup;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the User Management page.");
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }

        //[HttpPost]
        //public JsonResult AddOrEdit(UserManagementModels user)
        //{
        //    if (user.Id == 0)
        //    {
        //        user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
        //        _users.Add(user);
        //    }
        //    else
        //    {
        //        var existing = _users.FirstOrDefault(u => u.Id == user.Id);
        //        if (existing != null)
        //        {
        //            existing.FullName = user.FullName;
        //            existing.Position = user.Position;
        //            existing.Section = user.Section;
        //            existing.Department = user.Department;
        //            existing.Role = user.Role;
        //        }
        //    }
        //    return Json(new { success = true });
        //}

        [HttpPost]
        public async Task<JsonResult> DeleteUser(int id)
        {
            // ลบข้อมูลจากฐานข้อมูลหรือ DAO
            // สมมติ EmployeeDAO.DeleteUserById(id);
            bool result = await EmployeeDAO.DeleteUserById(id, API_Path_Main + API_Path_Sub, null);
            if (result)
                return Json(new { success = true });
            else
                return Json(new { success = false, message = "ไม่พบผู้ใช้งานหรือเกิดข้อผิดพลาด" });
        }


        [HttpPost]
        public async Task<IActionResult> GetAllUserPopup(searchEmployeeRoleModels model)
        {
            var vm = new ViewEmployeeRoleModels();
            var serviceCenter = new ServiceCenter(_configuration, _callAPIService);
            vm.DDLDepartment = await serviceCenter.GetDdlDepartment(API_Path_Main + API_Path_Sub, "business-units");
            ViewBag.DDLDepartment = new SelectList(vm.DDLDepartment.DropdownList.OrderBy(x => x.Code), "Code", "Name");
        
            try
            {
                var EmpRolelist = await EmployeeDAO.GetEmpByBu(model, API_Path_Main + API_Path_Sub, null);
                vm=EmpRolelist;
                return PartialView("_UserSelectModal", vm);
           //     return PartialView("_TestInsertUserModal", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users.");
                return Json(new { success = false, message = "An error occurred while retrieving users." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> insertUserPopup([FromBody] List<EmployeeRoleModels> model)
        {
            var vm = new ViewEmployeeRoleModels();
            try
            {
                if (model != null && model.Count > 0)
                {
                    foreach (var emp in model)
                    {
                        await EmployeeDAO.InsrtRoleEmpByBu(emp, API_Path_Main + API_Path_Sub, null);
                    }
                }
                // โหลดข้อมูลใหม่หลัง insert ถ้าต้องการ
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while inserting users.");
                return Json(new { success = false, message = "An error occurred while inserting users." });
            }
        }
    }
}
