using System.ComponentModel.DataAnnotations;

namespace SME_WEB_News.Models
{
    public partial class MNewsModels
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "กรุณากรอกหัวข้อข่าว")]
        public string? ArticlesTitle { get; set; }
        [Required(ErrorMessage = "กรุณากรอกเนื้อหาข่าว")]
        public string? ArticlesContent { get; set; }
        public string? ArticlesShortDescription { get; set; }
        public string? ArticlesAutherName { get; set; }
        [Required(ErrorMessage = "กรุณาเลือกประเภทข่าว")]
        public string? CatagoryCode { get; set; }
        [Required(ErrorMessage = "กรุณาเลือกสถานะปักหมุด")]
        public bool IsPin { get; set; }

        [Required(ErrorMessage = "กรุณาเลือกวันที่เผยแพร่")]
        public DateTime? PublishDate { get; set; }


        [Required(ErrorMessage = "กรุณาเลือกสถานะเผยแพร่")]
        public bool IsPublished { get; set; }
        [Required(ErrorMessage = "กรุณาเลือกวันที่เริ่มต้น")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "กรุณาเลือกวันที่สิ้นสุด")]
        public DateTime? EndDate { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; }

        public string? UpdateBy { get; set; }
        public int rowOFFSet { get; set; }
        public int rowFetch { get; set; }

        public string? Token { get; set; }
        public string? CatagoryName { get; set; }

        public string? EditStartDate { get; set; }
        public string? EditEndDate { get; set; }
        public string? EditPublishDate { get; set; }
        public string? BusinessUnitId { get; set; }

        public IFormFile? CoverFile { get; set; }
        public IFormFile? PicNewsFile { get; set; }
        public List<IFormFile> NewsFile { get; set; }
        public string? CoverFilePath { get; set; }
        public string? PicNewsFilePath { get; set; }
        public string? NewsFilePath { get; set; }
        public int? OrderId { get; set; }

        public string? FileNameOriginal { get; set; }
        public string? FlagPage { get; set; }
    }
    public class ViewMNewsModels
    {
        public MNewsModels SearchMNewsModels { get; set; } = new MNewsModels();
        public MNewsModels MNewsModels { get; set; } = new MNewsModels();


        public List<MNewsModels> ListTMNewsModels { get; set; } = new List<MNewsModels>();
        public List<MNewsModels> ListUpdateRangingNewsModels { get; set; } = new List<MNewsModels>();
        public int? TotalRowsList { get; set; }
        public PagingModel PageModel { get; set; }


        public vDropdownDTO DDLCategory { get; set; }
        public vDropdownDTO DDLpin { get; set; }
        public vDropdownDTO DDLpublish { get; set; }

        public vDropdownDTO DDLDepartment { get; set; }
        public List<DownloadItem> DownloadList { get; set; }
    }
    public class DownloadItem
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileSize { get; set; }
        public int DownloadCount { get; set; }
        public string FileType { get; set; }
    }
}
