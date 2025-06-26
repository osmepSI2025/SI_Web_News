namespace SME_WEB_News.Models
{

    public class BannerModels
    {
        public int Id { get; set; }

        public string? Title { get; set; } 

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public string? LinkUrl { get; set; }
        public DateTime? StartDateTime { get; set; }

        public DateTime? PublishDate { get; set; }

        public DateTime? EndDateTime { get; set; }

        public bool? FlagActive { get; set; }

        public int? SortOrder { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; }

        public string? UpdateBy { get; set; }
        public int rowOFFSet { get; set; }
        public int rowFetch { get; set; }

        public string? Token { get; set; }
        public IFormFile? BannerFile { get; set; }

    }
    public class ViewBannerNewsModels
    {
        public BannerModels SearchBannerModels { get; set; }
        public BannerModels BannerModesl { get; set; }


        public List<BannerModels> listBannerModels { get; set; }
        public int? TotalRowsList { get; set; }
        public PagingModel PageModel { get; set; }

        public vDropdownDTO DDLStatus { get; set; }


        public vDropdownDTO DDLDepartment { get; set; }

    }
}
