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
    /// Interaction logic for UpdateTypeProduct.xaml
    /// </summary>
    public partial class UpdateTypeProduct : Window
    {
        MainWindow mainWindow = null;
        public UpdateTypeProduct()
        {
            InitializeComponent();
            refresh(0);
        }

        public UpdateTypeProduct(MainWindow window)
        {
            InitializeComponent();
            mainWindow = window;
            refresh(0);
        }
        private void refresh(int index)
        {

            tpyeProduct_cb.Items.Clear();
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM type_tb";
                SQLiteDataReader sdr = command.ExecuteReader();

                while (sdr.Read())
                {
                    tpyeProduct_cb.Items.Add(sdr.GetString(1));
                }
                sdr.Close();
                conn.Close();
            }
            if (tpyeProduct_cb.Items.Count > 0)
            {
                tpyeProduct_cb.SelectedIndex = index;
            }
            else
            {
                type_textBox2.Text = "";
            }
        }

        private void addType(object sender, RoutedEventArgs e)
        {
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                string typeName = type_textBox.Text;
                if(typeName == "")
                {
                    MessageBox.Show("กรุณากรอกประเภทสินค้า", "Error !");
                    return;
                }
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM type_tb WHERE name = '" + typeName + "';";
                Int32 rowCount = Convert.ToInt32(command.ExecuteScalar());
                if (rowCount > 0)
                {
                    MessageBox.Show("มีประเภทสินค้านี้ในระบบเเล้ว");
                    conn.Close();
                }
                else
                {
                    string typeId = DateTime.Now.ToString("yyyyMMddHHmmss");
                    command.CommandText = "INSERT INTO type_tb(typeId, name) values ('" + typeId + "', '" + typeName + "')";
                    command.ExecuteNonQuery();
                    MessageBox.Show("เพิ่มประเภทสินค้าสำเร็จ");
                    conn.Close();
                    refresh(tpyeProduct_cb.Items.Count);
                    if (mainWindow != null)
                    {
                        mainWindow.refreshType();
                    }
                }
            }
        }

        private void onUpdateTypeCB(object sender, SelectionChangedEventArgs e)
        {
            int index = tpyeProduct_cb.SelectedIndex;
            if (index != -1)
            {
                type_textBox2.Text = tpyeProduct_cb.Items.GetItemAt(index).ToString();
            }
        }

        private void editType(object sender, RoutedEventArgs e)
        {
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                string typeName = type_textBox2.Text;
                if (typeName == "")
                {
                    MessageBox.Show("กรุณากรอกประเภทสินค้า", "Error !");
                    return;
                }
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM type_tb WHERE name = '" + typeName + "';";
                Int32 rowCount = Convert.ToInt32(command.ExecuteScalar());
                if (rowCount > 0)
                {
                    MessageBox.Show("มีประเภทสินค้านี้ในระบบเเล้ว");
                    conn.Close();
                }
                else
                {
                    string typeSelectName = tpyeProduct_cb.Text;
                    int index = tpyeProduct_cb.SelectedIndex;
                    command.CommandText = "SELECT * FROM type_tb WHERE name = '" + typeSelectName + "';";
                    SQLiteDataReader sdr = command.ExecuteReader();
                    string editId = "";
                    while (sdr.Read())
                    {
                        editId = sdr.GetString(0);
                    }
                    sdr.Close();

                    command.CommandText = "UPDATE product_tb SET typeId ='" + typeName + "' WHERE typeId ='" + typeSelectName + "'";
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE type_tb SET name ='" + typeName + "' WHERE typeId ='" + editId + "'";
                    command.ExecuteNonQuery();

                    MessageBox.Show("เเก้ไขประเภทสินค้าสำเร็จ");
                    conn.Close();
                    refresh(index);
                    if (mainWindow != null)
                    {
                        mainWindow.refreshType();
                        mainWindow.reloadTable();
                    }
                }
            }
        }

        private void deleteType(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("คุณเเน่ใจว่าจะลบ ใช่หรือไม่ ?", "ยืนยันการลบ", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                var conn = DBHelper.Instance.getConnection();
                if (conn != null)
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    string typeSelectName = tpyeProduct_cb.Text;
                    command.CommandText = "SELECT * FROM type_tb WHERE name = '" + typeSelectName + "';";
                    SQLiteDataReader sdr = command.ExecuteReader();
                    string delId = "";
                    while (sdr.Read())
                    {
                        delId = sdr.GetString(0);
                    }
                    sdr.Close();
                    command.CommandText = "Delete from type_tb WHERE typeId ='" + delId + "'";
                    command.ExecuteNonQuery();
                    MessageBox.Show("ลบประเภทสินค้าสำเร็จ");
                    conn.Close();
                    refresh(0);
                    if (mainWindow != null)
                    {
                        mainWindow.refreshType();
                    }
                }
            }
        }
    }
}
