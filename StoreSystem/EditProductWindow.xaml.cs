using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
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
    /// Interaction logic for EditProductWindow.xaml
    /// </summary>
    public partial class EditProductWindow : Window
    {
        string oldpName = null;
        MainWindow mainWindow = null;
        Product product = null;
        public EditProductWindow()
        {
            InitializeComponent();
            refresh("");
        }

        public EditProductWindow(MainWindow window, Product product)
        {
            InitializeComponent();
            mainWindow = window;
            this.product = product;
            pId.Content = product.รหัสสินค้า;
            name.Text = product.ชื่อสินค้า;
            detial.Text = product.รายละเอียด;
            unit.Text = product.หน่วยนับ;
            len.Text = product.คงเหลือ.ToString();
            price.Text = product.ราคาขาย.ToString();
            oldpName = name.Text;
            refresh(product.ประเภท);
        }

        private void refresh(string value)
        {
            type.Items.Clear();
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM product_tb where pId = "+ pId.Content;
                SQLiteDataReader sdr = command.ExecuteReader();
                sdr.Read();
                cost.Text = sdr.GetDouble(5).ToString();
                sdr.Close();
                command.CommandText = "SELECT * FROM type_tb";
                sdr = command.ExecuteReader();

                while (sdr.Read())
                {
                    type.Items.Add(sdr.GetString(1));
                }
                sdr.Close();
                conn.Close();
            }
            if (type.Items.Count > 0)
            {
                type.SelectedValue = value;
            }
        }

        private void editProduct(object sender, RoutedEventArgs e)
        {
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                string pName = name.Text,
                       pDetail = detial.Text,
                       pUnit = unit.Text,
                       pType = type.Text;
                Int32 pLen = Convert.ToInt32(len.Text);
                double pCost = Convert.ToDouble(cost.Text),
                      pPrice = Convert.ToDouble(price.Text);

                if(pLen < 0)
                {
                    pLen = 0;
                }
                conn.Open();
                var command = conn.CreateCommand();
                if (pName != oldpName)
                {
                    command.CommandText = "SELECT COUNT(*) FROM product_tb WHERE pName = '" + pName + "';";
                    Int32 rowCount = Convert.ToInt32(command.ExecuteScalar());
                    if (rowCount > 0)
                    {
                        MessageBox.Show("มีสินค้านี้ในระบบเเล้ว");
                        conn.Close();
                    }
                }
                command.CommandText = "UPDATE product_tb SET pName = '" + pName + "' , pDetail = '" + pDetail + "' , pLen = '" + pLen + "' , pUnit = '"
                    + pUnit + "' , pCost = '" + pCost + "' , pPrice = '" + pPrice + "' , typeId = '" + pType + "' WHERE pId ='" + product.รหัสสินค้า + "'";
                command.ExecuteNonQuery();
                MessageBox.Show("อัพเดตสินค้าสำเร็จ");
                conn.Close();

                if (mainWindow != null)
                {
                    mainWindow.reloadTable();
                }
            }
        }
        private void incProductLen(object sender, RoutedEventArgs e)
        {
            Int32 pLen = Convert.ToInt32(len.Text) + 1;
            len.Text = pLen.ToString();
        }

        private void decProductLen(object sender, RoutedEventArgs e)
        {
            Int32 Len = Convert.ToInt32(len.Text);
            if (Len > 0)
            {
                Int32 pLen = Len - 1;
                len.Text = pLen.ToString();
            }
        }
    }
}
