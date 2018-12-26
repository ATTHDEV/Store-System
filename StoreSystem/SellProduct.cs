using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSystem
{
    public class SellProduct
    {
        private string id;
        private string name;
        private string detail;
        private int len;
        private string unit;
        private double price;
        private double sumPrice;

        public string รหัสสินค้า { get => id; set => id = value; }
        public string ชื่อสินค้า { get => name; set => name = value; }
        public string รายละเอียด { get => detail; set => detail = value; }
        public int จำนวน { get => len; set => len = value; }
        public string หน่วยนับ { get => unit; set => unit = value; }
        public double ราคาขาย { get => price; set => price = value; }
        public double รวมราคา { get => sumPrice; set => sumPrice = value; }

        public static string condition = "SELECT* FROM sell_tb where sId = '123456'";
        public static void setProductId(string pId)
        {
            condition = "SELECT* FROM product_tb where pId = '" + pId + "'";
        }

        public static Dictionary<string, SellProduct> chart = new Dictionary<string, SellProduct>();

        public static ObservableCollection<SellProduct> GetSellProducts()
        {
            var sellProductColl = new ObservableCollection<SellProduct>(); 
            foreach (var sp in chart)
            {
                sellProductColl.Add(sp.Value);
            }
            return sellProductColl;
        }
    }
}
