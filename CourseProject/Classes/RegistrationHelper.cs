using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CourseProject.Classes
{
    public class RegistrationHelper
    {
        private string Login;
        private string Name;
        private string Surname;
        private string Gender;
        private string Mail;
        private string Phone;
        private string Password;

        private int AccessCode;

        public RegistrationHelper(string login, string name, string surname, string gender, string mail, string phone, string password)
        {
            Login = login;
            Name = name;
            Surname = surname;
            Gender = gender;
            Mail = mail;
            Phone = phone;
            Password = password;
        }

        public string getLogin() => Login;
        public string getName() => Name;
        public string getSurname() => Surname;
        public string getGender() => Gender;
        public string getMail() => Mail;
        public string getPhone() => Phone;
        public string getPassword()
        {
            string firstLog = Login.Substring(0, Login.Length / 2);
            string secondLog = Login.Substring(Login.Length / 2, Login.Length / 2);
            string solidPass = firstLog + Password + secondLog;
            return solidPass.GetHashCode().ToString();
        }
        public void SendCodeMessage() // метод отправляющий сообщение с кодом подтверждения на почту пользователя
        {
            try
            {
                Random r = new Random();
                AccessCode = r.Next(0, 1_000_000);
                MailMessage mail = new MailMessageBuilder()
                    .From("mineboyplayyt@gmail.com")
                    .To(Mail)
                    .Subject("Registration code")
                    .Body($"Your access code: {AccessCode}", Encoding.UTF8)
                    .Build();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("mineboyplayyt@gmail.com", "pzovovnlxxixrxwr"),
                    EnableSsl = true
                };
                smtp.Send(mail);
                MessageBox.Show("message with access code was sent on your e-mail address");
            }
            catch (Exception exc)
            {
                MessageBox.Show(Convert.ToString(exc), "Error");
            }
        }
        public bool VerifyMail(int code)
        {
            if (code == AccessCode)
                return true;
            else
                return false;
        }
    }
}
