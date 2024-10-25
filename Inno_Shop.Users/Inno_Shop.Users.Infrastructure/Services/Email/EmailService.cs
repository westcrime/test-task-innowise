using Inno_Shop.Users.Application.Services.Email;
using Inno_Shop.Users.Domain.Entities;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Inno_Shop.Users.Infrastructure.Services.Email
{
    public class EmailService : IEmailService
    {
        public async Task<string> SendEmailToken(string email)
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("innoshopmail@gmail.com", "crdv dfxx qrqf tpdl"),
                EnableSsl = true
            })
            {
                var token = Guid.NewGuid().ToString();
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("innoshopmail@gmail.com"),
                    Subject = "Verification",
                    Body = $"<h1>Your code is <br> <b>{token}</b></h1>",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
                return token;
            }
        }
    }
}
