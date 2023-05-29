using CourseProject.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Data;

namespace CourseProject
{
    /// <summary>
    /// Логика взаимодействия для ChangePhoneWindow.xaml
    /// </summary>
    public partial class ChangePhoneWindow : Window
    {
        private int AccessCode;
        private int Attempts;
        private Regex phoneRegex = new Regex(@"^(\+375|80)(17|25|29|33|44)(\d{7})$");
        private DB db = DB.getInstance();
        public ChangePhoneWindow()
        {
            InitializeComponent();
            Attempts = 3;
            try
            {
                Random r = new Random();
                AccessCode = r.Next(0, 1_000_000);
                MailMessage mail = new MailMessageBuilder()
                    .From("mineboyplayyt@gmail.com")
                    .To(DataStorage.client.Mail)
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

        private void AccessCodeBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Attempts < 0)
            {
                MessageBox.Show("You lose all your attempts!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }
            int code;
            if(!int.TryParse(CodeTB.Text, out code))
            {
                MessageBox.Show("Your access code isn't correct!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Attempts--;
                return;
            }
            if(AccessCode != code)
            {
                MessageBox.Show("Your access code isn't correct!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Attempts--;
                return;
            }
            CodeTB.Visibility = Visibility.Hidden;
            CodeLable.Visibility = Visibility.Hidden;
            AccessCodeBtn.Visibility = Visibility.Hidden;
            PhoneLable.Visibility = Visibility.Visible;
            PhoneTB.Visibility= Visibility.Visible;
            ChangePhoneNumberBtn.Visibility= Visibility.Visible;
        }

        private void ChangePhoneNumberBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!phoneRegex.IsMatch(PhoneTB.Text))
            {
                PhoneTB.Focus();
                MessageBox.Show("Phone number isn't correct!\nCheck your phone number and try again!", "Attention");
                return;
            }

            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE `client` SET `phone` = @pn WHERE `id` = @ci;", db.getConnection());
                command.Parameters.Add("@pn", MySqlDbType.VarChar).Value = PhoneTB.Text;
                command.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.client.Id;

                db.openConnection();
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("your phone number was updated");
                    
                }
                else
                    MessageBox.Show("your phone number wasn't updated");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something wrong!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                db.closeConnection();
                Close();
            }
            
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
