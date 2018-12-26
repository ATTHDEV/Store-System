using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ExportFullBinWindows.xaml
    /// </summary>
    public partial class ExportFullBinWindows : Window
    {
        MainWindow main;
        public ExportFullBinWindows(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.PageRangeSelection = PageRangeSelection.AllPages;
            printDialog.UserPageRangeEnabled = true;
            bool? doPrint = printDialog.ShowDialog();
            if (doPrint != true)
            {
                return;
            }
            string sId = main.save(customerName_textbox.Text, cusTomerAddr_textbox.Text);
            bool isAddr = main.StoreAddress != "";
            bool isTell = main.StoreName != "";
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string date2 = DateTime.Now.ToString("dd-MM-yyyy");
            string number = sId;
            string currentDir = Environment.CurrentDirectory;
            var directory = new DirectoryInfo(System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDir, @"..\..\resource\THSarabunNew.ttf")));
            var family = new FontFamily(directory.ToString());

            var fd = new FlowDocument();
            fd.ColumnWidth = fd.MaxPageWidth;
            fd.Blocks.Add(createSection(family,isAddr,isTell,number,date2,0));

            var separator = new Rectangle();
            separator.Margin = new Thickness(0, 10, 0, 0);
            separator.Stroke = new SolidColorBrush(Colors.Black);
            separator.StrokeDashArray = new DoubleCollection() { 2 };
            separator.StrokeThickness = 2;
            separator.Height = 3;
            separator.Width = double.NaN;

            var lineBlock = new BlockUIContainer(separator);
            fd.Blocks.Add(lineBlock);

            fd.Blocks.Add(createSection(family, isAddr, isTell, number, date2,20));

            IDocumentPaginatorSource idpSource = fd;
            printDialog.PrintDocument(idpSource.DocumentPaginator, date);
            MessageBox.Show("ทำรายการเสร็จสิ้น", "Success !");
            main.refreshChart();
            this.Close();
        }

        public Section createSection(FontFamily family , bool isAddr , bool isTell ,string number , string date , int ystart)
        {
            var section = new Section();
            section.Margin = new Thickness(20, 0, 20, 0);

            var t = main.CreateTextBox("ใบเสร็จรับเงินฉบับเต็ม", family, 18);
            t.Margin = new Thickness(0, ystart, 0, 0);
            t.TextAlignment = TextAlignment.Center;
            t.FontWeight = FontWeights.Bold;
            section.Blocks.Add(new BlockUIContainer(t));

            t = main.CreateTextBox(main.StoreName, family, 16);
            t.TextAlignment = TextAlignment.Left;
            t.FontWeight = FontWeights.Bold;
            section.Blocks.Add(new BlockUIContainer(t));

            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { MaxWidth = 600, MinWidth = 600 });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { MaxWidth = 50, MinWidth = 50 });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            headerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            headerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            if (isAddr)
            {
                t = main.CreateTextBox(main.StoreAddress, family, 16);
                t.TextAlignment = TextAlignment.Left;
                t.SetValue(Grid.ColumnProperty, 0);
                t.SetValue(Grid.RowProperty, 0);
                headerGrid.Children.Add(t);
            }

            if (isTell)
            {
                t = main.CreateTextBox("โทรศัพท์ " + main.StoreTell, family, 16);
                t.TextAlignment = TextAlignment.Left;
                t.SetValue(Grid.ColumnProperty, 0);
                t.SetValue(Grid.RowProperty, 1);
                headerGrid.Children.Add(t);
            }

            t = CreateTextBox("เลขที่ ", family, 16);
            t.FontWeight = FontWeights.Bold;
            t.SetValue(Grid.ColumnProperty, 1);
            t.SetValue(Grid.RowProperty, 0);
            headerGrid.Children.Add(t);

            t = CreateTextBox(number, family, 16);
            t.TextAlignment = TextAlignment.Right;
            t.SetValue(Grid.ColumnProperty, 2);
            t.SetValue(Grid.RowProperty, 0);
            headerGrid.Children.Add(t);

            t = CreateTextBox("วันที่ ", family, 16);
            t.FontWeight = FontWeights.Bold;
            t.SetValue(Grid.ColumnProperty, 1);
            t.SetValue(Grid.RowProperty, 1);
            headerGrid.Children.Add(t);

            t = CreateTextBox(date, family, 16);
            t.TextAlignment = TextAlignment.Right;
            t.SetValue(Grid.ColumnProperty, 2);
            t.SetValue(Grid.RowProperty, 1);
            headerGrid.Children.Add(t);

            section.Blocks.Add(new BlockUIContainer(headerGrid));


            var customerGrid = new Grid();
            customerGrid.Margin = new Thickness(0, 10, 0, 0);
            customerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            customerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            customerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            customerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            customerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            var t1 = CreateTextBox("ลูกค้า ", family, 16);
            t1.Margin = new Thickness(5, 0, 0, 0);
            t1.FontWeight = FontWeights.Bold;
            t1.SetValue(Grid.ColumnProperty, 0);
            t1.SetValue(Grid.RowProperty, 0);
            customerGrid.Children.Add(t1);

            t1 = CreateTextBox(customerName_textbox.Text, family, 16);
            t1.Margin = new Thickness(20, 0, 0, 0);
            t1.SetValue(Grid.ColumnProperty, 1);
            t1.SetValue(Grid.RowProperty, 0);
            customerGrid.Children.Add(t1);

            t1 = CreateTextBox("ที่อยู่ ", family, 16);
            t1.Margin = new Thickness(100, 0, 0, 0);
            t1.FontWeight = FontWeights.Bold;
            t1.SetValue(Grid.ColumnProperty, 2);
            t1.SetValue(Grid.RowProperty, 0);
            customerGrid.Children.Add(t1);

            t1 = CreateTextBox(cusTomerAddr_textbox.Text, family, 16);
            t1.Margin = new Thickness(20, 0, 0, 0);
            t1.SetValue(Grid.ColumnProperty, 3);
            t1.SetValue(Grid.RowProperty, 0);
            customerGrid.Children.Add(t1);

            section.Blocks.Add(new BlockUIContainer(customerGrid));

            var separator = new Rectangle();
            separator.Stroke = new SolidColorBrush(Colors.Black);
            separator.StrokeThickness = 2;
            separator.Height = 3;
            separator.Width = double.NaN;

            var lineBlock = new BlockUIContainer(separator);
            lineBlock.Margin = new Thickness(0, 5, 0, 0);
            section.Blocks.Add(lineBlock);

            var g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                MinWidth = 120,
                MaxWidth = 120,
            });
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                MinWidth = 310,
                MaxWidth = 310
            });
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                MinWidth = 100,
                MaxWidth = 100,
            });
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                MinWidth = 100,
                MaxWidth = 100,
            });
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                MinWidth = 100,
                MaxWidth = 100,
            });

            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            t1 = main.CreateTextBox("รหัสสินค้า", family, 13);
            t1.TextAlignment = TextAlignment.Left;
            t1.FontWeight = FontWeights.Bold;
            t1.SetValue(Grid.ColumnProperty, 0);
            t1.SetValue(Grid.RowProperty, 0);
            g.Children.Add(t1);

            t1 = main.CreateTextBox("รายการสินค้า", family, 13);
            t1.TextAlignment = TextAlignment.Center;
            t1.FontWeight = FontWeights.Bold;
            t1.SetValue(Grid.ColumnProperty, 1);
            t1.SetValue(Grid.RowProperty, 0);
            g.Children.Add(t1);

            t1 = main.CreateTextBox("จำนวน", family, 13);
            t1.FontWeight = FontWeights.Bold;
            t1.TextAlignment = TextAlignment.Right;
            t1.SetValue(Grid.ColumnProperty, 2);
            t1.SetValue(Grid.RowProperty, 0);
            g.Children.Add(t1);

            t1 = main.CreateTextBox("หน่วยละ", family, 13);
            t1.FontWeight = FontWeights.Bold;
            t1.TextAlignment = TextAlignment.Right;
            t1.SetValue(Grid.ColumnProperty, 3);
            t1.SetValue(Grid.RowProperty, 0);
            g.Children.Add(t1);

            t1 = main.CreateTextBox("จำนวนเงิน", family, 13);
            t1.FontWeight = FontWeights.Bold;
            t1.TextAlignment = TextAlignment.Right;
            t1.SetValue(Grid.ColumnProperty, 4);
            t1.SetValue(Grid.RowProperty, 0);
            g.Children.Add(t1);

            section.Blocks.Add(new BlockUIContainer(g));

            separator = new Rectangle();
            separator.Stroke = new SolidColorBrush(Colors.Black);
            separator.StrokeThickness = 2;
            separator.Height = 3;
            separator.Width = double.NaN;

            lineBlock = new BlockUIContainer(separator);
            section.Blocks.Add(lineBlock);

            g = new Grid();
            g.MinHeight = 260;
            g.MaxHeight = 260;
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                MinWidth = 120,
                MaxWidth = 120,
            });
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                MinWidth = 310,
                MaxWidth = 310
            });
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                MinWidth = 100,
                MaxWidth = 100,
            });
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                MinWidth = 100,
                MaxWidth = 100,
            });
            g.ColumnDefinitions.Add(new ColumnDefinition()
            {
                MinWidth = 100,
                MaxWidth = 100,
            });

            int i = 0;
            foreach (var sp in SellProduct.chart)
            {
                g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                t1 = main.CreateTextBox(sp.Value.รหัสสินค้า, family, 12);
                t1.SetValue(Grid.ColumnProperty, 0);
                t1.SetValue(Grid.RowProperty, i);
                g.Children.Add(t1);

                t1 = main.CreateTextBox(sp.Value.ชื่อสินค้า.ToString(), family, 12);
                t1.SetValue(Grid.ColumnProperty, 1);
                t1.SetValue(Grid.RowProperty, i);
                g.Children.Add(t1);

                t1 = main.CreateTextBox(sp.Value.จำนวน.ToString(), family, 12);
                t1.SetValue(Grid.ColumnProperty, 2);
                t1.TextAlignment = TextAlignment.Right;
                t1.Margin = new Thickness(0, 5, 5, 0);
                t1.SetValue(Grid.RowProperty, i);
                g.Children.Add(t1);

                t1 = main.CreateTextBox(String.Format("{0:n}", sp.Value.ราคาขาย), family, 12);
                t1.TextAlignment = TextAlignment.Right;
                t1.SetValue(Grid.ColumnProperty, 3);
                t1.SetValue(Grid.RowProperty, i);
                g.Children.Add(t1);

                t1 = main.CreateTextBox(String.Format("{0:n}", sp.Value.รวมราคา), family, 12);
                t1.TextAlignment = TextAlignment.Right;
                t1.SetValue(Grid.ColumnProperty, 4);
                t1.SetValue(Grid.RowProperty, i);
                g.Children.Add(t1);
                i++;
            }

            section.Blocks.Add(new BlockUIContainer(g));

            separator = new Rectangle();
            separator.Stroke = new SolidColorBrush(Colors.Black);
            separator.StrokeThickness = 2;
            separator.Height = 3;
            separator.Width = double.NaN;

            lineBlock = new BlockUIContainer(separator);
            lineBlock.Margin = new Thickness(0, 5, 0, 0);
            section.Blocks.Add(lineBlock);

            var totalGrid = new Grid();
            totalGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 580, MaxWidth = 580, });
            totalGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 70, MaxWidth = 70, });
            totalGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            totalGrid.Margin = new Thickness(0, 5, 0, 0);

            totalGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            t = main.CreateTextBox("รวมเงิน", family, 16);
            t.FontWeight = FontWeights.Bold;
            t.SetValue(Grid.ColumnProperty, 1);
            t.SetValue(Grid.RowProperty, 0);
            totalGrid.Children.Add(t);

            t = main.CreateTextBox(String.Format("{0:n}", main.calculateSumPrice()), family, 16);
            t.FontWeight = FontWeights.Bold;
            t.TextAlignment = TextAlignment.Right;
            t.SetValue(Grid.ColumnProperty, 2);
            t.SetValue(Grid.RowProperty, 0);
            totalGrid.Children.Add(t);
            section.Blocks.Add(new BlockUIContainer(totalGrid));

            var footerGrid = new Grid();
            footerGrid.Margin = new Thickness(0, 0, 0, 12.5);
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            footerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            t1 = CreateTextBox("ลงชื่อ", family, 16);
            t1.Margin = new Thickness(50, 0, 0, 0);
            t1.SetValue(Grid.ColumnProperty, 0);
            t1.SetValue(Grid.RowProperty, 0);
            footerGrid.Children.Add(t1);

            t1 = CreateTextBox("......................................", family, 16);
            t1.Margin = new Thickness(0, 0, 0, 0);
            t1.SetValue(Grid.ColumnProperty, 1);
            t1.SetValue(Grid.RowProperty, 0);
            footerGrid.Children.Add(t1);

            t1 = CreateTextBox("ลูกค้า", family, 16);
            t1.Margin = new Thickness(0, 0, 0, 0);
            t1.SetValue(Grid.ColumnProperty, 2);
            t1.SetValue(Grid.RowProperty, 0);
            footerGrid.Children.Add(t1);

            t1 = CreateTextBox("ลงชื่อ", family, 16);
            t1.Margin = new Thickness(100, 0, 0, 0);
            t1.SetValue(Grid.ColumnProperty, 3);
            t1.SetValue(Grid.RowProperty, 0);
            footerGrid.Children.Add(t1);

            t1 = CreateTextBox("......................................", family, 16);
            t1.Margin = new Thickness(0, 0, 0, 0);
            t1.SetValue(Grid.ColumnProperty, 4);
            t1.SetValue(Grid.RowProperty, 0);
            footerGrid.Children.Add(t1);

            t1 = CreateTextBox("ผู้รับเงิน", family, 16);
            t1.Margin = new Thickness(0, 0, 0, 0);
            t1.SetValue(Grid.ColumnProperty, 5);
            t1.SetValue(Grid.RowProperty, 0);
            footerGrid.Children.Add(t1);

            section.Blocks.Add(new BlockUIContainer(footerGrid));
            return section;
        }

        public TextBlock CreateTextBox(string text , FontFamily family , int fontSize)
        {
            var t = new TextBlock()
            {
                Text = text,
                FontFamily = family,
                FontSize = fontSize,
                Margin = new Thickness(5, 3, 5, 3),
                TextWrapping = TextWrapping.Wrap
            };
            return t;
        }
    }
}
