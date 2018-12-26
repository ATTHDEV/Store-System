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
    /// Interaction logic for ChangeNameStoreWindow.xaml
    /// </summary>
    public partial class ChangeNameStoreWindow : Window
    {
        MainWindow main;
        public ChangeNameStoreWindow()
        {
            InitializeComponent();
            init();
        }

        public ChangeNameStoreWindow(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            init();
        }

        private void init()
        {
            storeName_textbox.Text = main.StoreName;
            storeName_address.Text = main.StoreAddress;
            storeTexll_textbox.Text = main.StoreTell;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                var name = storeName_textbox.Text;
                var address = storeName_address.Text;
                var tell = storeTexll_textbox.Text;
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM store_name_tb;";
                Int32 rowCount = Convert.ToInt32(command.ExecuteScalar());
                if (rowCount <= 0)
                {
                    command.CommandText = "INSERT INTO store_name_tb(id,name,address,tell) values('0123456789','" + name + "','" + address + "','" + tell + "')";
                    command.ExecuteNonQuery();
                    MessageBox.Show("เเก้ไขชื่อร้านสำเร็จ");
                    conn.Close();
                    main.initStoreName();
                }
                else
                {
                    command.CommandText = "UPDATE store_name_tb SET name = '" + name + "' , address = '" + address + "' , tell = '" + tell + "' WHERE id ='0123456789'";
                    command.ExecuteNonQuery();
                    MessageBox.Show("เเก้ไขชื่อร้านสำเร็จ");
                    conn.Close();
                    main.initStoreName();
                }
            }
        }
    }
}
