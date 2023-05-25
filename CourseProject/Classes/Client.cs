using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CourseProject.Classes
{
    public class Client
    {
        private uint _id;
        public uint Id { get { return _id; } }
        private string _login;
        public string Login { get => _login; }
        private string _name;
        public string Name { get => _name; }
        private string _surname;
        public string Surname { get => _surname; }
        private string _gender;
        public string Gender { get => _gender; }
        private string _mail;
        public string Mail { get => _mail; }
        private string _phone;
        public string Phone { get => _phone; }
        private string _password;

        private DB db;

        private static Client instance;

        public static Client getInstance()
        {
            if (instance != null)
                return instance;
            return null;
        }
        public static Client getInstance(uint id, string login, string name, string surname, string gender, string mail, string phone, string password) // получение текщего экземпляра класса Client, если он уже существет или создание нового если он отсутствует
        {
            if (instance == null)
                instance = new Client(id, login, name, surname, gender, mail, phone, password);
            return instance;
        }

        public static void delInstance() { instance = null; }

        private Client() { }

        public Client(uint id, string login, string name, string surname, string gender, string mail, string phone, string password)
        {
            _id = id;
            _login = login;
            _name = name;
            _surname = surname;
            _gender = gender;
            _mail = mail;
            _phone = phone;
            _password = password;
            db = DB.getInstance();
        }

        public bool isPasswordMatch(string login , string password)
        {
            string FL = login.Substring(0, login.Length / 2);
            string SL = login.Substring(login.Length / 2, login.Length / 2);
            string NP = FL + password + SL;
            int HP = NP.GetHashCode();
            if (HP == Convert.ToInt32(_password))
                return true;
            else
                return false;
        }
        public void updatePassword(string newPassword)
        {
            string FL = _login.Substring(0, _login.Length / 2);
            string SL = _login.Substring(_login.Length / 2, _login.Length / 2);
            string NP = FL + newPassword + SL;
            int HP = NP.GetHashCode();

            try
            {

                MySqlCommand command = new MySqlCommand("UPDATE `client` SET `hash_password` = @uP WHERE `client`.`id` = @uI;", db.getConnection());
                command.Parameters.Add("@uP", MySqlDbType.VarChar).Value = HP.ToString();
                command.Parameters.Add("@uI", MySqlDbType.UInt32).Value = _id;

                db.openConnection();

                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Your password have been changed");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(Convert.ToString(exc), "Error");
            }
            finally { db.closeConnection(); }
        }
    }
}
