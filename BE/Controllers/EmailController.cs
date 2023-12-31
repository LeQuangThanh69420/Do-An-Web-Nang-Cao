using System.Net;
using System.Net.Mail;
using BE.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    public class EmailModel
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
    public class EmailController : BaseApiController
    {
        [NonAction]
        public async Task<ActionResult> SendEmail(EmailModel emailModel)
        {
            try
            {
                // Tạo đối tượng SmtpClient để gửi email
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    //Truy cập https://myaccount.google.com/lesssecureapps để cho phép ứng dụng kém an toàn, cần là tài khoản đuôi khác @gmail.com
                    smtpClient.Credentials = new NetworkCredential("yourEmailAdress", "yourEmailPassword");
                    smtpClient.EnableSsl = true;
                    // Tạo đối tượng MailMessage để cấu hình email
                    MailMessage mail = new MailMessage();
                    mail.IsBodyHtml = true;
                    mail.From = new MailAddress("yourEmailAdress");
                    mail.To.Add(emailModel.To);
                    mail.Subject = emailModel.Subject;
                    mail.Body = emailModel.Body;
                    // Gửi email
                    await smtpClient.SendMailAsync(mail);
                }
                return Ok(new {message = "Email send successfully, please wait and go check your Email!"});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = $"Email send failed! Error: {ex.Message}"});
            }
        }
    }
}