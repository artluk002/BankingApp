using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Classes
{
    internal class MailMessageBuilder
    {
        private MailMessage _mailMessage = new MailMessage(); // создание экземпляра 
        public MailMessageBuilder From(String addres) // метод создания сообщения
        {
            _mailMessage.From = new MailAddress(addres);
            return this;
        }
        public MailMessageBuilder To(string addres) // метод создания сообщения
        {
            _mailMessage.To.Add(addres);
            return this;
        }
        public MailMessageBuilder Cc(string addres) // метод создания сообщения
        {
            _mailMessage.CC.Add(addres);
            return this;
        }
        public MailMessageBuilder Subject(string subject) // метод создания сообщения
        {
            _mailMessage.Subject = subject;
            return this;
        }
        public MailMessageBuilder Body(string body, Encoding encoding) // метод создания сообщения
        {
            _mailMessage.Body = body;
            _mailMessage.BodyEncoding = encoding;
            return this;
        }
        public MailMessage Build() // метод возвращающий экземпляр класса
        {
            return _mailMessage;
        }
    }
}
