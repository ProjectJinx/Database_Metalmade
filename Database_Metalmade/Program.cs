using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Data.Sqlite;
using static Bogus.DataSets.Name;

namespace Database_Metalmade
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateDB();
            Creator c = new Creator();
            c.CreateUsers(100);
            Auswertung();
            Console.ReadLine();
        }

        static void CreateTable(string table, List<Tuple<string, string>> columns)
        {
            string col = "";
            foreach(Tuple<string,string> t in columns)
            {
                if (table == "team")
                {
                    if (t.Item1 == "team")
                        col += t.Item1 + " " + t.Item2 + " PRIMARY KEY " + ",";
                    else
                        col += t.Item1 + " " + t.Item2 + ",";
                }
                else if(table == "einw")
                    col += t.Item1 + " " + t.Item2 + ",";
                else
                {
                    if (t.Item1 == "ma")
                        col += t.Item1 + " " + t.Item2 + " PRIMARY KEY " + ",";
                    else
                        col += t.Item1 + " " + t.Item2 + ",";
                }
            }
            col = col.Remove(col.Length - 1);
            using (var connection = new SqliteConnection("Data Source=metalmade.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1})", table, col);
                command.ExecuteNonQuery();

            }
        }


        static void CreateDB()
        {
            List<Tuple<string, string>> ma = new List<Tuple<string, string>>
            {
                Tuple.Create("ma", "varchar(255)"),
                Tuple.Create("nachname","varchar(255)"),
                Tuple.Create("vorname","varchar(255)"),
            };
            CreateTable("ma", ma);

            List<Tuple<string, string>> ma_restur = new List<Tuple<string, string>>
            {
                Tuple.Create("ma", "varchar(255)"),
                Tuple.Create("resturlaub", "int")
            };
            CreateTable("ma_restur", ma_restur);

            List<Tuple<string, string>> ma_team = new List<Tuple<string, string>>
            {
                Tuple.Create("ma", "varchar(255)"),
                Tuple.Create("team", "varchar(255)")
            };
            CreateTable("ma_team", ma_team);

            List<Tuple<string, string>> team = new List<Tuple<string, string>>
            {
                Tuple.Create("team", "varchar(255)"),
                Tuple.Create("teamleiter", "varchar(255)")
            };
            CreateTable("team", team);

            List<Tuple<string, string>> einw = new List<Tuple<string, string>>
            {
                Tuple.Create("ms", "varchar(255)"),
                Tuple.Create("ma", "varchar(255)")
            };
            CreateTable("einw", einw);
            Console.WriteLine("Datenbank erfolgreich erstellt");

        }

        static void Auswertung()
        {
            Reader rd = new Reader();
            Console.WriteLine("Folgende Mitarbeiter haben eine Einweisung für eine Stanze erhalten:");
            string request=
                @"SELECT DISTINCT ma.nachname, ma.vorname, ma_restur.resturlaub
	                FROM einw, ma, ma_restur
	                WHERE einw.ma=ma.ma 
	                AND einw.ma=ma_restur.ma
	                AND einw.ms='Stz2'
	                OR einw.ma=ma.ma 
	                AND einw.ma=ma_restur.ma
	                AND einw.ms='Stz1'";

            Console.Write(rd.Read(request));

            Console.WriteLine("Mitarbeiter mit den wenigsten Resturlauben:");
            request =
                @"	SELECT ma.nachname, ma.vorname, ma_restur.resturlaub
	                    FROM ma, ma_restur
	                    WHERE ma.ma=ma_restur.ma
	                    ORDER BY ma_restur.resturlaub
	                    ASC
	                    LIMIT 1;";
            Console.Write(rd.Read(request));

            Console.WriteLine("Anzahl der Mitarbeiter, die weniger als 24 Resturlaubstage haben:");
            request =
                @"SELECT COUNT(ma.nachname) AS Anzahl
	                FROM ma, ma_restur
	                WHERE ma.ma=ma_restur.ma
	                AND ma_restur.resturlaub<24";
            Console.Write(rd.Read(request));

            Console.WriteLine("O.g. Mitarbeiter inklusive Vor- und Nachnamen, sortiert nach Resturlaub:");
            request =
                @"SELECT DISTINCT ma.nachname, ma.vorname, ma_restur.resturlaub, ma_team.team, team.teamleiter
                    FROM einw, ma, ma_restur, ma_team, team
                    WHERE einw.ma=ma.ma 
                    AND einw.ma=ma_restur.ma
                    AND einw.ma=ma_team.ma
                    AND ma_team.team=team.team
                    AND einw.ms='Stz2'
                    OR einw.ma=ma.ma 
                    AND einw.ma=ma_restur.ma
                    AND einw.ma=ma_team.ma
                    AND ma_team.team=team.team
                    AND einw.ms='Stz1'
                    ORDER BY ma_restur.resturlaub
                    ASC;";
            Console.Write(rd.Read(request));


        }

    }
}
