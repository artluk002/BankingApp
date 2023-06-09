using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Classes
{
    internal class Credit
    {
        private int _id;
        public int Id { get => _id; } 
        private double _total_sum;
        public double Total_sum { get => _total_sum; } 
        private double _credit_sum;
        public double Credit_sum { get => _credit_sum; } 
        private DateTime _credit_date;
        public DateTime Credit_date { get => _credit_date; } 
        private int _credit_status;
        public int Credit_status { get => _credit_status; } 
        private DateTime _repayment_date;
        public DateTime Repayment_date { get => _repayment_date; } 
        private double _repayment_sum;
        public double Repayment_sum { get => _repayment_sum; }
        private int _card_id;
        public int Card_id { get => _card_id;  }
        private string _currency;
        public string Currency { get => _currency; }

        public Credit(int id, double total_sum, double credit_sum, DateTime credit_date, int credit_status, DateTime repayment_date, double repayment_sum, int card_id, string currency)
        {
            _id = id;
            _total_sum = total_sum;
            _credit_sum = credit_sum;
            _credit_date = credit_date;
            _credit_status = credit_status;
            _repayment_date = repayment_date;
            _repayment_sum = repayment_sum;
            _card_id = card_id;
            _currency = currency;
        }
    }
}
