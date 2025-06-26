using System.Text.Json.Serialization;

namespace SME_WEB_News.Models
{
    public class MBusinessUnitModels
    {
        public int Id { get; set; }

        public string? BusinessUnitId { get; set; }

        public string? BusinessUnitCode { get; set; }

        public int? BusinessUnitLevel { get; set; }

        public string? ParentId { get; set; }

        public string? CompanyId { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public string? NameTh { get; set; }

        public string? NameEn { get; set; }

        public string? AbbreviationTh { get; set; }

        public string? AbbreviationEn { get; set; }

        public string? DescriptionTh { get; set; }

        public string? DescriptionEn { get; set; }
    }
    public class ApiBusinessUnitResponse
    {
        [JsonPropertyName("results")]
        public MBusinessUnitModels? Results { get; set; }
    }
    public class ApiListBusinessUnitResponse
    {
        [JsonPropertyName("results")]
        public List<MBusinessUnitModels?> Results { get; set; }
    }

}
