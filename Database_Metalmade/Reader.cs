using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database_Metalmade
{
    class Reader
    {
        SqliteConnection connection;
        public Reader()
        {
            using (connection = new SqliteConnection("Data Source=metalmade.db"))
            {
                connection.Open();
            }
        }

        public string Read(string select)
        {
            connection.Open();
            string result = "";
            var command = connection.CreateCommand();
            command.CommandText =select;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var data = reader.GetString(0);
                    result += data + "\n";
                }
            }
            return result;
        }
    }
}
