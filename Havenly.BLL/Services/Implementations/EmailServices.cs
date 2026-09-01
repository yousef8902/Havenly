using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Havenly.BLL.Services.Implementations
{
    public class EmailServices:IEmailServices
    {

        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _fromEmail;
        private readonly string _password;

        public EmailServices( string fromEmail, string password,string smtpServer = "smtp.gmail.com", int port= 587)
        {
            _smtpServer = smtpServer;
            _port = port;
            _fromEmail = fromEmail;
            _password = password;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_smtpServer, _port)
                {
                    Credentials = new NetworkCredential(_fromEmail, _password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage(_fromEmail, toEmail, subject, body);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex) { 
            Console.WriteLine(ex.ToString());
            }


        }
        public async Task SendEmailRequestbooking(string toEmail,Booking booking)
        { 
            string subject= "Booking Request";
            string body = $"A new booking request has been submitted for {booking.Listing.Property.PropertyName}. Please review and take necessary action.";
            SendEmailAsync(toEmail, subject, body);



        }

        }

}

