using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace StoreSystem
{
    class DBHelper
    {
        private static DBHelper instance = null;
        private static SQLiteConnection conn;

        private DBHelper()
        {

        }

        public static DBHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBHelper();
                }
                return instance;
            }
        }


        public void connect()
        {
            string currentDir = Environment.CurrentDirectory;
            var directory = new DirectoryInfo(Path.GetFullPath(Path.Combine(currentDir, @"DATA\")));
            if (!Directory.Exists(directory.ToString()))
            {
                Directory.CreateDirectory(directory.ToString());
            }
            conn = new SQLiteConnection(@"Data Source="+ directory.ToString() + "database.db;");
        }

        public SQLiteConnection getConnection()
        {
            return conn;
        }

        public SQLiteCommand getCommand()
        {
            return conn.CreateCommand();
        }

        public void createdTable()
        {
            if (conn != null)
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "" +
                    "CREATE TABLE IF NOT EXISTS store_name_tb(" +
                        "id char(10) not null PRIMARY KEY," +
                        "name  Varchar(100) not null," +
                        "address  Varchar(200) not null," +
                        "tell  Varchar(10) not null" +
                    ")";
                command.ExecuteNonQuery();

                command.CommandText = "" +
                    "CREATE TABLE IF NOT EXISTS type_tb(" +
                        "typeId char(15) not null PRIMARY KEY," +
                        "name  Varchar(100) not null" +
                    ")";
                command.ExecuteNonQuery();

                command.CommandText = "" +
                     "CREATE TABLE IF NOT EXISTS product_tb(" +
                         "pId char(13) not null PRIMARY KEY," +
                         "pName  Varchar(100) not null," +
                         "pDetail  Varchar(200)," +
                         "pLen integer not null," +
                         "pUnit Varchar(20) not null," +
                         "pCost REAL not null," +
                         "pPrice REAL not null," +
                         "typeId char(15) not null," +
                         "FOREIGN KEY (typeId) REFERENCES type_tb(typeId)" +
                     ")";
                command.ExecuteNonQuery();


                command.CommandText = "" +
                     "CREATE TABLE IF NOT EXISTS sell_tb(" +
                         "sId char(5) not null," +
                         "pId char(13) not null," +
                         "pLen integer not null," +
                         "pVat REAL not null," +
                         "pDate DATE not null," +
                         "customerName  Varchar(100)," +
                         "customerAddress  Varchar(200)," +
                         "PRIMARY KEY (sId, pId , pDate)," +
                         "FOREIGN KEY (pId) REFERENCES product_tb(pId)" +
                     ")";
                command.ExecuteNonQuery();


                conn.Close();
            }
        }
    }
}
