using System.Collections.Generic;

namespace SME_WEB_News.Models
{
    public class TokenInfoViewModel
    {
        public string AccessToken { get; set; }
        public string IdToken { get; set; }
        public string User { get; set; }
        public List<ClaimViewModel> Claims { get; set; }
    }

    public class ClaimViewModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}