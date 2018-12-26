using System;
using System.Collections.Generic;
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
    /// Interaction logic for ChangeNumberWindow.xaml
    /// </summary>
    public partial class ChangeNumberWindow : Window
    {
        private MainWindow main;
        private SellProduct product;
        public ChangeNumberWindow()
        {
            InitializeComponent();
        }

        public ChangeNumberWindow(MainWindow main, SellProduct product)
        {
            InitializeComponent();
            this.main = main;
            this.product = product;
            productName.Content = product.ชื่อสินค้า;
            len.Text = product.จำนวน.ToString();
        }

        private void init()
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Int32 Len = Convert.ToInt32(len.Text);
            if(Len < 0)
            {
                Len = 0;
            }
            main.queryChart(product.รหัสสินค้า, Len , false);
            this.Close();
        }

        private void incProductLen(object sender, RoutedEventArgs e)
        {
            Int32 pLen = Convert.ToInt32(len.Text) + 1;
            len.Text = pLen.ToString();
        }

        private void decProductLen(object sender, RoutedEventArgs e)
        {
            Int32 Len = Convert.ToInt32(len.Text);
            if (Len > 1)
            {
                Int32 pLen = Len - 1;
                len.Text = pLen.ToString();
            }
        }

    }
}
