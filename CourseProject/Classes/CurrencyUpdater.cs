using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CourseProject.Classes
{
    public class CurrencyUpdater
    {
        private static DB db = DB.getInstance();
        private const string ApiUrl = "https://openexchangerates.org/api/latest.json?app_id=0e2de571a1d54fb2b7c60c73f1e217e9";

        public static async Task UpdateAll()
        {
            try
            {
                var exchangeRate = await GetExchangeRate();
                db.openConnection();
                foreach (var item in exchangeRate)
                {
                    MySqlCommand command = new MySqlCommand("UPDATE `currencies` SET `price` = @p, `last_update` = @lu WHERE `symbols` = @s;", db.getConnection());
                    command.Parameters.Add("@s", MySqlDbType.VarChar).Value = item.Key;
                    command.Parameters.Add("@p", MySqlDbType.Double).Value = item.Value;
                    command.Parameters.Add("@lu", MySqlDbType.DateTime).Value = DateTime.Now;

                    
                    if (command.ExecuteNonQuery() == 1)
                        continue;
                    else
                        throw new Exception();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
            finally { db.closeConnection(); }
        }

        public static async Task<Dictionary<string, double>> GetExchangeRate()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(ApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<CurrencyData>(jsonResponse);

                    return data.Rates;
                }
                else
                {
                    throw new Exception("Failed to retrieve exchange rate.");
                }
            }
        }


    }
    public class CurrencyData
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, double> Rates { get; set; }
    }
}
