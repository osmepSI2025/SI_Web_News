namespace SME_WEB_News.Models
{
    public class PagingModel
    {
        public int? PageSize { get; set; }
        public int? CurrentPageNumber { get; set; }
        public double? TotalPage { get; set; }
    }
}
