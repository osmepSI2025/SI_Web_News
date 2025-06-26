using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class MailService
{
    private readonly IConfiguration _configuration;
    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendMailAsync(string to, string subject, string body)
    {
        try
        {
            var mailInfo = _configuration.GetSection("MailInfo");
            string fromEmail = mailInfo["EmailAdminfrom"];
            string fromName = mailInfo["EmailAdminfromName"];
            string password = mailInfo["EmailPassword"];
            string host = mailInfo["Host"];
            int port = int.Parse(mailInfo["Port"]);

            using var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }
        catch
        {
            // Log error here if needed
            return false;
        }
    }
}