using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreSystem
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        MainWindow mainWindow = null;
        public AddProductWindow()
        {
            InitializeComponent();
            refresh();
        }

        public AddProductWindow(MainWindow window)
        {
            InitializeComponent();
            mainWindow = window;
            refresh();
        }

        private void refresh()
        {

            type.Items.Clear();
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM type_tb";
                SQLiteDataReader sdr = command.ExecuteReader();

                while (sdr.Read())
                {
                    type.Items.Add(sdr.GetString(1));
                }

                sdr.Close();
                conn.Close();
            }
            if (type.Items.Count > 0)
            {
                type.SelectedIndex = 0;
            }
            len.Text = "1";
            cost.Text = "0";
            price.Text = "0";
        }



        private void AddProduct(object sender, RoutedEventArgs e)
        {
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                string pId = id.Text,
                       pName = name.Text,
                       pDetail = detial.Text,
                       pUnit = unit.Text,
                       pType = type.Text;
                if(len.Text == "")
                {
                    MessageBox.Show("กรุณากรอกจำนวนให้ถูกต้อง", "Warrning !");
                    return;
                }
                else if(Convert.ToInt32(len.Text)< 0)
                {
                    MessageBox.Show("กรุณากรอกจำนวนให้ถูกต้อง", "Warrning !");
                    return;
                }
                Int32 pLen = Convert.ToInt32(len.Text);
                double pCost = Convert.ToDouble(cost.Text),
                      pPrice = Convert.ToDouble(price.Text);
                if(pId == "")
                {
                    MessageBox.Show("กรุณากรอกรหัสสินค้า", "Warrning !");
                    return;
                }else if (pName == "")
                {
                    MessageBox.Show("กรุณากรอกชื่อสินค้า", "Warrning !");
                    return;
                }else if (pUnit == "")
                {
                    MessageBox.Show("กรุณากรอกหน่วยนับ", "Warrning !");
                    return;
                }else if (pCost <= 0)
                {
                    MessageBox.Show("กรุณากรอกต้นทุนให้ถูกต้อง", "Warrning !");
                    return;
                }else if (pCost <= 0)
                {
                    MessageBox.Show("กรุณากรอกราคาขายให้ถูกต้อง", "Warrning !");
                    return;
                }


                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM product_tb WHERE pName = '" + pName + "';";
                Int32 rowCount = Convert.ToInt32(command.ExecuteScalar());
                if (rowCount > 0)
                {
                    MessageBox.Show("มีสินค้านี้ในระบบเเล้ว");
                    conn.Close();
                }
                else
                {
                    command.CommandText = "INSERT INTO product_tb(pId,pName,pDetail,pLen,pUnit,pCost,pPrice,typeId) " +
                        "values ('" + pId + "','" + pName + "','" + pDetail + "','" + pLen + "','" + pUnit + "','" + pCost + "','" + pPrice + "','" + pType + "')";
                    command.ExecuteNonQuery();
                    MessageBox.Show("เพิ่มสินค้าสำเร็จ");
                    conn.Close();
                    if (mainWindow != null)
                    {
                        mainWindow.reloadTable();
                        this.Close();
                    }
                }
            }
        }

        private void autoGenarateId(object sender, RoutedEventArgs e)
        {
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                conn.Open();
                Int32 rowCount;
                var pId = "";
                do
                {
                    pId = mainWindow.randomGenerate(13, 0);
                    var command = conn.CreateCommand();
                    command.CommandText = "SELECT COUNT(*) FROM product_tb WHERE pId = '" + pId + "';";
                    rowCount = Convert.ToInt32(command.ExecuteScalar());
                } while (rowCount > 0);
                conn.Close();

                id.Text = pId;
            }
        }
    }
}
