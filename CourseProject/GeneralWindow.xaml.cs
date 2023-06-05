using CourseProject.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Логика взаимодействия для GeneralWindow.xaml
    /// </summary>
    public partial class GeneralWindow : Window
    {
        private Regex phoneRegex = new Regex(@"^(\+375|80)(17|25|29|33|44)(\d{7})$");
        private DB db;
        public GeneralWindow()
        {
            InitializeComponent();

            db = DB.getInstance();

            if (!Directory.Exists(@"C:\Users\Public\Documents\BankingApp"))
                Directory.CreateDirectory(@"C:\Users\Public\Documents\BankingApp");
            if(!File.Exists(@"C:\Users\Public\Documents\BankingApp\log.txt"))
                File.Create(@"C:\Users\Public\Documents\BankingApp\log.txt").Close();

            using (StreamWriter sw = new StreamWriter(@"C:\Users\Public\Documents\BankingApp\log.txt", true))
            {
                sw.WriteLine($"[{DateTime.Now.ToString("yyyy-mm-dd HH-mm-ss")}]: logged by {DataStorage.client.Login}");
            }

            MySqlCommand command = new MySqlCommand("SELECT `last_update` FROM `currencies` WHERE `id` = @id", db.getConnection());
            command.Parameters.Add("@id", MySqlDbType.Int32).Value = 1;

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable table = new DataTable();

            adapter.SelectCommand= command;
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                DataRow[] dataRowrow = table.Select();
                TimeSpan lu = new TimeSpan(Convert.ToDateTime(dataRowrow[0][0]).Ticks);
                TimeSpan ct = new TimeSpan(DateTime.Now.Ticks);
                TimeSpan rs = ct.Subtract(lu);
                if (rs.Ticks > new TimeSpan(8, 0, 0).Ticks)
                {
                    CurrencyUpdater.UpdateAll();
                }
            }
            command = new MySqlCommand("SELECT * FROM `currencies`", db.getConnection());

            adapter = new MySqlDataAdapter();
            table = new DataTable();
            adapter.SelectCommand= command;
            adapter.Fill(table);
            if(table.Rows.Count > 0)
            {
                DataRow[] dataRowrow = table.Select();
                DataStorage.rates = new Dictionary<string, double>();
                foreach (DataRow row in dataRowrow)
                {
                    DataStorage.rates.Add(row[1].ToString(), Convert.ToDouble(row[2]));
                }
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewCardRegistration NCR = new NewCardRegistration(this);
            NCR.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateCards();
        }
        private void selectBankCard()
        {
            
            if (cardsCB.Text == "")
                return;
            MySqlCommand command = new MySqlCommand("Select * from `card` where number = @n", db.getConnection());
            command.Parameters.Add("@n", MySqlDbType.VarChar).Value = cardsCB.Text;

            MySqlDataAdapter adapter = new MySqlDataAdapter()
            { SelectCommand = command };
            DataTable currentCard = new DataTable();
            adapter.Fill(currentCard);
            foreach(DataRow row in currentCard.Rows)
            {
                DataStorage.card = new Card(Convert.ToInt32(row["id"]), row["type"].ToString(), row["number"].ToString(), row["cvv_code"].ToString(), Convert.ToDouble(row["balance"]), row["currency"].ToString(), row["payment_system"].ToString(), Convert.ToDateTime(row["card_date"]), row["pin_code"].ToString(), Convert.ToInt32(row["client_id"]));
            }
            if (DataStorage.card.Payment_System == "MasterCard")
            {
                MasterCardBackground.Visibility = Visibility.Visible;
                VisaBackground.Visibility = Visibility.Hidden;
            }
            else
            {
                VisaBackground.Visibility = Visibility.Visible;
                MasterCardBackground.Visibility = Visibility.Hidden;
            }
            lable_card_number.Content = $"{DataStorage.card.Number.Substring(0,4)} {DataStorage.card.Number.Substring(4, 4)} {DataStorage.card.Number.Substring(8, 4)} {DataStorage.card.Number.Substring(12, 4)}";
            lable_card_name_surname.Content = $"{DataStorage.client.Name.ToUpper()} {DataStorage.client.Surname.ToUpper()}";
            lable_card_date.Content = $"{DataStorage.card.Card_Date.ToString("MM")}/{DataStorage.card.Card_Date.ToString("yy")}";
            lable_balance.Content = $"{DataStorage.card.Balance}";
            lable_currency_code.Content = $"{DataStorage.card.Currency}";
            lable_card_cvv.Content = "***";
            lable_cvv_code.Visibility = Visibility.Visible;
            lable_expirity_date.Visibility = Visibility.Visible;
            lable_balance.Visibility = Visibility.Visible;
            lable_currency_code.Visibility = Visibility.Visible;
        }

        private void cardsCB_DropDownClosed(object sender, EventArgs e)
        {
            selectBankCard();
        }

        private void lable_card_cvv_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lable_card_cvv.Content == null)
                return;
            if (lable_card_cvv.Content == "***")
                lable_card_cvv.Content = DataStorage.card.Cvv;
            else
                lable_card_cvv.Content = "***";

        }
        public void updateCards()
        {
            try
            {
                MySqlCommand command = new MySqlCommand("Select id, number from `card` where client_id = @ci", db.getConnection());
                command.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.client.Id;
                db.openConnection();
                DataTable cards = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = command;
                adapter.Fill(cards);

                cardsCB.ItemsSource = cards.DefaultView;
                cardsCB.DisplayMemberPath = "number";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { db.closeConnection(); }
        }

        private void ToCardNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Регулярное выражение, разрешающее только цифры и ограничение на 16 символа
            Regex regex = new Regex("^[0-9]{0,16}$");

            // Проверяем, что вводимый текст соответствует регулярному выражению
            if (!regex.IsMatch(((TextBox)sender).Text + e.Text))
            {
                e.Handled = true; // Предотвращаем ввод символа
            }
        }

        private void TransferToCard_Click(object sender, RoutedEventArgs e)
        {
            if(ToCardNumberTB.Text.Length != 16)
            {
                MessageBox.Show("The card number had to be 16 digits", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(cardsCB.SelectedItem == null)
            {
                MessageBox.Show("To transfer money to another card, you need to select your card", "Attention", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            DataStorage.toSendCardNumber = ToCardNumberTB.Text;
            TransferToCardWindow TTCW = new TransferToCardWindow(this);
            TTCW.ShowDialog();
        }

        private void TransactionsHistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            TransactionsHistoryWindow THW = new TransactionsHistoryWindow();
            THW.ShowDialog();
        }

        private void ClientBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow CW = new ClientWindow(this);
            this.Hide();
            CW.Show();
        }

        private void TransferToPhone_Click(object sender, RoutedEventArgs e)
        {
            if(!phoneRegex.IsMatch(ToPhoneNumberTB.Text))
            {
                MessageBox.Show("Your number isn't correct, please check it and try again!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(DataStorage.card == null)
            {
                MessageBox.Show("Select your card!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DataStorage.toPhoneNumber = ToPhoneNumberTB.Text;
            TopUpMobileWindow TUMW = new TopUpMobileWindow(this);
            TUMW.ShowDialog();

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
                if((((TextBox)sender).Text + e.Text).Length > 11)
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

        private void CommunalPaymentsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DataStorage.card == null)
            {
                MessageBox.Show("Select your card!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CommunalPaymentsWindow CPW = new CommunalPaymentsWindow(this);
            CPW.ShowDialog();
        }

        private void InternetPaymentsBtn_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (DataStorage.card == null)
            {
                MessageBox.Show("Select your card!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            InternetPaymentsWindow IPW = new InternetPaymentsWindow(this);
            IPW.ShowDialog();
        }
    }
}
