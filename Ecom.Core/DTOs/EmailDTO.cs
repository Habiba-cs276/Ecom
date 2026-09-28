using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.DTOs
{
    public class EmailDTO
    {
        public EmailDTO(string to, string from, string subject, string content)
        {
            this.To = to;
            this.From = from;
            this.Subject = subject;
            this.Content = content;
        }
        public EmailDTO(string to, string subject, string content)
        {
            this.To = to;
            this.Subject = subject;
            this.Content = content;
        }
        public string To { get; set; }
        public string? From { get; set; }    
        public string Subject { get; set; }
        public string Content { get; set; }
       

    }
}
