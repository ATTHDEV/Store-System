using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StoreSystem
{
    class ReportData
    {
        private string name;
        private int len;
        private double money;
        private double moneyProfit;


        public string ชื่อสินค้า { get => name; set => name = value; }
        public int จำนวนที่ขายได้ { get => len; set => len = value; }
        public double เงินที่ได้ { get => money; set => money = value; }
        public double กำไรที่ได้ { get => moneyProfit; set => moneyProfit = value; }

        public static string condition = "SELECT* FROM sell_tb where pDate = ''";

        public static void setDay(string date)
        {
            condition = "SELECT* FROM sell_tb where strftime('%Y-%m-%d', pDate) = '" + date + "'";
        }

        public static void setMonth(string date)
        {
            condition = "SELECT* FROM sell_tb where strftime('%Y-%m', pDate) = '" + date + "'";
        }

        public static void setYear(string date)
        {
            condition = "SELECT* FROM sell_tb where strftime('%Y', pDate) = '" + date + "'";
        }

        public static Dictionary<string, ReportData> tables = new Dictionary<string, ReportData>();

        public static ObservableCollection<ReportData> GetReportData()
        {
            var repordData = new ObservableCollection<ReportData>();
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = condition;
                SQLiteDataReader sdr = command.ExecuteReader();
                while (sdr.Read())
                {
                    string id = sdr.GetString(1);
                    var commandProduct = conn.CreateCommand();
                    commandProduct.CommandText = "SELECT* FROM product_tb WHERE pId = '" + id + "'";
                    var productReader = commandProduct.ExecuteReader();
                    productReader.Read();
                    if (tables.ContainsKey(id))
                    {
                        var rp = tables[id];
                        int num = rp.จำนวนที่ขายได้ + sdr.GetInt32(2);
                        rp.จำนวนที่ขายได้ = num;
                        rp.เงินที่ได้ = num * productReader.GetDouble(6);
                        rp.กำไรที่ได้ = rp.เงินที่ได้ - num * productReader.GetDouble(5);
                    }
                    else
                    {
                        var reportRow = new ReportData();
                        reportRow.name = productReader.GetString(1);
                        reportRow.จำนวนที่ขายได้ = sdr.GetInt32(2);
                        reportRow.เงินที่ได้ = reportRow.จำนวนที่ขายได้ * productReader.GetDouble(6);
                        reportRow.กำไรที่ได้ = reportRow.เงินที่ได้ - reportRow.จำนวนที่ขายได้ * productReader.GetDouble(5);
                        tables.Add(id, reportRow);
                    }
                }
                sdr.Close();
                conn.Close();

                foreach (var rp in tables)
                {
                    repordData.Add(rp.Value);
                }

            }
            return repordData;
        }
    }
}
