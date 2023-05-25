using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Classes
{
    internal class Card
    {
        private int _id;
        public int Id { get { return _id; } }
        private string _type;
        public string Type { get { return _type; } }
        private string _number;
        public string Number { get { return _number; } }
        private string _cvv;
        public string Cvv { get { return _cvv; } }
        private double _balance;
        public double Balance { get { return _balance; } }
        private string _currency;
        public string Currency { get { return _currency; } }
        private string _payment_system;
        public string Payment_System { get { return _payment_system; } }
        private DateTime _card_date;
        public DateTime Card_Date { get { return _card_date; } }
        private string _pin_code;
        private int _client_id;

        public Card(int id, string type, string number, string cvv, double balance, string currency, string payment_system, DateTime card_date, string pin_code, int client_id)
        {
            _id = id;
            _type = type;
            _number = number;
            _cvv = cvv;
            _balance = balance;
            _currency = currency;
            _payment_system = payment_system;
            _card_date = card_date;
            _pin_code = pin_code;
            _client_id = client_id;
        }
        public bool CheckPinCode(string pin_code)
        {
            if(_pin_code == pin_code) return true;
            return false;
        }
    }
}
