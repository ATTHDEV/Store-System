using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSystem
{
    public class Product
    {
        private string id;
        private string name;
        private string detail;
        private int len;
        private string unit;
        private double cost;
        private double price;
        private string type;
  
        public string รหัสสินค้า { get => id; set => id = value; }
        public string ชื่อสินค้า { get => name; set => name = value; }
        public string รายละเอียด { get => detail; set => detail = value; }
        public int คงเหลือ { get => len; set => len = value; }
        public string หน่วยนับ { get => unit; set => unit = value; }
        public double ราคาทุน{ get => cost; set => cost = value;}
        public double ราคาขาย { get => price; set => price = value; }
        public string ประเภท { get => type; set => type = value; }


        public static string condition = "SELECT * FROM product_tb";
        public static void setCondition(string strCondition)
        {
            condition = strCondition;
        }

        public static ObservableCollection<Product> GetProducts()
        {
            var productColl = new ObservableCollection<Product>();
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = condition;
                SQLiteDataReader sdr = command.ExecuteReader();

                while (sdr.Read())
                {
                    var product = new Product();
                    product.id = sdr.GetString(0);
                    product.name = sdr.GetString(1);
                    product.detail = sdr.GetString(2);
                    product.len = sdr.GetInt32(3);
                    product.unit = sdr.GetString(4);
                    product.cost = sdr.GetDouble(5);
                    product.price = sdr.GetDouble(6);
                    product.type = sdr.GetString(7);
                    productColl.Add(product);
                }
                sdr.Close();
                conn.Close();
            }
            return productColl;
        }

        public void setConfition(string condition)
        {

        }
    }
}
