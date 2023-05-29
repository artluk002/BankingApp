using CourseProject.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace CourseProject
{
    /// <summary>
    /// Логика взаимодействия для TransactionsHistoryWindow.xaml
    /// </summary>
    public partial class TransactionsHistoryWindow : Window
    {
        private DB db = DB.getInstance();
        public TransactionsHistoryWindow()
        {
            InitializeComponent();
            Load_Transactions();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public void Load_Transactions()
        {
            try
            {
                MySqlCommand command = new MySqlCommand("Select * from `transactions` inner join `card` on `transactions`.`card_id` = `card`.`id` inner join `client` on `client`.`id` = `card`.`client_id` where `client`.`id` = @ci", db.getConnection());
                command.Parameters.Add("@ci", MySqlDbType.Int32).Value = DataStorage.client.Id;
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);
                List<Transactions> transactions = new List<Transactions>();
                foreach(DataRow row in table.Rows)
                {
                    transactions.Add(new Transactions(Convert.ToInt32(row["id"]), row["type"].ToString(), row["destination"].ToString(), Convert.ToDateTime(row["transaction_date"]).ToString("dd.MM.yyyy HH:mm:ss"), row["number"].ToString(), Convert.ToDouble(row["transaction_value"]), Convert.ToInt32(row["card_id"])));
                }
                transactions = transactions.OrderByDescending(x => x.Transaction_Date).ToList();
                TransactionsList.ItemsSource = transactions;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TransactionsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Load_Transactions();
        }
    }
}
