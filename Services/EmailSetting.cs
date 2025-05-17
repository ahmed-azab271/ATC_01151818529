using Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.IServices;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class EmailSettings : IEmailSetting
    {
        private readonly IConfiguration configuration;

        public EmailSettings(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public void SendEmail(Email email)
        {
            var Client = new SmtpClient("smtp.gmail.com", 587);
            Client.EnableSsl = true;
            Client.Credentials = new NetworkCredential(configuration["EmailSettings: From"]
                                                       ,configuration["EmailSettings: Password "]);

            Client.Send(configuration["EmailSettings: From"], email.To, email.Subject, email.Body);
        }
    }
}
