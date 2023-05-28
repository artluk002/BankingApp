using CourseProject.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CourseProject
{
    /// <summary>
    /// Логика взаимодействия для ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        private Regex passwordRegex = new Regex(@"^.{8,20}$");
        private DB db = DB.getInstance();
        public ChangePassword()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CurrentPasswordBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (!passwordRegex.IsMatch(CurrentPasswordTB.Text))
            {
                CurrentPasswordTB.Focus();
                MessageBox.Show("Current Password isn't correct!\nPassword must be more than 8 and less than 20 characters!", "Attention");
                return;
            }
            if (!passwordRegex.IsMatch(NewPasswordTB.Text))
            {
                CurrentPasswordTB.Focus();
                MessageBox.Show("New Password isn't correct!\nPassword must be more than 8 and less than 20 characters!", "Attention");
                return;
            }
            if (!DataStorage.client.isPasswordMatch(DataStorage.client.Login, CurrentPasswordTB.Text))
            {
                MessageBox.Show("Current password don't mutch with your password!");
                return;
            }
            if (!NewPasswordTB.Text.Equals(NewPasswordRepitTB.Text))
            {
                NewPasswordTB.Focus();
                MessageBox.Show("The new passwords don't match!", "Attention");
                return;
            }
            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE `client` SET `hash_password` = @hp WHERE `id` = @ci;", db.getConnection());
                command.Parameters.Add("@hp", MySqlDbType.VarChar).Value = (DataStorage.client.Login.Substring(0, DataStorage.client.Login.Length / 2) + NewPasswordTB.Text + DataStorage.client.Login.Substring(DataStorage.client.Login.Length / 2, DataStorage.client.Login.Length / 2)).GetHashCode().ToString();
                command.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.client.Id;

                db.openConnection();
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("your password was updated");

                }
                else
                    MessageBox.Show("your password wasn't updated");
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
    }
}
