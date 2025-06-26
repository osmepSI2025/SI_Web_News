using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME_WEB_News.Models;
using System.Linq;
using System.Text.Json;

[Authorize] // บังคับให้ต้อง Authen ก่อนเข้าถึง Controller นี้
public class AdminController : Controller
{
    public IActionResult Index()
    {
        
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
            ViewBag.EmpDetail =empDetailObj;
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    // ถ้าต้องการเฉพาะบาง action ให้ใช้ [Authorize] เฉพาะ action นั้น
    [Authorize]
    public IActionResult ManageNews()
    {
        ViewBag.EmpDetail = HttpContext.Session.GetString("EmpDetail");
        // โค้ดสำหรับจัดการข่าว
        return View();
    }
}