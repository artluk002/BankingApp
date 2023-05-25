using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Threading;
using MySql.Data.MySqlClient;
using CourseProject.Classes;
using System.Data;

namespace CourseProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        private DB db;
        public MainWindow()
        {
            InitializeComponent();
            db = DB.getInstance();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LinkLableRegistration_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistrationWindow rw = new RegistrationWindow(this);
            this.Hide();
            rw.Show();
        }

        private void LinkLableLostPasswordOrLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RecoveryWindow RW = new RecoveryWindow(this);
            this.Hide(); 
            RW.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.languageCode= "en-US";
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            MainWindow newWin = new MainWindow();
            newWin.Show();
            this.Close();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.languageCode= "ru-RU";
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");
            this.UpdateLayout();
            MainWindow newWin = new MainWindow();
            newWin.Show();
            this.Close();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = LoginTextBox.Text;
                string password = (login.Substring(0, login.Length / 2) + PasswordTextBox.Password + login.Substring(login.Length / 2, login.Length / 2));

                DataTable table = new DataTable();

                MySqlDataAdapter adapter = new MySqlDataAdapter();

                MySqlCommand command = new MySqlCommand("SELECT * FROM `client` WHERE `login` = @uL AND `hash_password` = @uP", db.getConnection());
                command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = login;
                command.Parameters.Add("@uP", MySqlDbType.VarChar).Value = password.GetHashCode().ToString();

                adapter.SelectCommand = command;
                adapter.Fill(table);

                if (table.Rows.Count > 0)
                {
                    DataRow[] foundrow = table.Select();
                    if (DataStorage.client != null)
                        DataStorage.client = null;
                    DataStorage.client = Client.getInstance(Convert.ToUInt32(foundrow[0][0]), foundrow[0][1].ToString(), foundrow[0][2].ToString(), foundrow[0][3].ToString(), foundrow[0][4].ToString(), foundrow[0][5].ToString(), foundrow[0][6].ToString(), foundrow[0][7].ToString());
                    this.Hide();
                    GeneralWindow GW = new GeneralWindow();
                    GW.Show();
                    /*MainForm mF = new MainForm(table);
                    mF.Show();*/
                }
                else
                    MessageBox.Show("User with such data does not exist!");
            }
            catch (Exception t)
            {
                MessageBox.Show(Convert.ToString(t), "Error");
            }
            
            
        }
    }
}
