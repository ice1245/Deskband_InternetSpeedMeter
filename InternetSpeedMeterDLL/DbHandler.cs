using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace InternetSpeedMeter
{
    class DbHandler
    {

        SQLiteConnection con;
        SQLiteCommand cmd;
        SQLiteDataReader dr;


        public void CreateDatabaseAndTable()
        {
            
            if (!File.Exists("MyDatabase.sqlite"))
            {

                

                //    SQLiteConnection.CreateFile("MyDatabase.sqlite");

                //    string sql = @"CREATE TABLE MyRecord(
                //                   ID INTEGER PRIMARY KEY AUTOINCREMENT ,
                //                   Date           TEXT      NOT NULL,
                //                   DataUsed      TEXT      NOT NULL
                //                );";
                //    con = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
                //    con.Open();
                //    cmd = new SQLiteCommand(sql, con);
                //    cmd.ExecuteNonQuery();
                //    con.Close();

            }
            else
            {
                con = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            }
        }
        public void AddData(string date, string data)
        {
            cmd = new SQLiteCommand();
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "insert into MyRecord(Date,DataUsed) values ('" + date + "','" + data + "')";
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public void UpdateData(int id, string data)
        {
            cmd = new SQLiteCommand();
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "update MyRecord set DataUsed='" + data + "' where ID=" + id + "";
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public string GetTodayDownloadedData(int id )
        {
           string data  = "";
            cmd = new SQLiteCommand("Select * From MyRecord where id='" + id + "'", con);
            con.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                data = dr[2].ToString();
            }
            con.Close();
            return data;
        }
        public void SelectData()
        {
            int counter = 0;
            cmd = new SQLiteCommand("Select *From MyRecord", con);
            con.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                counter++;
                Console.WriteLine(dr[0] + " : " + dr[1] + " " + dr[2]);

            }
            con.Close();
        }
        public int SearchDataByDate(string data)
        {
            int id = 0;
            int counter = 0;
            cmd = new SQLiteCommand("Select * From MyRecord where Date='" + data + "'", con);
            con.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                counter++;
                id = Int32.Parse(dr[0].ToString());

            }
            con.Close();
            return id;
        }
    }
}
