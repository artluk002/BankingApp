using CourseProject.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace CourseProject
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private MainWindow backUp;
        private Regex phoneRegex = new Regex(@"^(\+375|80)(17|25|29|33|44)(\d{7})$");
        private Regex loginRegex = new Regex(@"^(\w{4,20})$");
        private Regex passwordRegex = new Regex(@"^.{8,20}$");
        private Regex mailRegex = new Regex(@"^.+@.+\..+$");
        private RegistrationHelper RH;
        private int num;
        private int Attempts;
        private DB db;
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        public RegistrationWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            backUp= mainWindow;
            db = DB.getInstance();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            backUp.Show();
        }

        private void RegistrationBtn_Click(object sender, RoutedEventArgs e)
        {
            Attempts = 0;
            int errCount = 0;
            if(!loginRegex.IsMatch(LoginTextBox.Text))
            {
                LoginTextBox.Focus();
                MessageBox.Show("Login isn't correct!\nLogin must be more than 4 and less than 20 characters!", "Attention");
                errCount++;
            }
            if(!mailRegex.IsMatch(MailTextBox.Text))
            {
                MailTextBox.Focus();
                MessageBox.Show("Mail isn't correct!\nCheck your mail and try again!", "Attention");
                errCount++;
            }
            if(!IsMail(MailTextBox.Text))
            {
                MessageBox.Show("this mail isn't exist", "Attention");
                errCount++;
            }
            if(!phoneRegex.IsMatch(PhoneTextBox.Text))
            {
                PhoneTextBox.Focus();
                MessageBox.Show("Phone number isn't correct!\nCheck your phone number and try again!", "Attention");
                errCount++;
            }
            if (!passwordRegex.IsMatch(PasswordTextBox.Password))
            {
                PasswordTextBox.Focus();
                MessageBox.Show("Password isn't correct!\nPassword must be more than 8 and less than 20 characters!", "Attention");
                errCount++;
            }
            if (!PasswordTextBox.Password.Equals(RepitPasswordTextBox.Password))
            {
                RepitPasswordTextBox.Focus();
                MessageBox.Show("The passwords don't match!", "Attention");
                errCount++;
            }
            if(MaleRB.IsChecked == false && FemaleRB.IsChecked == false)
            {
                GenderLable.Focus();
                MessageBox.Show("You must to choose your gender!", "Attention");
                errCount++;
            }
            if (IsUserExist())
            {
                MessageBox.Show("user with such login is exist!", "Change Login and try again!");
                errCount++;
            }
            if(IsMailExist())
            {
                MessageBox.Show("User with such mail exist", "Error");
                errCount++;
            }
            if (errCount > 0)
                return;
            string gender;
            if (MaleRB.IsChecked == true)
                gender = "male";
            else gender = "female";
            RH = new RegistrationHelper(LoginTextBox.Text, FirstNameTextBox.Text, SurNameTextBox.Text, gender, MailTextBox.Text, PhoneTextBox.Text, PasswordTextBox.Password);
            RegirstrationCanvas.Visibility= Visibility.Hidden;
            ConfirmCanvas.Visibility= Visibility.Visible;
            RH.SendCodeMessage();

        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Attempts > 4)
            {
                MessageBox.Show("You lost all attempts 😥\nPlease try to registered again!", "Error");
                return;
            }
            if (!RH.VerifyMail(Convert.ToInt32(AccessCodeTextBox.Text)))
            { 
                if(Attempts == 3)
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
                try
                {
                    MySqlCommand command = new MySqlCommand("INSERT INTO `client` (`login`, `name`, `surname`, `gender`, `mail`, `phone`, `hash_password`) VALUES (@cL, @cN, @cSn, @cG, @cM, @cPn, @cHP)", db.getConnection());
                    command.Parameters.Add("@cL", MySqlDbType.VarChar).Value = RH.getLogin();
                    command.Parameters.Add("@cN", MySqlDbType.VarChar).Value = RH.getName();
                    command.Parameters.Add("@cSn", MySqlDbType.VarChar).Value = RH.getSurname();
                    command.Parameters.Add("@cG", MySqlDbType.VarChar).Value = RH.getGender();
                    command.Parameters.Add("@cM", MySqlDbType.VarChar).Value = RH.getMail();
                    command.Parameters.Add("@cPn", MySqlDbType.VarChar).Value = RH.getPhone();
                    command.Parameters.Add("@cHP", MySqlDbType.VarChar).Value = RH.getPassword();

                    db.openConnection();
                    if (command.ExecuteNonQuery() == 1)
                        MessageBox.Show("Account was created");
                    else
                        MessageBox.Show("Account wasn't created");

                    
                    MessageBox.Show("You have been registered in the app!", "Congratulattions!");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Something wrong!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally {
                    db.closeConnection();
                    backUp.Show();
                    Close();
                }

            }
        }
        public Boolean IsUserExist() // метод проверки существования пользователя в базе данных
        {
            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `client` WHERE `login` = @uL", db.getConnection());
            command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = LoginTextBox.Text;

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
                return true;
            else
                return false;
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
                return true;
            else
                return false;
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

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.languageCode = "ru-RU";
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");
            RegistrationWindow newWin = new RegistrationWindow(backUp);
            newWin.Show();
            this.Close();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.languageCode = "en-US";
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            RegistrationWindow newWin = new RegistrationWindow(backUp);
            newWin.Show();
            this.Close();
        }
    }
}
