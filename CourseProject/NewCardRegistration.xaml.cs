using CourseProject.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    /// Логика взаимодействия для NewCardRegistration.xaml
    /// </summary>
    public partial class NewCardRegistration : Window
    {
        private Random random= new Random();
        private DB db = DB.getInstance();
        private MySqlDataAdapter adapter = new MySqlDataAdapter();
        private DataTable table = new DataTable();
        private GeneralWindow backUp;
        public NewCardRegistration(GeneralWindow generalWindow)
        {
            InitializeComponent();
            CardTypeCB.Items.Add(Properties.Resources.credittypecard);
            CardTypeCB.Items.Add(Properties.Resources.debittypecard);
            foreach(var item in DataStorage.rates)
            {
                CurrencyCB.Items.Add(item.Key);
            }
            PaymentSystemCB.Items.Add("Visa");
            PaymentSystemCB.Items.Add("MasterCard");
            backUp = generalWindow;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateCardBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string pin = PinCodeTextBox.Text;
                if (pin.Length != 4)
                {
                    MessageBox.Show("Pin code should have 4 numbers!", "Error");
                }
                string cardType;
                if (CardTypeCB.SelectionBoxItem.ToString() == Properties.Resources.debittypecard)
                    cardType = "Debit";
                else
                    cardType = "Credit";
                var currency = CurrencyCB.SelectionBoxItem.ToString();
                var paymentSystem = PaymentSystemCB.SelectionBoxItem.ToString();
                var cardNumber = "";
                var cardPin = PinCodeTextBox.Text;
                var cvvCode = "";
                bool isCardFree = false;
                DateTime dateTime = DateTime.Now;
                var cardDate = dateTime.AddYears(5);

                for (int i = 0; i < 3; i++)
                    cvvCode += Convert.ToString(random.Next(0, 10));
                do
                {
                    if (paymentSystem == "Visa")
                        cardNumber = "4";
                    else
                        cardNumber = "5";
                    for (int i = 0; i < 15; i++)
                        cardNumber += Convert.ToString(random.Next(0, 10));
                    MySqlCommand command = new MySqlCommand("Select * from `card` where `number`= @cn", db.getConnection());
                    command.Parameters.Add("@cn", MySqlDbType.VarChar).Value = cardNumber;

                    adapter.SelectCommand = command;
                    adapter.Fill(table);
                    if (table.Rows.Count == 0)
                        isCardFree = true;
                } while (!isCardFree);


                MySqlCommand command1 = new MySqlCommand("Insert into `card`(type, number, cvv_code, currency, payment_system, card_date, pin_code, client_id) values( @t, @n, @cc, @c, @ps, @cd, @pn, @ci)", db.getConnection());
                command1.Parameters.Add("@t", MySqlDbType.VarChar).Value = cardType;
                command1.Parameters.Add("@n", MySqlDbType.VarChar).Value = cardNumber;
                command1.Parameters.Add("@cc", MySqlDbType.VarChar).Value = cvvCode;
                command1.Parameters.Add("@c", MySqlDbType.VarChar).Value = currency;
                command1.Parameters.Add("@ps", MySqlDbType.VarChar).Value = paymentSystem;
                command1.Parameters.Add("@cd", MySqlDbType.DateTime).Value = cardDate;
                command1.Parameters.Add("@pn", MySqlDbType.VarChar).Value = pin;
                command1.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.client.Id;

                db.openConnection();
                command1.ExecuteNonQuery();

                MessageBox.Show("You card was successfully created", "Date has been saved", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
                backUp.updateCards();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { db.closeConnection(); }
        }

        private void PinCodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Регулярное выражение, разрешающее только цифры и ограничение на 4 символа
            Regex regex = new Regex("^[0-9]{0,4}$");

            // Проверяем, что вводимый текст соответствует регулярному выражению
            if (!regex.IsMatch(((TextBox)sender).Text + e.Text))
            {
                e.Handled = true; // Предотвращаем ввод символа
            }
        }
    }
}
