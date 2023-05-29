using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Classes
{
    internal class Transactions
    {
        private int id;
        private string type;
        public string Type { get { return type; } }
        private string destination;
        public string Destination { get { return destination; } }
        private string transaction_date;
        public string Transaction_Date { get { return transaction_date; } }
        private string number;
        public string Number { get { return number; } }
        private double transaction_value;
        public double Transaction_Value { get { return transaction_value; } }
        private int card_id;

        public Transactions(int id, string type, string destination, string transaction_date, string number, double transaction_value, int card_id)
        {
            this.id = id;
            this.type = type;
            this.destination = destination;
            this.transaction_date = transaction_date;
            this.number = number;
            this.transaction_value = transaction_value;
            this.card_id = card_id;
        }
    }
}
