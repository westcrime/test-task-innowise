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
        public async Task<ResponseModel<string>> SendEmailToken(string email)
        {
            try
            {
                using (var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // или 465 для SSL
                    Credentials = new NetworkCredential("innoshopmail@gmail.com", "12345678qw!"),
                    EnableSsl = true
                })
                {
                    var token = Guid.NewGuid().ToString();
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("innoshopmail@gmail.com"),
                        Subject = "Verification",
                        Body = $"<h1>Your code is <br> <b>{token}<b></h1>",
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(email);

                    await smtpClient.SendMailAsync(mailMessage);

                    return new ResponseModel<string>
                    {
                        Success = true,
                        Data = token,
                        ErrorMessage = string.Empty
                    };
                }
            }
            catch (SmtpException smtpEx)
            {
                // Обработка ошибок SMTP
                return new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = $"SMTP Error: {smtpEx.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = $"General Error: {ex.Message}"
                };
            }
        }
    }
}
