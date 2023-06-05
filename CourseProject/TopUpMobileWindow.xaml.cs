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
    /// Логика взаимодействия для TopUpMobileWindow.xaml
    /// </summary>
    public partial class TopUpMobileWindow : Window
    {
        private GeneralWindow backUp;
        private double money;
        private DB db = DB.getInstance();
        private string ChoosenOperator;
        public TopUpMobileWindow(GeneralWindow generalWindow)
        {
            InitializeComponent();
            this.backUp = generalWindow;
            ToPhoneNumberTB.Text = DataStorage.toPhoneNumber;
            FromCardNumberTB.Text = DataStorage.card.Number;
            FromCardExpirityDateMounthTB.Text = DataStorage.card.Card_Date.ToString("MM");
            FromCardExpirityDateYearTB.Text = DataStorage.card.Card_Date.ToString("yy");
            Load_MobileServices();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
            backUp.Show();
        }

        private void ToPhoneNumberTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((((TextBox)sender).Text + e.Text).StartsWith("+"))
            {
                if ((((TextBox)sender).Text + e.Text).Length > 13)
                {
                    e.Handled = true; // Предотвращаем ввод символа
                }
                else
                {
                    e.Handled = false;
                }
            }
            else if ((((TextBox)sender).Text + e.Text).StartsWith("8"))
            {
                if ((((TextBox)sender).Text + e.Text).Length > 11)
                {
                    e.Handled = true; // Предотвращаем ввод символа
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void MobilesServecesCB_DropDownClosed(object sender, EventArgs e)
        {
            ChoosenOperator = MobilesServecesCB.Text;
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
            string operatorCode;
            char nextNum;
            if (DataStorage.toPhoneNumber.StartsWith("+375"))
            {
                operatorCode = DataStorage.toPhoneNumber.Substring(4, 2);
                nextNum = DataStorage.toPhoneNumber[6];
            }
            else
            {
                operatorCode = DataStorage.toPhoneNumber.Substring(2, 2);
                nextNum = DataStorage.toPhoneNumber[4];
            }

            switch(operatorCode)
            {
                case "25":
                    if(ChoosenOperator != "Life")
                    {
                        MessageBox.Show("The carrier you selected does not match the phone number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                case "29":
                    if(ChoosenOperator == "Life")
                    {
                        MessageBox.Show("The carrier you selected does not match the phone number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else if (ChoosenOperator == "MTS")
                    {
                        if(nextNum != '2' && nextNum != '5' && nextNum != '7' && nextNum != '8')
                        {
                            MessageBox.Show("The carrier you selected does not match the phone number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {
                        if(nextNum != '1' && nextNum != '3' && nextNum != '6' && nextNum != '9')
                        {
                            MessageBox.Show("The carrier you selected does not match the phone number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    break;
                case "33":
                    if(ChoosenOperator != "MTS")
                    {
                        MessageBox.Show("The carrier you selected does not match the phone number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                case "44":
                    if (ChoosenOperator != "A1")
                    {
                        MessageBox.Show("The carrier you selected does not match the phone number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                default:
                    MessageBox.Show($"Operator with this code {operatorCode} does not exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
            }
            if(FromCardCvvCodeTB.Text != DataStorage.card.Cvv)
            {
                MessageBox.Show("Check your cvv code and try again!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                FromCardCvvCodeTB.Focus();
                return;
            }

            money /= DataStorage.rates["BYN"];
            money *= DataStorage.rates[DataStorage.card.Currency];

            if(DataStorage.card.Balance < money)
            {
                MessageBox.Show("You don't have enough money to top up your mobile phone!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            money /= DataStorage.rates[DataStorage.card.Currency];

            DataStorage.attempts = 0;
            AccessWindow AW = new AccessWindow();
            AW.ShowDialog();
            if (DataStorage.attempts > 0)
            {
                Random r = new Random();
                DateTime transDate = DateTime.Now;
                string transaction_number = "p";

                for (int i = 0; i < 10; i++)
                    transaction_number += r.Next(0, 10).ToString();

                string desc1 = $"Transaction number: {transaction_number}.\n" +
                   $"Date: {DateTime.Now}.\n" +
                   $"Type: Replenishment.\n" +
                   $"Replenishment phone number: {DataStorage.toPhoneNumber}, from card: {DataStorage.card.Number}.\n" +
                   $"Sent amount: {(money * DataStorage.rates[DataStorage.card.Currency])} {DataStorage.card.Currency}.";

                try
                {
                    MySqlCommand command = new MySqlCommand("UPDATE `card` SET `balance` = balance - @dm WHERE `id` = @idf", db.getConnection());
                    command.Parameters.Add("@dm", MySqlDbType.Double).Value = money * DataStorage.rates[DataStorage.card.Currency];
                    command.Parameters.Add("@idf", MySqlDbType.Int32).Value = DataStorage.card.Id;

                    MySqlCommand command2 = new MySqlCommand("INSERT INTO `transactions` (`type`, `destination`, `transaction_date`, `number`, `transaction_value`, `card_id`, `description`) VALUES (@t, @d, @td, @n, @tv, @ci, @desc)", db.getConnection());
                    command2.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Replenishment";
                    command2.Parameters.Add("@d", MySqlDbType.VarChar).Value = $"to {DataStorage.toPhoneNumber}";
                    command2.Parameters.Add("@td", MySqlDbType.DateTime).Value = DateTime.Now;
                    command2.Parameters.Add("@n", MySqlDbType.VarChar).Value = transaction_number;
                    command2.Parameters.Add("@tv", MySqlDbType.Double).Value = money * DataStorage.rates[DataStorage.card.Currency];
                    command2.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.card.Id;
                    command2.Parameters.Add("@desc", MySqlDbType.Text).Value = desc1;

                    MySqlCommand command3 = new MySqlCommand("Update `client_services` set `balance` = balance + @m where name = @n", db.getConnection());
                    command3.Parameters.Add("@m", MySqlDbType.Double).Value = (money * DataStorage.rates["BYN"]) / 1.02;
                    command3.Parameters.Add("@n", MySqlDbType.VarChar).Value = ChoosenOperator;

                    db.openConnection();
                    command.ExecuteNonQuery();
                    command2.ExecuteNonQuery();
                    command3.ExecuteNonQuery();
                    Close();
                    backUp.updateCards();
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

        private void MoneyToSendTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешение ввода только десятичных чисел
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MoneyToSendTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MoneyToSendTB.Text == "")
            {
                commissionLable.Content = "0";
                toPayLable.Content = "0";
                return;
            }
            if(!Double.TryParse(MoneyToSendTB.Text, out money))
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
            commissionLable.Content = (money * 0.02).ToString();
            toPayLable.Content = (money += money * 0.02).ToString();
        }
        public void Load_MobileServices()
        {
            try
            {
                MySqlCommand command = new MySqlCommand("Select `id`, `name` from `client_services` where `type` = @t ", db.getConnection());
                command.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Mobile";

                MySqlDataAdapter adapter = new MySqlDataAdapter() { SelectCommand = command };
                DataTable table = new DataTable();
            
                db.openConnection();
                adapter.Fill(table);
                MobilesServecesCB.ItemsSource = table.DefaultView;
                MobilesServecesCB.DisplayMemberPath = "name";
                MobilesServecesCB.SelectedValuePath = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            { db.closeConnection(); }
        }

        private void ToPhoneNumberTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataStorage.toPhoneNumber = ToPhoneNumberTB.Text;
        }
    }
}
