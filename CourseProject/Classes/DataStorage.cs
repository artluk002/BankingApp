using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Classes
{
    internal class DataStorage
    {
        public static Client client;
        public static Card card;
        public static Dictionary<string, double> rates;
        public static string toSendCardNumber;
        public static int attempts;
        public static bool isCardExist(string cardNumber)
        {
            DB db = DB.getInstance();

            MySqlCommand command= new MySqlCommand("Select * from `card` where number = @n",db.getConnection());
            command.Parameters.Add("@n", MySqlDbType.VarChar).Value = cardNumber;
            MySqlDataAdapter adapter = new MySqlDataAdapter() { SelectCommand= command };
            DataTable table = new DataTable();
            adapter.Fill(table);

            if(table.Rows.Count > 0 )
            {
                return true;
            }
            return false;
        }
    }
}
