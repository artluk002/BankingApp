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
using Org.BouncyCastle.Asn1.Crmf;
using System.Threading;

namespace CourseProject
{
    /// <summary>
    /// Логика взаимодействия для RecoveryWindow.xaml
    /// </summary>
    public partial class RecoveryWindow : Window
    {
        private MainWindow backUp;
        private Client client;

        private Regex mailRegex = new Regex(@"^.+@.+\..+$");
        private Regex passwordRegex = new Regex(@"^.{8,20}$");
        
        private int code;
        private int Attempts;
        private DB db;
        public RecoveryWindow()
        {
            InitializeComponent();
        }
        public RecoveryWindow(MainWindow backUp)
        {
            InitializeComponent();
            this.backUp = backUp;
            db = DB.getInstance();
            Client.delInstance();
        }

        private void ConfirmBtn1_Click(object sender, RoutedEventArgs e)
        {
            if(!mailRegex.IsMatch(MailTextBox.Text))
            {
                MessageBox.Show("Your mail isn't correct! pls Check it and try again", "Attention");
                return;
            }

            if(!IsMailExist())
            {
                MessageBox.Show("We can't find a client with this email\nPlease check your email and try again", "Attention");
                return;
            }


            if (!SendCodeMessage(MailTextBox.Text))
                return;
            Step1Canvas.Visibility = Visibility.Hidden;
            Step2Canvas.Visibility = Visibility.Visible;
            Attempts = 0;
        }

        private void ConfirmBtn2_Click(object sender, RoutedEventArgs e)
        {
            if (Attempts > 4)
            {
                MessageBox.Show("You lost all attempts 😥\nPlease try to registered again!", "Error");
                Step2Canvas.Visibility = Visibility.Hidden;
                Step1Canvas.Visibility = Visibility.Visible;
                return;
            }
            if (Convert.ToInt32(AccessCodeTextBox.Text) != code)
            {
                if (Attempts == 3)
                {
                    Attempts++;
                    MessageBox.Show("It's your last attempt! 😂", "Last Chance");
                    return;
                }
                Attempts++;
                MessageBox.Show("Your access code isn't correct! Please check your mail and write correct access code!", "Attention");
                return;
            }
            else
            {
                UserLoginLable.Content = client.Login;
                Step2Canvas.Visibility = Visibility.Hidden;
                Step3Canvas.Visibility = Visibility.Visible;
            }
        }

        private void ConfirmBtn3_Click(object sender, RoutedEventArgs e)
        {
            if (!passwordRegex.IsMatch(PasswordTextBox.Password))
            {
                PasswordTextBox.Focus();
                MessageBox.Show("Password isn't correct!\nPassword must be more than 8 and less than 20 characters!", "Attention");
                return;
            }
            if (!PasswordTextBox.Password.Equals(RepitPasswordTextBox.Password))
            {
                RepitPasswordTextBox.Focus();
                MessageBox.Show("The passwords don't match!", "Attention");
                return;
            }

            client.updatePassword(PasswordTextBox.Password);

            this.Close();
            backUp.Show();
        }
        private bool SendCodeMessage(string Mail) // метод отправляющий сообщение с кодом подтверждения на почту пользователя
        {
            try
            {
                Random r = new Random();
                code = r.Next(100_000, 1_000_000);
                MailMessage mail = new MailMessageBuilder()
                    .From("mineboyplayyt@gmail.com")
                    .To(Mail)
                    .Subject("Recovery code")
                    .Body($"Your access code: {code}", Encoding.UTF8)
                    .Build();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("mineboyplayyt@gmail.com", "pzovovnlxxixrxwr"),
                    EnableSsl = true
                };
                smtp.Send(mail);
                MessageBox.Show("message with access code was sent on your e-mail address");
                return true;
            }
            catch (Exception exc)
            {
                MessageBox.Show(Convert.ToString(exc), "Error");
                return false;
            }
        }

        public Boolean IsMailExist() // метод проверки существования почты в базе данных
        {
            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `client` WHERE `mail` = @uM", db.getConnection());
            command.Parameters.Add("@uM", MySqlDbType.VarChar).Value = MailTextBox.Text;

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                DataRow[] dr = table.Select();
                client = Client.getInstance(Convert.ToUInt32(dr[0][0]), dr[0][1].ToString(), dr[0][2].ToString(), dr[0][3].ToString(), dr[0][4].ToString(), dr[0][5].ToString(), dr[0][6].ToString(), dr[0][7].ToString());
                return true;
            }
            else
                return false;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            backUp.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.languageCode = "en-US";
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            RecoveryWindow newWin = new RecoveryWindow(backUp);
            newWin.Show();
            this.Close();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.languageCode = "ru-RU";
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");
            RecoveryWindow newWin = new RecoveryWindow(backUp);
            newWin.Show();
            this.Close();
        }
    }
}
