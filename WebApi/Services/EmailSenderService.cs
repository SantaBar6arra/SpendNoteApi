using System.Net.Mail;
using System.Net;
using WebApi.Constants;

namespace WebApi.Services
{
    public class EmailSenderService
    {
        private readonly MailAddress _fromMailAddress;
        private readonly IConfiguration _configuration;

        private const string _subject = "Verification code for Spend-Note app";
        private const string _bodyPart = "Your refresh password code: ";

        public EmailSenderService()
        {
            _configuration = new ConfigurationBuilder().
                AddJsonFile(ConfigurationConstants.ConfigurationFileName).Build();

            _fromMailAddress = new(
                _configuration[ConfigurationConstants.EmailSettings_Address]);
        }

        public void SendEmail(string emailAddress, string guid)
        {
            MailAddress toAddress = new(emailAddress);
            string body = _bodyPart + guid;

            SmtpClient smtpClient = new ()
            {
                Host = _configuration[ConfigurationConstants.EmailSettings_SmtpHost],
                Port = Convert.ToInt32(_configuration[ConfigurationConstants.EmailSettings_SmtpPort]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    _fromMailAddress.Address,
                    _configuration[ConfigurationConstants.EmailSettings_AppPassword])
            };

            using (var message = new MailMessage(_fromMailAddress, toAddress)
            {
                Subject = _subject,
                Body = body
            })
            {
                smtpClient.Send(message);
            }
        }
    }
}
