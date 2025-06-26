namespace SME_WEB_News.Models
{
    public partial class MMailModels
    {
        public int Id { get; set; }

        /// <summary>
        /// Complete/Partial/Error
        /// </summary>
        public string? MailType { get; set; }

        public string? MailTo { get; set; }

        public string? MailCc { get; set; }

        public string? MailSubject { get; set; }

        public string? MailDetail { get; set; }

        /// <summary>
        /// วันที่สร้าง
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Userที่สร้าง
        /// </summary>
        public string? CreateBy { get; set; }

        /// <summary>
        /// วันที่อัพเดทล่าสุด
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// Userที่อัพเดทล่าสุด
        /// </summary>
        public string? UpdateBy { get; set; }
    }
    public class MailSettingModels
    {
        public string? EmailAdminfrom { get; set; }
        public string? EmailAdminfromName { get; set; }
        public string? EmailPassword { get; set; }
        public string? Host { get; set; }
        public string? Port { get; set; }
    }
}
