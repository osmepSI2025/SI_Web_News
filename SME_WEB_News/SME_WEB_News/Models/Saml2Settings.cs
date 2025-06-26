namespace SME_WEB_News.Models
{
    public class Saml2Settings
    {
        public string EntityId { get; set; }
        public string IdpEntityId { get; set; }
        public string SingleSignOnServiceUrl { get; set; }
        public string SingleLogoutServiceUrl { get; set; }
        public string SigningCertificate { get; set; }
        public string CertificatePath { get; set; }
    }

}
