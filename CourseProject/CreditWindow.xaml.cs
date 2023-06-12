using CourseProject.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Entity;
using System.Linq;
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
    /// Логика взаимодействия для CreditWindow.xaml
    /// </summary>
    public partial class CreditWindow : Window
    {
        private GeneralWindow backUp;
        private DB db = DB.getInstance();
        private string currentCurrency;
        private int minCreditSumm = 400;
        private int maxCreditSumm = 90000;
        private int minCreditMounth = 4;
        private int maxCreditMounth = 72;
        private double annualRate = 12.5;
        private Credit credit;
        public Random r = new Random();
        public CreditWindow(GeneralWindow general)
        {
            InitializeComponent();
            CurrencyCB.Items.Add("BYN");
            CurrencyCB.Items.Add("USD");
            CurrencyCB.Items.Add("EUR");
            currentCurrency = "BYN";
            CurrencyCB.SelectedIndex = 0;
            sumSlider.Minimum = minCreditSumm;
            sumSlider.Maximum = maxCreditSumm;
            mounthSlider.Minimum = minCreditMounth;
            mounthSlider.Maximum = maxCreditMounth;
            backUp = general;
            monthlypayment_Balance.Content = calculateMounthlyPayment((int)sumSlider.Value, (int)mounthSlider.Value, annualRate).ToString();
            loadCredit();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
            backUp.updateCards();
        }

        private void MoneyToCreditTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешение ввода только целых чисел
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void sumSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sumSlider.Value = Math.Round(sumSlider.Value);
            monthlypayment_Balance.Content = calculateMounthlyPayment((int)sumSlider.Value, (int)mounthSlider.Value, annualRate).ToString();
        }

        private void CurrencyCB_DropDownClosed(object sender, EventArgs e)
        {
            switch (CurrencyCB.SelectedValue)
            {
                case "BYN":
                    minCreditSumm = 400;
                    maxCreditSumm = 90000;
                    currentCurrency = "BYN";
                    break;
                case "USD":
                    minCreditSumm = 100;
                    maxCreditSumm = 31000;
                    currentCurrency = "USD";
                    break;
                case "EUR":
                    minCreditSumm = 100;
                    maxCreditSumm = 29000;
                    currentCurrency = "EUR";
                    break;
                default:
                    break;
            }
            sumSlider.Minimum = minCreditSumm;
            sumSlider.Maximum = maxCreditSumm;
            CurrencyLable.Content = currentCurrency;
        }

        private void MounthToCreditTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешение ввода только целых чисел
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void mounthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mounthSlider.Value = Math.Round(mounthSlider.Value);
            monthlypayment_Balance.Content = calculateMounthlyPayment((int)sumSlider.Value, (int)mounthSlider.Value, annualRate).ToString();
        }

        private void ApplyForALoanBtn_Click(object sender, RoutedEventArgs e)
        {
            double credit_sum = sumSlider.Value;
            int number_of_mounths = (int)mounthSlider.Value;
            DateTime credit_date = DateTime.Now;
            DateTime repayment_date = credit_date.AddMonths(1);
            double total_sum = calculateTotalLoanAmount((int)credit_sum, number_of_mounths, annualRate);
            double payment = calculateMounthlyPayment((int)credit_sum, number_of_mounths, annualRate);

            DataStorage.attempts = 0;
            AccessWindow AW = new AccessWindow();
            AW.ShowDialog();
            if (DataStorage.attempts <= 0)
                return;
            
            string transaction_number = "p";

            for (int i = 0; i < 10; i++)
                transaction_number += r.Next(0, 10).ToString();
            try
            {
                double moneyToCard;
                if (DataStorage.card.Currency != currentCurrency)
                {
                    moneyToCard = credit_sum / DataStorage.rates[currentCurrency];
                    moneyToCard *= DataStorage.rates[DataStorage.card.Currency];
                }
                else
                    moneyToCard = credit_sum;

                MySqlCommand command = new MySqlCommand("INSERT INTO `credit`(total_sum, credit_sum, credit_date, repayment_date, repayment_sum, card_id, currency) values(@ts, @cs, @cd, @rd, @rs, @ci, @cu)", db.getConnection());
                command.Parameters.Add("@ts", MySqlDbType.Double).Value = total_sum;
                command.Parameters.Add("@cs", MySqlDbType.Double).Value = 0;
                command.Parameters.Add("@cd", MySqlDbType.DateTime).Value = credit_date;
                command.Parameters.Add("@ci", MySqlDbType.Int64).Value = DataStorage.card.Id;
                command.Parameters.Add("@rd", MySqlDbType.DateTime).Value = repayment_date;
                command.Parameters.Add("@rs", MySqlDbType.Double).Value = payment;
                command.Parameters.Add("@cu", MySqlDbType.VarChar).Value = currentCurrency;

                MySqlCommand command1 = new MySqlCommand("Select * from `credit` where `card_id` = @ci", db.getConnection());
                command1.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.card.Id;

                MySqlCommand command2 = new MySqlCommand("Update `card` set balance = balance + @m where id = @i", db.getConnection());
                command2.Parameters.Add("@m", MySqlDbType.Double).Value = moneyToCard;
                command2.Parameters.Add("@i", MySqlDbType.Int32).Value = DataStorage.card.Id;

                MySqlCommand command3 = new MySqlCommand("Insert into `transactions`(type, destination, transaction_date, number, transaction_value, card_id, description) values(@t, @d, @td, @n, @tv, @ci, @de)", db.getConnection());
                command3.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Credit";
                command3.Parameters.Add("@d", MySqlDbType.VarChar).Value = $"On card {DataStorage.card.Number}";
                command3.Parameters.Add("@td", MySqlDbType.DateTime).Value = DateTime.Now;
                command3.Parameters.Add("@n", MySqlDbType.VarChar).Value = transaction_number;
                command3.Parameters.Add("@tv", MySqlDbType.Double).Value = credit_sum;
                command3.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.card.Id;
                command3.Parameters.Add("@de", MySqlDbType.Text).Value =  $"Transaction number: {transaction_number}.\n" +
                                                                          $"Date: {DateTime.Now}.\n" +
                                                                          $"Type: Credit.\n" +
                                                                          $"A loan was taken from a bank in the amount of {credit_sum} {currentCurrency}, to a card {DataStorage.card.Number}";

                MySqlDataAdapter adapter = new MySqlDataAdapter() { SelectCommand = command1 };
                DataTable table = new DataTable();

                db.openConnection();
                command.ExecuteNonQuery();

                adapter.Fill(table);
                if (table.Rows.Count > 0)
                    foreach (DataRow row in table.Rows)
                        credit = new Credit(Convert.ToInt32(row["id"]), Convert.ToDouble(row["total_sum"]), Convert.ToDouble(row["credit_sum"]), Convert.ToDateTime(row["credit_date"]), Convert.ToInt32(row["credit_status"]), Convert.ToDateTime(row["repayment_date"]), Convert.ToDouble(row["repayment_sum"]), Convert.ToInt32(row["card_id"]), row["currency"].ToString());

                command2.ExecuteNonQuery();

                command3.ExecuteNonQuery();

                MessageBox.Show("Credit completed!", "Successfully", MessageBoxButton.OK, MessageBoxImage.Information);

                loadCredit();
                TakeCreditCanvas.Visibility = Visibility.Hidden;
                RepayCreditCanvas.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally { db.closeConnection(); }


        }
        private double calculateMounthlyPayment(int moneyToCredit, int numberOfMounths, double annualRate)
        {
            double r = (annualRate / 12) / 100; // Преобразуем годовую процентную ставку в месячную:
            double mounthlyPayment = moneyToCredit * (r * Math.Pow(1 + r, numberOfMounths) / (Math.Pow(1 + r, numberOfMounths) - 1));// Подставим значения в формулу: Monthly Payment = Loan Amount * (r * (1 + r)^n) / ((1 + r)^n - 1)
            return Math.Round(mounthlyPayment, 2);
        }
        private double calculateTotalLoanAmount(int moneyToCredit, int numberOfMounths, double annualRate) => Math.Round(calculateMounthlyPayment(moneyToCredit, numberOfMounths, annualRate) * numberOfMounths, 2);
        private void loadCredit()
        {
            try
            {
                MySqlCommand command = new MySqlCommand("Select * from `credit` where `card_id` = @ci", db.getConnection());
                command.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.card.Id;

                MySqlDataAdapter adapter = new MySqlDataAdapter() { SelectCommand = command };
                DataTable table = new DataTable();

                adapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                        credit = new Credit(Convert.ToInt32(row["id"]), Convert.ToDouble(row["total_sum"]), Convert.ToDouble(row["credit_sum"]), Convert.ToDateTime(row["credit_date"]), Convert.ToInt32(row["credit_status"]), Convert.ToDateTime(row["repayment_date"]), Convert.ToDouble(row["repayment_sum"]), Convert.ToInt32(row["card_id"]), row["currency"].ToString());
                    loanDate_Lable.Content = credit.Credit_date.ToString("dd-MM-yyyy");
                    PaidMoney_Lable.Content = credit.Credit_sum.ToString();
                    FullMoney_Lable.Content = credit.Total_sum.ToString();
                    NextPayment_Lable.Content = credit.Repayment_date.ToString("dd-MM-yyyy");
                    ToPayment_Lable.Content = credit.Repayment_sum.ToString();
                    CurrencyLable2.Content = credit.Currency;

                    TakeCreditCanvas.Visibility = Visibility.Hidden;
                    RepayCreditCanvas.Visibility = Visibility.Visible;
                }
                else
                {
                    TakeCreditCanvas.Visibility = Visibility.Visible;
                    RepayCreditCanvas.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void PayBtn_Click(object sender, RoutedEventArgs e)
        {
            double paymentSum = credit.Repayment_sum;
            paymentSum /= DataStorage.rates[credit.Currency];
            paymentSum *= DataStorage.rates[DataStorage.card.Currency];
            if(paymentSum > DataStorage.card.Balance )
            {
                MessageBox.Show("You don't have enough money to pay off the loan", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DateTime checkDate = credit.Repayment_date;
            DateTime nextPaymentDate = credit.Repayment_date.AddMonths(1);
            double credit_summ = credit.Credit_sum + credit.Repayment_sum;

            double repayment_sum = credit.Repayment_sum;

            if(checkDate.Date < DateTime.Now.Date)
            {
                while(checkDate != DateTime.Now.Date)
                {
                    repayment_sum /= 1.01;
                    checkDate = checkDate.AddDays(1);
                }
            }


            string transaction_number = "p";

            for (int i = 0; i < 10; i++)
                transaction_number += r.Next(0, 10).ToString();

            try
            {
                MySqlCommand command = new MySqlCommand("Update `card` set balance = balance - @ps where id = @ci", db.getConnection());
                command.Parameters.Add("@ps", MySqlDbType.Double).Value = paymentSum;
                command.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.card.Id;

                MySqlCommand command1;

                MySqlCommand command3 = new MySqlCommand("Insert into `transactions`(type, destination, transaction_date, number, transaction_value, card_id, description) values(@t, @d, @td, @n, @tv, @ci, @de)", db.getConnection());
                command3.Parameters.Add("@t", MySqlDbType.VarChar).Value = "Payment";
                command3.Parameters.Add("@d", MySqlDbType.VarChar).Value = $"To the bank for a loan";
                command3.Parameters.Add("@td", MySqlDbType.DateTime).Value = DateTime.Now;
                command3.Parameters.Add("@n", MySqlDbType.VarChar).Value = transaction_number;
                command3.Parameters.Add("@tv", MySqlDbType.Double).Value = paymentSum;
                command3.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.card.Id;
                command3.Parameters.Add("@de", MySqlDbType.Text).Value = $"Transaction number: {transaction_number}.\n" +
                                                                          $"Date: {DateTime.Now}.\n" +
                                                                          $"Type: Payment.\n" +
                                                                          $"Loan payment for {credit.Repayment_date}, in the amount of {paymentSum} {DataStorage.card.Currency}";

                if (credit_summ >= credit.Total_sum)
                {
                    command1 = new MySqlCommand("Delete from `credit` where card_id = @ci", db.getConnection());
                    command1.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.card.Id;
                    MessageBox.Show("You have successfully paid off the loan", "Congratulations", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    command1 = new MySqlCommand("UPDATE `credit` SET `credit_sum` = @cs, `repayment_date` = @rd, `repayment_sum` = @rs WHERE id = @i;", db.getConnection());
                    command1.Parameters.Add("@cs", MySqlDbType.Double).Value = credit_summ;
                    command1.Parameters.Add("@rd", MySqlDbType.DateTime).Value = nextPaymentDate;
                    command1.Parameters.Add("@rs", MySqlDbType.Double).Value = repayment_sum;
                    command1.Parameters.Add("@i", MySqlDbType.Int32).Value = credit.Id;
                }


                db.openConnection();
                command.ExecuteNonQuery();
                command1.ExecuteNonQuery();
                command3.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally { db.closeConnection(); }
            
            loadCredit();
            backUp.updateCards();
        }

        private void MoneyToCreditTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(MoneyToCreditTB.Text) > maxCreditSumm)
            {
                MoneyToCreditTB.Text = maxCreditSumm.ToString();
            }
            else if (Convert.ToInt32(MoneyToCreditTB.Text) < minCreditSumm)
            {
                MoneyToCreditTB.Text = minCreditSumm.ToString();
            }
        }

        private void MounthToCreditTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(MounthToCreditTB.Text) > maxCreditMounth)
            {
                MounthToCreditTB.Text = maxCreditMounth.ToString();
            }
            else if (Convert.ToInt32(MounthToCreditTB.Text) < minCreditMounth)
            {
                MounthToCreditTB.Text = minCreditMounth.ToString();
            }
        }
    }
}
