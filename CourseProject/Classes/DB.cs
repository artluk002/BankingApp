using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Classes
{
    internal class DB
    {
        MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=banking_app"); // установка соединения с базой данных

        private static DB instance;

        private DB() { } // приватный конструктор без параметров

        public static DB getInstance() // получение текщего экземпляра класса DB, если он уже существет или создание нового если он отсутствует
        {
            if (instance == null)
                instance = new DB();
            return instance;
        }

        public void openConnection() // открытие соединения с базой данных
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }
        public void closeConnection() // закрытие соединения с базой данных
        {
            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }
        public MySqlConnection getConnection() // получение поединения 
        {
            return connection;
        }
    }
}
