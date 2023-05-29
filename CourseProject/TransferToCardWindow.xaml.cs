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
    /// Логика взаимодействия для TransferToCardWindow.xaml
    /// </summary>
    public partial class TransferToCardWindow : Window
    {
        private GeneralWindow backUp;
        private Card ToCard;
        private DB db = DB.getInstance();
        public TransferToCardWindow(GeneralWindow generalWindow)
        {
            InitializeComponent();
            this.backUp = generalWindow;
            FromCardNumberTB.Text = DataStorage.card.Number;
            FromCardExpirityDateMounthTB.Text = DataStorage.card.Card_Date.ToString("MM");
            FromCardExpirityDateYearTB.Text = DataStorage.card.Card_Date.ToString("yy");
            ToCardNumberTB.Text = DataStorage.toSendCardNumber;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

        private void ToCardNumberTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Регулярное выражение, разрешающее только цифры и ограничение на 16 символа
            Regex regex = new Regex("^[0-9]{0,16}$");

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

        private void TransferBtn_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            double money;
            string transaction_number = "p";    
            for(int i = 0; i < 10; i++)
                transaction_number += r.Next(0, 10).ToString();
            if (FromCardCvvCodeTB.Text == "***")
            {
                MessageBox.Show("Enter your cvv code and try again", "Null cvv code error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(FromCardCvvCodeTB.Text != DataStorage.card.Cvv)
            {
                MessageBox.Show("Your cvv code isn't correct", "Cvv code error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(!double.TryParse(MoneyToSendTB.Text, out money))
            {
                MessageBox.Show("The amount you entered is incorrect. Check your amount and tyr again", "Money error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(DataStorage.card.Balance < money)
            {
                MessageBox.Show("You can't transfer more money than you have now");
                return;
            }
            if(!DataStorage.isCardExist(ToCardNumberTB.Text))
            {
                MessageBox.Show($"Card with such number: {ToCardNumberTB.Text} isn't exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Double from_currency = DataStorage.rates[DataStorage.card.Currency];
            Double to_currency;
            try
            {
                MySqlCommand command = new MySqlCommand("Select * from `card` where number = @n", db.getConnection());
                command.Parameters.Add("@n", MySqlDbType.VarChar).Value = ToCardNumberTB.Text;
                MySqlDataAdapter adapter = new MySqlDataAdapter() { SelectCommand= command };
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                    ToCard = new Card(Convert.ToInt32(row["id"]), row["type"].ToString(), row["number"].ToString(), row["cvv_code"].ToString(), Convert.ToDouble(row["balance"]), row["currency"].ToString(), row["payment_system"].ToString(), Convert.ToDateTime(row["card_date"]), row["pin_code"].ToString(), Convert.ToInt32(row["client_id"]));
                to_currency = DataStorage.rates[ToCard.Currency];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (DataStorage.card.Currency != "USD")
                money /= DataStorage.rates[DataStorage.card.Currency];
            if(money < 1.00)
            {
                MessageBox.Show($"Transfer amount cannot be less than {money * DataStorage.rates[DataStorage.card.Currency]} {DataStorage.card.Currency}", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            DataStorage.attempts = 0;
            AccessWindow AW = new AccessWindow();
            AW.ShowDialog();
            if(DataStorage.attempts > 0)
            {
                try
                {
                    MySqlCommand command = new MySqlCommand("UPDATE `card` SET `balance` = balance - @dm WHERE `id` = @idf", db.getConnection());
                    command.Parameters.Add("@dm", MySqlDbType.Double).Value = (money * DataStorage.rates[DataStorage.card.Currency]);
                    command.Parameters.Add("@idf", MySqlDbType.Int32).Value = DataStorage.card.Id;

                    MySqlCommand command1 = new MySqlCommand("UPDATE `card` SET `balance` = balance + @am WHERE `id` = @ids", db.getConnection());
                    command1.Parameters.Add("@am", MySqlDbType.Double).Value = (money * DataStorage.rates[ToCard.Currency]);
                    command1.Parameters.Add("@ids", MySqlDbType.Int32).Value = ToCard.Id;

                    MySqlCommand command2 = new MySqlCommand("INSERT INTO `transactions` (`type`, `destination`, `transaction_date`, `number`, `transaction_value`, `card_id`) VALUES (@t, @d, @td, @n, @tv, @ci)", db.getConnection());
                    command2.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Transfer";
                    command2.Parameters.Add("@d", MySqlDbType.VarChar).Value = $"to {ToCard.Number}";
                    command2.Parameters.Add("@td", MySqlDbType.DateTime).Value = DateTime.Now;
                    command2.Parameters.Add("@n", MySqlDbType.VarChar).Value = transaction_number;
                    command2.Parameters.Add("@tv", MySqlDbType.Double).Value = (money * DataStorage.rates[DataStorage.card.Currency]);
                    command2.Parameters.Add("ci", MySqlDbType.Int32).Value = DataStorage.card.Id;

                    MySqlCommand command3 = new MySqlCommand("INSERT INTO `transactions` (`type`, `destination`, `transaction_date`, `number`, `transaction_value`, `card_id`) VALUES (@t, @d, @td, @n, @tv, @ci)", db.getConnection());
                    command3.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Transfer";
                    command3.Parameters.Add("@d", MySqlDbType.VarChar).Value = $"From {DataStorage.card.Number}";
                    command3.Parameters.Add("@td", MySqlDbType.DateTime).Value = DateTime.Now;
                    command3.Parameters.Add("@n", MySqlDbType.VarChar).Value = transaction_number;
                    command3.Parameters.Add("@tv", MySqlDbType.Double).Value = (money * DataStorage.rates[ToCard.Currency]);
                    command3.Parameters.Add("ci", MySqlDbType.Int32).Value = ToCard.Id;

                    db.openConnection();
                    command.ExecuteNonQuery();
                    command1.ExecuteNonQuery();
                    command2.ExecuteNonQuery();
                    command3.ExecuteNonQuery();
                    Close();
                    backUp.updateCards();
                }
                catch(Exception ex)
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
    }
}
