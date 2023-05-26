using CourseProject.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;

namespace CourseProject
{
    /// <summary>
    /// Логика взаимодействия для ChangeMailWindow.xaml
    /// </summary>
    public partial class ChangeMailWindow : Window
    {
        private int AccessCode;
        private int Attempts;
        private Regex mailRegex = new Regex(@"^.+@.+\..+$");
        private DB db = DB.getInstance();
        private string mail;
        public ChangeMailWindow()
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
            if (Attempts < 0)
            {
                MessageBox.Show("You lose all your attempts!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }
            int code;
            if (!int.TryParse(CodeTB.Text, out code))
            {
                MessageBox.Show("Your access code isn't correct!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Attempts--;
                return;
            }
            if (AccessCode != code)
            {
                MessageBox.Show("Your access code isn't correct!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Attempts--;
                return;
            }
            CodeTB.Text = null;
            CodeTB.Visibility = Visibility.Hidden;
            CodeLable.Visibility = Visibility.Hidden;
            AccessCodeBtn.Visibility = Visibility.Hidden;
            ChangeMailLable.Visibility = Visibility.Visible;
            MailTB.Visibility = Visibility.Visible;
            AccessCodeBtn_Copy.Visibility = Visibility.Visible;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AccessCodeBtn_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!mailRegex.IsMatch(MailTB.Text))
            {
                MailTB.Focus();
                MessageBox.Show("Mail isn't correct!\nCheck your mail and try again!", "Attention");
                return;
            }
            if (!IsMail(MailTB.Text))
            {
                MessageBox.Show("this mail isn't exist", "Attention");
                return;
            }
            if (IsMailExist())
            {
                MessageBox.Show("User with such mail exist", "Error");
                return;
            }
            mail = MailTB.Text;
            CodeTB.Visibility = Visibility.Visible;
            CodeLable.Visibility = Visibility.Visible;
            AccessCodeBtn_Copy1.Visibility = Visibility.Visible;
            ChangeMailLable.Visibility = Visibility.Hidden;
            MailTB.Visibility = Visibility.Hidden;
            AccessCodeBtn_Copy.Visibility = Visibility.Hidden;
            Attempts = 3;
            try
            {
                Random r = new Random();
                AccessCode = r.Next(100_000, 1_000_000);
                MailMessage mail = new MailMessageBuilder()
                    .From("mineboyplayyt@gmail.com")
                    .To(MailTB.Text)
                    .Subject("Registration code")
                    .Body($"Your access code: {AccessCode}", Encoding.UTF8)
                    .Build();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("mineboyplayyt@gmail.com", "pzovovnlxxixrxwr"),
                    EnableSsl = true
                };
                smtp.Send(mail);
                MessageBox.Show("message with access code was sent on your new e-mail address\nPlease enter your access code to verify your actions", "Attention", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exc)
            {
                MessageBox.Show(Convert.ToString(exc), "Error");
            }

        }
        public Boolean IsMail(String thisMail) // метод проверки на правильность ввода почты
        {
            try
            {
                MailAddress mail = new MailAddress(thisMail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void AccessCodeBtn_Copy1_Click(object sender, RoutedEventArgs e)
        {
            if (Attempts < 0)
            {
                MessageBox.Show("You lose all your attempts!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }
            int code;
            if (!int.TryParse(CodeTB.Text, out code))
            {
                MessageBox.Show("Your access code isn't correct!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Attempts--;
                return;
            }
            if (AccessCode != code)
            {
                MessageBox.Show("Your access code isn't correct!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Attempts--;
                return;
            }
            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE `client` SET `mail` = @m WHERE `id` = @ci;", db.getConnection());
                command.Parameters.Add("@m", MySqlDbType.VarChar).Value = MailTB.Text;
                command.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.client.Id;

                db.openConnection();
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("your mail was updated");

                }
                else
                    MessageBox.Show("your mail wasn't updated");
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
        public Boolean IsMailExist() // метод проверки существования почты в базе данных
        {
            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `client` WHERE `mail` = @uM", db.getConnection());
            command.Parameters.Add("@uM", MySqlDbType.VarChar).Value = MailTB.Text;

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
                return true;
            else
                return false;
        }
    }
}
