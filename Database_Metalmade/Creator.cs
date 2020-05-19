using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Database_Metalmade
{
    class Creator
    {
        private readonly static string MA = "ma, nachname, vorname",
           RESTUR = "ma, resturlaub",
           MA_TEAM = "ma, team",
           TEAM = "team, teamleiter",
           EINW = "ms, ma";

        static string[] MS = { "Sae1", "Stz1", "Stz2", "Frs1", "Frs2" };
        static string[] TEAMS = { "A", "B", "C" };

        private List<string> ma = new List<string>();
        Random rnd = new Random();

        public Creator()
        {

        }

        public void CreateUsers(int anzahl)
        {
            for (int i = 0; i < anzahl; i++)
            {
                var arbeiter = new Faker<Person>()
                    .RuleFor(u => u.vorname, f => f.Name.FirstName())
                    .RuleFor(u => u.nachname, f => f.Name.LastName())
                    .RuleFor(u => u.urlaub, f => f.Random.Number(1, 30));

                var a = arbeiter.Generate();
                a.vorname = a.vorname.Replace("'", "");
                a.nachname = a.nachname.Replace("'", "");
                a.einw = CreateEinweisungen();
                string kurz = a.vorname.Remove(1) + a.nachname.Remove(1);
                while (ma.Contains(kurz))
                {
                    int cut = 2;
                    kurz = a.vorname.Remove(1) + a.nachname.Remove(cut);
                    cut++;
                    if (cut.Equals(a.nachname.Length - 1))
                        kurz = a.vorname + a.nachname;
                }
                a.ma = kurz;
                ma.Add(kurz);
                if (i < 3)
                {
                    a.team = TEAMS[i];
                    FillDB("team", TEAM, "'" + TEAMS[i] + "'" + ", " + "'" + a.ma + "'");
                }
                else
                    a.team = TEAMS[rnd.Next(0, 3)];

                FillDB("ma_team", MA_TEAM, Fix(a.ma) + ", " + Fix(a.team));
                FillDB("ma", MA, Fix(a.ma) + ", " + Fix(a.nachname) + ", " + Fix(a.vorname));
                FillDB("ma_restur", RESTUR, Fix(a.ma) + ", " + a.urlaub);
                foreach (string einweisung in a.einw)
                    FillDB("einw", EINW, Fix(einweisung) + ", " + Fix(a.ma));
            }

        }
        private string Fix(string data)
        {
            return "'" + data + "'";
        }

        static string[] CreateEinweisungen()
        {
            Random rnd = new Random();
            int anz = rnd.Next(1, 4);
            string[] einw = new string[anz];
            int count = rnd.Next(0, MS.Length);
            for (int i = 0; i < anz; i++)
            {
                if (count > MS.Length -1 )
                    count = 0;
                einw[i] = MS[count];
                count++;
            }
            return einw;
        }


        static void FillDB(string table, string inserts, string data)
        {
            using (var connection = new SqliteConnection("Data Source=metalmade.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = string.Format("INSERT INTO {0} ( {1} ) VALUES( {2} )", table, inserts, data);
                command.ExecuteNonQuery();

            }
        }
    }
}
