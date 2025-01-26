namespace WorkshopManager.Web.Models
{
    public class SmtpSettings
    {
        public bool isActive { get; set; } = false;
        public string host { get; set; } = "";
        public int port { get; set; }
        public string email { get; set; } = "";
        public string password { get; set; } = "";

    }
}
