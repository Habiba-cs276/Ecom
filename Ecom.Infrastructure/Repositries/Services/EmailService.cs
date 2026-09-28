using Ecom.Core.DTOs;
using Ecom.Core.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositries.Services
{
    public class EmailService : IEmailService
    {
        // usinggggg SMTP to verify the Email 
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(EmailDTO emailDTO)
        {
           MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("My ECOM", _configuration["EmailSetting:From"]));

            message.To.Add(new MailboxAddress(emailDTO.To, emailDTO.To));
            message.Subject = emailDTO.Subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailDTO.Content
            };

            // making thhee connection with SMTP server 
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(
                        _configuration["EmailSetting:Smtp"],
                        int.Parse(_configuration["EmailSetting:Port"]),
                        true);

                    await smtp.AuthenticateAsync(
                        _configuration["EmailSetting:UserName"],
                        _configuration["EmailSetting:Password"]
                        );

                    await smtp.SendAsync(message);
                }
                catch(Exception ex)
                {

                }
                finally
                {
                  await  smtp.DisconnectAsync(true);
                         smtp.Dispose();
                }
               
            }

        }

    }
}
