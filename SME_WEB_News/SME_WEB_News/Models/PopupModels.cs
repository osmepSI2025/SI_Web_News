namespace SME_WEB_News.Models
{
    public class PopupModels
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public bool? FlagActive { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string? CreateBy { get; set; }

        public string? UpdateBy { get; set; }

        public int rowOFFSet { get; set; }
        public int rowFetch { get; set; }

        public string? Token { get; set; }
        public IFormFile? PopupFile { get; set; }

        public string? LinkUrl { get; set; }
        public DateTime? PublishDate { get; set; }
    }
    public class ViewPopupModels
    {
        public PopupModels SearchPopupModels { get; set; }
        public PopupModels BannerModesl { get; set; }


        public List<PopupModels> listPopupModels { get; set; }
        public int? TotalRowsList { get; set; }
        public PagingModel PageModel { get; set; }

        public vDropdownDTO DDLStatus { get; set; }


        public vDropdownDTO DDLDepartment { get; set; }

    }
}
