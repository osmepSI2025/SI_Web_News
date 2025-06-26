
using System.Text.Json.Serialization;

namespace SME_WEB_News.Models
{


    public class ViewEmployeeRoleModels
    {
        public searchEmployeeRoleModels SearchEmployeeRoleModels { get; set; }
        public EmployeeRoleModels EmployeeRoleModels { get; set; }


        public List<EmployeeRoleModels> listEmployeeRoleModels { get; set; }

        public List<EmployeeRoleModels> AdminUsers { get; set; }
        public List<EmployeeRoleModels> SuperAdminUsers { get; set; }
        public int? TotalRowsList { get; set; }
        public PagingModel PageModel { get; set; }

        public vDropdownDTO DDLStatus { get; set; }


        public vDropdownDTO DDLDepartment { get; set; }



    }
    public class EmployeeRoleModels
    {
        public int Id { get; set; }

        public string? EmployeeId { get; set; }
        public string? NameTh { get; set; }

        public string? FirstNameTh { get; set; }

        public string? LastNameTh { get; set; }

        public string? LastNameEn { get; set; }
        public string? BusinessUnitId { get; set; }
        public string? BusinessUnitName { get; set; }
        public string? PositionId { get; set; }
        public string? PositionName { get; set; }
        public string? EmployeeRole { get; set; }
        public string? RoleCode { get; set; }
    }
    public class ApiListEmployeeRoleResponse
    {
        [JsonPropertyName("results")]
        public List<EmployeeResult> Results { get; set; }
    }

    public class searchEmployeeRoleModels
    {

        public string? EmployeeId { get; set; }

        public string? BusinessUnitId { get; set; }

        public string? EmployeeRole { get; set; }

        public string? EmployeeName { get; set; }
        public int rowOFFSet { get; set; }
        public int rowFetch { get; set; }
    }

    public class ListEmployeeRoleModels
    {
       public List<EmployeeRoleModels> Results { get; set; }
    }

}
