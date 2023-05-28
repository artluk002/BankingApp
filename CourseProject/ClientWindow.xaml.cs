using CourseProject.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CourseProject
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private GeneralWindow backUp;
        private DB db = DB.getInstance();
        public ClientWindow()
        {
            InitializeComponent();
        }
        public ClientWindow(GeneralWindow generalWindow)
        {
            InitializeComponent();
            this.backUp = generalWindow;
            clientFullNameLable.Content = $"{DataStorage.client.Name} {DataStorage.client.Surname}";
            clientMailLable.Content = DataStorage.client.Mail;
            clientPhoneLable.Content = DataStorage.client.Phone;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
            backUp.Show();
        }

        private void EnLangBtn_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.languageCode = "ru-RU";
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");
            backUp = new GeneralWindow();
            backUp.Hide();
            ClientWindow newWin = new ClientWindow(backUp);
            newWin.Show();
            this.Close();
        }

        private void SignOutBtn_Click(object sender, RoutedEventArgs e)
        {
            DataStorage.client = null;
            DataStorage.card = null;
            DataStorage.toSendCardNumber = null;
            MainWindow MW = new MainWindow();
            backUp.Close();
            this.Close();
            MW.Show();
        }

        private void ChangePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword CPW = new ChangePassword();
            CPW.ShowDialog();
            updateClientInfo();
        }

        private void ChangeMailBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeMailWindow CPW = new ChangeMailWindow();
            CPW.ShowDialog();
            updateClientInfo();
        }

        private void ChangePhoneNumberBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePhoneWindow CPW = new ChangePhoneWindow();
            CPW.ShowDialog();
            updateClientInfo();
        }

        private void RuLangBtn_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.languageCode = "en-US";
            Properties.Settings.Default.Save();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            backUp = new GeneralWindow();
            backUp.Hide();
            ClientWindow newWin = new ClientWindow(backUp);
            newWin.Show();
            this.Close();
        }
        public void updateClientInfo()
        {
            try
            {
                MySqlCommand command = new MySqlCommand("Select * from `client` WHERE `id` = @ci;", db.getConnection());
                command.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.client.Id;

                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter() { SelectCommand = command };
                adapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    DataRow[] foundrow = table.Select();
                    Client.delInstance();
                    DataStorage.client = Client.getInstance(Convert.ToUInt32(foundrow[0][0]), foundrow[0][1].ToString(), foundrow[0][2].ToString(), foundrow[0][3].ToString(), foundrow[0][4].ToString(), foundrow[0][5].ToString(), foundrow[0][6].ToString(), foundrow[0][7].ToString());
                    clientFullNameLable.Content = $"{DataStorage.client.Name} {DataStorage.client.Surname}";
                    clientMailLable.Content = DataStorage.client.Mail;
                    clientPhoneLable.Content = DataStorage.client.Phone;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something wrong!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
