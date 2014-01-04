using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.SQLite;
namespace ircbot
{
    class Database
    {
        SQLiteConnection con;
        public Database()
        {
            con = new SQLiteConnection("Data Source=test.db");
        }
        public void checkUser(string user)
        {
            con.Open();
            SQLiteCommand com = new SQLiteCommand(con);
            com.CommandText = "SELECT * from autoop where `name` = @user;";
            com.Parameters.Add(new SQLiteParameter("@user", user));
            var read = com.ExecuteReader();
            if (read.HasRows)
            {
                read.Read();
                string level = read.GetString(2);
                UserLevel lvl = (UserLevel) Enum.Parse(typeof(UserLevel), level, true);
                Console.WriteLine(lvl);
            }
            con.Close();
        }
        public static void M1ain(string[] args)
        {

            //    foreach (var i in ic.Commands)
              //  {
                  //  Console.WriteLine(i.Key);
                //}
            //}
         //   new Database().checkUser("alex");
            Console.ReadKey();
        }
    }
}
