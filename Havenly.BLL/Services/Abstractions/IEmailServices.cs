using Havenly.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IEmailServices
    {
        
        public  Task SendEmailRequestbookingToHost(string toEmail, Booking booking);
   
        public  Task ReciveEmailRequestbookingFromHost(string toEmail, Booking booking);

        public Task SendEmailRequestPropertyToAdmin(string toEmail, Booking booking);
        public  Task ReciveEmailRequestPropertyFromAdmin(string toEmail, Booking booking);




    }
}
