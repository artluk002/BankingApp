using CourseProject.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для InternetPaymentsWindow.xaml
    /// </summary>
    public partial class InternetPaymentsWindow : Window
    {
        private DB db = DB.getInstance();
        private GeneralWindow backUp;
        private string ChoosenInternetService;
        private string PersonalNumber;
        private double money;
        public InternetPaymentsWindow(GeneralWindow backUp)
        {
            InitializeComponent();
            Load_Internet_Services();
            FromCardNumberTB.Text = DataStorage.card.Number;
            FromCardExpirityDateMounthTB.Text = DataStorage.card.Card_Date.ToString("MM");
            FromCardExpirityDateYearTB.Text = DataStorage.card.Card_Date.ToString("yy");
            this.backUp = backUp;
        }

        private void MoneyToSendTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешение ввода только десятичных чисел
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MoneyToSendTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(MoneyToSendTB.Text, out money))
            {
                MessageBox.Show("You enter invalid money!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                MoneyToSendTB.Text = "";
                MoneyToSendTB.Focus();
                return;
            }
            if (money < 1)
            {
                MessageBox.Show("Money can't be lower than 1", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                MoneyToSendTB.Text = "";
                MoneyToSendTB.Focus();
                return;
            }
        }

        private void FromCardCvvCodeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Регулярное выражение, разрешающее только цифры и ограничение на 3 символа
            Regex regex = new Regex("^[0-9]{0,3}$");

            // Проверяем, что вводимый текст соответствует регулярному выражению
            if (!regex.IsMatch(((TextBox)sender).Text + e.Text))
            {
                e.Handled = true; // Предотвращаем ввод символа
            }
        }

        private void TopUpYourAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PersonalNumber.Length < 10)
            {
                MessageBox.Show("Your personal account number should have 10 characters!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DataStorage.attempts = 0;
            AccessWindow AW = new AccessWindow();
            AW.ShowDialog();
            if (DataStorage.attempts > 0)
            {
                money /= DataStorage.rates["BYN"];
                Random r = new Random();
                DateTime transDate = DateTime.Now;
                string transaction_number = "p";

                for (int i = 0; i < 10; i++)
                    transaction_number += r.Next(0, 10).ToString();

                string desc1 = $"Transaction number: {transaction_number}.\n" +
                   $"Date: {DateTime.Now}.\n" +
                   $"Type: Payment.\n" +
                   $"Payment of internet ({ChoosenInternetService}) to personal account {PersonalNumber} from card {DataStorage.card.Number}.\n" +
                   $"Sent amount: {(money * DataStorage.rates[DataStorage.card.Currency])} {DataStorage.card.Currency}.";

                try
                {
                    MySqlCommand command = new MySqlCommand("UPDATE `card` SET `balance` = balance - @dm WHERE `id` = @idf", db.getConnection());
                    command.Parameters.Add("@dm", MySqlDbType.Double).Value = money * DataStorage.rates[DataStorage.card.Currency];
                    command.Parameters.Add("@idf", MySqlDbType.Int32).Value = DataStorage.card.Id;

                    MySqlCommand command2 = new MySqlCommand("INSERT INTO `transactions` (`type`, `destination`, `transaction_date`, `number`, `transaction_value`, `card_id`, `description`) VALUES (@t, @d, @td, @n, @tv, @ci, @desc)", db.getConnection());
                    command2.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Payment";
                    command2.Parameters.Add("@d", MySqlDbType.VarChar).Value = $"to {ChoosenInternetService}";
                    command2.Parameters.Add("@td", MySqlDbType.DateTime).Value = DateTime.Now;
                    command2.Parameters.Add("@n", MySqlDbType.VarChar).Value = transaction_number;
                    command2.Parameters.Add("@tv", MySqlDbType.Double).Value = money * DataStorage.rates[DataStorage.card.Currency];
                    command2.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.card.Id;
                    command2.Parameters.Add("@desc", MySqlDbType.Text).Value = desc1;

                    MySqlCommand command3 = new MySqlCommand("Update `client_services` set `balance` = balance + @m where name = @n and type = @t", db.getConnection());
                    command3.Parameters.Add("@m", MySqlDbType.Double).Value = (money * DataStorage.rates["BYN"]);
                    command3.Parameters.Add("@n", MySqlDbType.VarChar).Value = ChoosenInternetService;
                    command3.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Internet";

                    MySqlCommand command1 = new MySqlCommand("", db.getConnection());

                    db.openConnection();
                    command.ExecuteNonQuery();
                    command2.ExecuteNonQuery();
                    command3.ExecuteNonQuery();
                    backUp.updateCards();
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                finally
                {
                    db.closeConnection();
                }

            }
        }

        private void InternetServecesCB_DropDownClosed(object sender, EventArgs e)
        {
            ChoosenInternetService = InternetServecesCB.Text;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PersonalAccountTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Регулярное выражение, разрешающее только цифры и ограничение на 16 символа
            Regex regex = new Regex("^[0-9]{0,10}$");

            // Проверяем, что вводимый текст соответствует регулярному выражению
            if (!regex.IsMatch(((TextBox)sender).Text + e.Text))
            {
                e.Handled = true; // Предотвращаем ввод символа
            }
        }

        private void PersonalAccountTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            PersonalNumber = PersonalAccountTB.Text;
        }
        public void Load_Internet_Services()
        {
            try
            {
                MySqlCommand command = new MySqlCommand("Select `id`, `name` from `client_services` where `type` = @t ", db.getConnection());
                command.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Internet";

                MySqlDataAdapter adapter = new MySqlDataAdapter() { SelectCommand = command };
                DataTable table = new DataTable();

                db.openConnection();
                adapter.Fill(table);
                InternetServecesCB.ItemsSource = table.DefaultView;
                InternetServecesCB.DisplayMemberPath = "name";
                InternetServecesCB.SelectedValuePath = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            { db.closeConnection(); }
        }
    }
}
