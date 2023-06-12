using CourseProject.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
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
    /// Логика взаимодействия для PersonalAccountWindow.xaml
    /// </summary>
    public partial class PersonalAccountWindow : Window
    {
        private GeneralWindow backUp;
        private paymentType payment;
        private DB db = DB.getInstance();
        private Random r = new Random();

        public PersonalAccountWindow(GeneralWindow GW)
        {
            InitializeComponent();
            backUp = GW;
            payment = paymentType.None;
            CardNumberTB.Text = DataStorage.card.Number;
            CardExpirityDateMounthTB.Text = DataStorage.card.Card_Date.ToString("MM");
            CardExpirityDateYearTB.Text = DataStorage.card.Card_Date.ToString("yy");
            if(!isPersonalAccountExists((int)DataStorage.client.Id))
            {
                createPersonalAccount();
            }
            loadPerdoanlAccount();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
            backUp.updateCards();
        }

        private void TransferFromCardToAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            CardCanvas.IsEnabled = true;
            PersonalAccountNumberCanvas.IsEnabled = true;
            TransferSumCanvas.IsEnabled = true;
            payment = paymentType.FromCardToAccount;
        }

        private void TransferFromAccountToCardBtn_Click(object sender, RoutedEventArgs e)
        {
            CardCanvas.IsEnabled = true;
            PersonalAccountNumberCanvas.IsEnabled = false;
            TransferSumCanvas.IsEnabled = true;
            payment = paymentType.FromAccountToCard;
        }

        private void TransferFromAccountToAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            CardCanvas.IsEnabled = false;
            PersonalAccountNumberCanvas.IsEnabled = true;
            TransferSumCanvas.IsEnabled = true;
            payment = paymentType.FromAccountToAccount;
        }

        private void TransferBtn_Click(object sender, RoutedEventArgs e)
        {
            double sum;
            MySqlCommand command1 = null, command2 = null, command3 = null, command4 = null;
            if(!double.TryParse(MoneyToSendTB.Text, out sum))
            {
                MessageBox.Show("You enter invalid sum", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string transaction_number = "p";

            string desc1;

            for (int i = 0; i < 10; i++)
                transaction_number += r.Next(0, 10).ToString();

            switch (payment)
            {
                case paymentType.FromCardToAccount:
                    if (!isPersonalAccountExists(PersonalAccountNumberTB2.Text))
                    {
                        MessageBox.Show($"Pesonal account with such number {PersonalAccountNumberTB2.Text} isn't exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    double cardSum = (sum / DataStorage.rates["BYN"]) * DataStorage.rates[DataStorage.card.Currency];
                    DataStorage.attempts = 0;
                    AccessWindow AW = new AccessWindow();
                    AW.ShowDialog();
                    if(sum > DataStorage.card.Balance)
                    {
                        MessageBox.Show("You don't have enough money to transfer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if(DataStorage.attempts > 0)
                    {
                        command1 = new MySqlCommand("Update `card` set balance = balance - @s where `id` =  @i", db.getConnection());
                        command1.Parameters.Add("@s", MySqlDbType.Double).Value = cardSum;
                        command1.Parameters.Add("@i", MySqlDbType.Int32).Value = DataStorage.card.Id;
                        command2 = new MySqlCommand("Update `client_personal_account` set `personal_account_check` = `personal_account_check` + @s where `personal_account` = @n", db.getConnection());
                        command2.Parameters.Add("@s", MySqlDbType.Double).Value = sum;
                        command2.Parameters.Add("@n", MySqlDbType.VarChar).Value = PersonalAccountNumberTB2.Text;

                        desc1 = $"Transaction number: {transaction_number}.\n" +
                                       $"Date: {DateTime.Now}.\n" +
                                       $"Type: Transfer.\n" +
                                       $"Transfer from card: {DataStorage.card.Number}, to personal account: {PersonalAccountNumberTB2.Text}.\n" +
                                       $"Transfered amount: {cardSum} {DataStorage.card.Currency}.";

                        command3 = new MySqlCommand("INSERT INTO `transactions` (`type`, `destination`, `transaction_date`, `number`, `transaction_value`, `card_id`, `description`) VALUES (@t, @d, @td, @n, @tv, @ci, @desc)", db.getConnection());
                        command3.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Transfer";
                        command3.Parameters.Add("@d", MySqlDbType.VarChar).Value = $"From card: {DataStorage.card.Number},\nto personal account {PersonalAccountNumberTB2.Text}";
                        command3.Parameters.Add("@td", MySqlDbType.DateTime).Value = DateTime.Now;
                        command3.Parameters.Add("@n", MySqlDbType.VarChar).Value = transaction_number;
                        command3.Parameters.Add("@tv", MySqlDbType.Double).Value = cardSum;
                        command3.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.card.Id;
                        command3.Parameters.Add("@desc", MySqlDbType.Text).Value = desc1;

                        command4 = new MySqlCommand("Insert into `transactions` (`type`, `destination`, `transaction_date`, `number`, `transaction_value`, `card_id`, `description`) Values (@t, @d, @td, @n, @tv, (SELECT `id` FROM `card` WHERE `client_id` = (SELECT `client_id` from `client_personal_account` WHERE `client_personal_account`.`personal_account` = @pan) LIMIT 1), @desc)", db.getConnection());
                        command4.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Transfer";
                        command4.Parameters.Add("@d", MySqlDbType.VarChar).Value = $"From card: {DataStorage.card.Number},\nto personal account {PersonalAccountNumberTB2.Text}";
                        command4.Parameters.Add("@td", MySqlDbType.DateTime).Value = DateTime.Now;
                        command4.Parameters.Add("@n", MySqlDbType.VarChar).Value = transaction_number;
                        command4.Parameters.Add("@tv", MySqlDbType.Double).Value = sum;
                        command4.Parameters.Add("@pan", MySqlDbType.VarChar).Value = PersonalAccountNumberTB2.Text;
                        command4.Parameters.Add("@desc", MySqlDbType.Text).Value = desc1;

                    }
                    break;
                case paymentType.FromAccountToCard:
                    if(Convert.ToDouble(PersonalAccountBalance.Content) < sum)
                    {
                        MessageBox.Show("You don't have enough money to transfer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    cardSum = (sum / DataStorage.rates["BYN"]) * DataStorage.rates[DataStorage.card.Currency];
                    command1 = new MySqlCommand("Update `client_personal_account` set `personal_account_check` = `personal_account_check` - @s where `personal_account` = @n", db.getConnection());
                    command1.Parameters.Add("@s", MySqlDbType.Double).Value = sum;
                    command1.Parameters.Add("@n", MySqlDbType.VarChar).Value = PersonalAccountNumberTB.Text;
                    command2 = new MySqlCommand("Update `card` set balance = balance + @s where `id` =  @i", db.getConnection());
                    command2.Parameters.Add("@s", MySqlDbType.Double).Value = cardSum;
                    command2.Parameters.Add("@i", MySqlDbType.Int32).Value = DataStorage.card.Id;

                    desc1 = $"Transaction number: {transaction_number}.\n" +
                                       $"Date: {DateTime.Now}.\n" +
                                       $"Type: Transfer.\n" +
                                       $"Transfer from personal account: {PersonalAccountNumberTB.Text}, to card: {DataStorage.card.Number}.\n" +
                                       $"Transfered amount: {sum} BYN.";

                    command3 = new MySqlCommand("INSERT INTO `transactions` (`type`, `destination`, `transaction_date`, `number`, `transaction_value`, `card_id`, `description`) VALUES (@t, @d, @td, @n, @tv, @ci, @desc)", db.getConnection());
                    command3.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Transfer";
                    command3.Parameters.Add("@d", MySqlDbType.VarChar).Value = $"From personal account: {PersonalAccountNumberTB.Text},\nto card: {DataStorage.card.Number}";
                    command3.Parameters.Add("@td", MySqlDbType.DateTime).Value = DateTime.Now;
                    command3.Parameters.Add("@n", MySqlDbType.VarChar).Value = transaction_number;
                    command3.Parameters.Add("@tv", MySqlDbType.Double).Value = sum;
                    command3.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.card.Id;
                    command3.Parameters.Add("@desc", MySqlDbType.Text).Value = desc1;
                    break;
                case paymentType.FromAccountToAccount:
                    if (!isPersonalAccountExists(PersonalAccountNumberTB2.Text))
                    {
                        MessageBox.Show($"Pesonal account with such number {PersonalAccountNumberTB2.Text} isn't exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (Convert.ToDouble(PersonalAccountBalance.Content) < sum)
                    {
                        MessageBox.Show("You don't have enough money to transfer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    command1 = new MySqlCommand("Update `client_personal_account` set `personal_account_check` = `personal_account_check` - @s where `personal_account` = @n", db.getConnection());
                    command1.Parameters.Add("@s", MySqlDbType.Double).Value = sum;
                    command1.Parameters.Add("@n", MySqlDbType.VarChar).Value = PersonalAccountNumberTB.Text;
                    command2 = new MySqlCommand("Update `client_personal_account` set `personal_account_check` = `personal_account_check` + @s where `personal_account` = @n", db.getConnection());
                    command2.Parameters.Add("@s", MySqlDbType.Double).Value = sum;
                    command2.Parameters.Add("@n", MySqlDbType.VarChar).Value = PersonalAccountNumberTB2.Text;

                    desc1 = $"Transaction number: {transaction_number}.\n" +
                                       $"Date: {DateTime.Now}.\n" +
                                       $"Type: Transfer.\n" +
                                       $"Transfer from personal account: {PersonalAccountNumberTB.Text}, to personal account: {PersonalAccountNumberTB2.Text}.\n" +
                                       $"Transfered amount: {sum} BYN.";

                    command3 = new MySqlCommand("INSERT INTO `transactions` (`type`, `destination`, `transaction_date`, `number`, `transaction_value`, `card_id`, `description`) VALUES (@t, @d, @td, @n, @tv, @ci, @desc)", db.getConnection());
                    command3.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Transfer";
                    command3.Parameters.Add("@d", MySqlDbType.VarChar).Value = $"From personal account: {PersonalAccountNumberTB.Text},\nto personal account: {PersonalAccountNumberTB2.Text}";
                    command3.Parameters.Add("@td", MySqlDbType.DateTime).Value = DateTime.Now;
                    command3.Parameters.Add("@n", MySqlDbType.VarChar).Value = transaction_number;
                    command3.Parameters.Add("@tv", MySqlDbType.Double).Value = sum;
                    command3.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.card.Id;
                    command3.Parameters.Add("@desc", MySqlDbType.Text).Value = desc1;

                    command4 = new MySqlCommand("Insert into `transactions` (`type`, `destination`, `transaction_date`, `number`, `transaction_value`, `card_id`, `description`) Values (@t, @d, @td, @n, @tv, (SELECT `id` FROM `card` WHERE `client_id` = (SELECT `client_id` from `client_personal_account` WHERE `client_personal_account`.`personal_account` = @pan) LIMIT 1), @desc)", db.getConnection());
                    command4.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Transfer";
                    command4.Parameters.Add("@d", MySqlDbType.VarChar).Value = $"From personal account: {PersonalAccountNumberTB.Text},\nto personal account: {PersonalAccountNumberTB2.Text}";
                    command4.Parameters.Add("@td", MySqlDbType.DateTime).Value = DateTime.Now;
                    command4.Parameters.Add("@n", MySqlDbType.VarChar).Value = transaction_number;
                    command4.Parameters.Add("@tv", MySqlDbType.Double).Value = sum;
                    command4.Parameters.Add("@pan", MySqlDbType.VarChar).Value = PersonalAccountNumberTB2.Text;
                    command4.Parameters.Add("@desc", MySqlDbType.Text).Value = desc1;
                    break;
                default:
                    return;
            }
            try
            {
                db.openConnection();
                if(command1 != null)
                    command1.ExecuteNonQuery();
                if(command2 != null )
                    command2.ExecuteNonQuery();
                if (command3 != null)
                    command3.ExecuteNonQuery();
                if (command4 != null)
                    command4.ExecuteNonQuery();

            }
            catch( Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally { db.closeConnection(); loadPerdoanlAccount(); }
        }

        private void CardCvvCodeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Регулярное выражение, разрешающее только цифры и ограничение на 3 символа
            Regex regex = new Regex("^[0-9]{0,3}$");

            // Проверяем, что вводимый текст соответствует регулярному выражению
            if (!regex.IsMatch(((TextBox)sender).Text + e.Text))
            {
                e.Handled = true; // Предотвращаем ввод символа
            }
        }

        private void MoneyToSendTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешение ввода только десятичных чисел
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PersonalAccountNumberTB2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Регулярное выражение, разрешающее только цифры и ограничение на 10 символа
            Regex regex = new Regex("^[0-9]{0,10}$");

            // Проверяем, что вводимый текст соответствует регулярному выражению
            if (!regex.IsMatch(((TextBox)sender).Text + e.Text))
            {
                e.Handled = true; // Предотвращаем ввод символа
            }
        }
        private enum paymentType
        {
            FromCardToAccount,
            FromAccountToCard,
            FromAccountToAccount,
            None,
        }
        private bool isPersonalAccountExists(string account)
        {
            try
            {
                MySqlCommand command = new MySqlCommand("Select * from `client_personal_account` where `personal_account` = @pa", db.getConnection());
                command.Parameters.Add("@pa", MySqlDbType.VarChar).Value = account;

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable table = new DataTable();

                adapter.Fill(table);
                if (table.Rows.Count > 0)
                    return true;
                else 
                    return false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private bool isPersonalAccountExists(int client_id)
        {
            try
            {
                MySqlCommand command = new MySqlCommand("Select * from `client_personal_account` where `client_id` = @ci", db.getConnection());
                command.Parameters.Add("@ci", MySqlDbType.Int32).Value = client_id;

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable table = new DataTable();

                adapter.Fill(table);
                if (table.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private void loadPerdoanlAccount()
        {
            try
            {
                MySqlCommand command = new MySqlCommand("Select * from `client_personal_account` where client_id = @ci", db.getConnection());
                command.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.client.Id;
                MySqlDataAdapter adapter = new MySqlDataAdapter() { SelectCommand= command };
                DataTable table = new DataTable();
                adapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        PersonalAccountNumberTB.Text = row["personal_account"].ToString();
                        PersonalAccountBalance.Content = row["personal_account_check"].ToString();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private void createPersonalAccount()
        {
            try
            {
                string personal_account = "";
                do
                {
                    for (int i = 0; i < 10; i++)
                        personal_account += r.Next(0, 10).ToString();
                } while (isPersonalAccountExists(personal_account));

                MySqlCommand command = new MySqlCommand("INSERT INTO `client_personal_account`(personal_account, personal_account_check, client_id) Values(@pa, @pac, @ci)", db.getConnection());
                command.Parameters.Add("@pa", MySqlDbType.VarChar).Value = personal_account;
                command.Parameters.Add("@pac", MySqlDbType.Double).Value = 0;
                command.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.client.Id;

                db.openConnection();
                command.ExecuteNonQuery();
            }
            catch(Exception ex )
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            finally { db.closeConnection(); }
        }
    }
}
