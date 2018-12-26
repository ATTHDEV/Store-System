
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using iTextSharp.text.html.simpleparser;
using iTextSharp.tool.xml;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.ComponentModel;

namespace StoreSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] month =
        {
            "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม",
        };

        string[] day =
        {
            "อาทิตย์","จันทร์", "อังคาร","พุธ","พฤหัสบดี","ศุกร์","เสาร์",
        };

        string storeName = "XXXXX";
        string storeAddress = "";
        string storeTell = "";

        public string StoreName
        {
            get => storeName;
            set => storeName = value;
        }
        public string StoreAddress
        {
            get => storeAddress;
            set => storeAddress = value;
        }

        public string StoreTell
        {
            get => storeTell;
            set => storeTell = value;
        }

        public MainWindow()
        {
            InitializeComponent();
            DBHelper.Instance.connect();
            DBHelper.Instance.createdTable();
            initStoreName();
            refreshType();
            reloadTable();
            initReport();
        }

        public void initReport()
        {
            seeDay.IsChecked = true;
        }

        public void initStoreName()
        {

            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM store_name_tb ";
                Int32 rowCount = Convert.ToInt32(command.ExecuteScalar());
                conn.Close();
                if (rowCount > 0)
                {
                    conn.Open();
                    command = conn.CreateCommand();
                    command.CommandText = "SELECT * FROM store_name_tb";
                    SQLiteDataReader sdr = command.ExecuteReader();
                    sdr.Read();
                    storeName = sdr.GetString(1);
                    storeAddress = sdr.GetString(2);
                    storeTell = sdr.GetString(3);
                    sdr.Close();
                    conn.Close();
                }
            }
        }

        public void refreshReport(int mode)
        {

            if (mode == 0)
            {
                if (calenderReport.SelectedDate.HasValue)
                {
                    DateTime date = calenderReport.SelectedDate.Value;
                    ReportData.setDay(date.ToString("yyyy-MM-dd"));
                    int d = Convert.ToInt32(date.DayOfWeek);
                    int m = Convert.ToInt32(date.Month) - 1;
                    headerDate.Content = "ข้อมูลรายได้วัน" + day[d].ToString() + " ที่ " +
                        date.Day + " " + month[m] + " ปี " + date.Year;
                }
            }
            else if (mode == 1)
            {
                string y = calenderReport.DisplayDate.Year.ToString();
                string m = string.Format("{0:00}", calenderReport.DisplayDate.Month);
                string date = string.Format("{0}-{1}", y, m);
                ReportData.setMonth(date);
                headerDate.Content = "ข้อมูลรายได้เดือน " + month[Convert.ToInt32(m) - 1];
            }
            else if (mode == 2)
            {
                string year = calenderReport.DisplayDate.Year.ToString();
                ReportData.setYear(year);
                headerDate.Content = "ข้อมูลรายได้ปี " + year;
            }
            if (reportData.Items.Count > 0)
                ReportData.tables.Clear();
            sumProfit.Content = "";
            reportData.ItemsSource = ReportData.GetReportData();
            if (reportData.Items.Count > 0)
            {
                double profit = calculateSumProfit();
                sumProfit.Content = String.Format("{0:n}", profit);
            }
        }

        public void refreshType()
        {

            type_cb.Items.Clear();
            type_cb.Items.Add("ดูสินค้าทั้งหมด");
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM type_tb";
                SQLiteDataReader sdr = command.ExecuteReader();

                while (sdr.Read())
                {
                    type_cb.Items.Add(sdr.GetString(1));
                }
                sdr.Close();
                conn.Close();
            }
            if (type_cb.Items.Count > 0)
            {
                type_cb.SelectedIndex = 0;
                string type = type_cb.Text;
                if (type.Equals("ดูสินค้าทั้งหมด"))
                {
                    Product.setCondition("SELECT * FROM product_tb");
                }
                else
                {
                    Product.setCondition("SELECT * FROM product_tb where typeid = '" + type + "'");
                }
                dataProduct.ItemsSource = Product.GetProducts();
            }
        }

        private void showAddProductWindow(object sender, RoutedEventArgs e)
        {
            AddProductWindow addProductWindow = new AddProductWindow(this);
            addProductWindow.Show();
        }

        private void exitProgram(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void changeStroreName(object sender, RoutedEventArgs e)
        {
            ChangeNameStoreWindow changeNameStoreWindow = new ChangeNameStoreWindow(this);
            changeNameStoreWindow.Show();
        }

        private void contactMe(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("หากมีข้อสงใส หรือ ข้อผิดพลาดเกิดขึ้น ท่านสามารถเเจ้งผู้พัฒนาได้ที่ \natthawutcpe@gmail.com ขอบคุณครับ", "ขอบคุณที่ใช้งานโปรเเกรม");
        }

        private void showEditProductWindow(object sender, RoutedEventArgs e)
        {
            if (dataProduct.SelectedIndex != -1)
            {
                var rowData = (Product)dataProduct.SelectedItem;
                EditProductWindow editProductWindow = new EditProductWindow(this, rowData);
                editProductWindow.Show();
            }
        }

        private void showConfirmDeleteDialog(object sender, RoutedEventArgs e)
        {
            if (dataProduct.SelectedItems.Count > 0)
            {
                var result = MessageBox.Show("คุณเเน่ใจว่าจะลบ ใช่หรือไม่ ?", "ยืนยันการลบ", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.Yes)
                {
                    var conn = DBHelper.Instance.getConnection();
                    if (conn != null)
                    {
                        conn.Open();
                        var command = conn.CreateCommand();
                        if (dataProduct.SelectedItems.Count > 1)
                        {
                            var rowData = dataProduct.SelectedItems;
                            foreach (Product p in rowData)
                            {
                                string pId = p.รหัสสินค้า;
                                command.CommandText = "Delete from product_tb WHERE pId ='" + pId + "'";
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            var rowData = (Product)dataProduct.SelectedItem;
                            string pId = rowData.รหัสสินค้า;
                            command.CommandText = "Delete from product_tb WHERE pId ='" + pId + "'";
                            command.ExecuteNonQuery();
                        }
                        MessageBox.Show("ลบสินค้าสำเร็จ");
                        conn.Close();
                        reloadTable();
                    }
                }
            }
        }

        private void showUpdateTypeWindow(object sender, RoutedEventArgs e)
        {
            UpdateTypeProduct updateTypeWindow = new UpdateTypeProduct(this);
            updateTypeWindow.Show();
        }

        public void reloadTable()
        {
            dataProduct.ItemsSource = Product.GetProducts();
        }

        public string randomGenerate(int len, int mode)
        {
            Random rnd = new Random();
            int maxSize = len;
            char[] chars = new char[30];
            string a;
            if (mode == 0)
                a = "1234567890";
            else
                a = "1234567890abcdefghijklmnopqrstuvwxyz";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private void searchProductById(object sender, RoutedEventArgs e)
        {
            string id = idText.Text;
            if (id.Equals(""))
            {
                MessageBox.Show("กรุณากรอกรหัสสินค้า", "Warning !");
            }
            else
            {
                var conn = DBHelper.Instance.getConnection();
                if (conn != null)
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = "SELECT COUNT(*) FROM product_tb WHERE pId = '" + id + "';";
                    Int32 rowCount = Convert.ToInt32(command.ExecuteScalar());
                    conn.Close();
                    if (rowCount > 0)
                    {
                        Product.setCondition("SELECT * FROM product_tb where pId = '" + id + "'");
                        dataProduct.ItemsSource = Product.GetProducts();
                    }
                    else
                    {
                        MessageBox.Show("ไม่พบสินค้านี้");
                    }
                }
            }
        }

        private void searchProductByType(object sender, RoutedEventArgs e)
        {
            if (type_cb.SelectedIndex > -1)
            {

                string type = type_cb.Items.GetItemAt(type_cb.SelectedIndex).ToString();
                if (type.Equals("ดูสินค้าทั้งหมด"))
                {
                    Product.setCondition("SELECT * FROM product_tb");
                }
                else
                {
                    Product.setCondition("SELECT * FROM product_tb where typeId = '" + type + "'");
                }
                dataProduct.ItemsSource = Product.GetProducts();
            }
        }

        private void createBarcode(object sender, RoutedEventArgs e)
        {
            CreatedBarcodeWindow createdBarcodeWindow = new CreatedBarcodeWindow(this);
            createdBarcodeWindow.Show();
        }

        public string save(string custName, string custAddr)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string sId = randomGenerate(5, 0);
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM sell_tb WHERE pDate = '" + today + "';";
                Int32 countDay = Convert.ToInt32(command.ExecuteScalar());
                if (countDay > 0)
                {
                    command.CommandText = "SELECT MAX(sId) FROM sell_tb WHERE pDate = '" + today + "';";
                    var sdr = command.ExecuteReader();
                    sdr.Read();
                    Int32 rowCount = Convert.ToInt32(sdr.GetString(0));
                    sdr.Close();
                    sId = String.Format("{0:00000}", rowCount + 1);
                }
                else
                {
                    sId = String.Format("{0:00000}",00001);
                }
                foreach (var sp in SellProduct.chart)
                {
                    command.CommandText = "INSERT INTO sell_tb(sId,pId,pLen,pVat,pDate,customerName,customerAddress) " +
                       "values ('" + sId + "','" + sp.Value.รหัสสินค้า + "','" + sp.Value.จำนวน + "','" + Convert.ToDouble(vat.Text) + "','" + today + "','" + custName + "','" + custAddr + "')";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE product_tb SET pLen = pLen - " + sp.Value.จำนวน + " WHERE pId ='" + sp.Value.รหัสสินค้า + "' and pLen > 0 ";
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
            return sId;
        }

        private void saveSell(object sender, RoutedEventArgs e)
        {
            double sum = Convert.ToDouble(sumProductPrice.Content);
            Int32 priceInput = textPriceInput.Text != "" ? Convert.ToInt32(textPriceInput.Text) : 0;
            if (dataSellProduct.Items.Count > 0 && (priceInput - sum) >= 0)
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.PageRangeSelection = PageRangeSelection.AllPages;
                printDialog.UserPageRangeEnabled = true;
                bool? doPrint = printDialog.ShowDialog();
                if (doPrint != true)
                {
                    return;
                }
                string rand = save("", "");
                bool isAddr = storeAddress != "";
                bool isTell = storeTell != "";
                string date = DateTime.Now.ToString("dd_MM_yyyy") + "_id#" + rand;

                string currentDir = Environment.CurrentDirectory;
                var directory = new DirectoryInfo(Path.GetFullPath(Path.Combine(currentDir, @"..\..\resource\THSarabunNew.ttf")));
                var family = new System.Windows.Media.FontFamily(directory.ToString());

                float offset = 150;
                if (isAddr) offset += 35;
                if (isTell) offset += 20;
                float width = 250;
                float heifht = offset + SellProduct.chart.Count * 30;

                var fd = new System.Windows.Documents.FlowDocument();
                fd.PageWidth = width;
                fd.PageHeight = heifht;

                var t = CreateTextBox(storeName, family, 14);
                t.TextAlignment = TextAlignment.Center;
                t.FontWeight = FontWeights.Bold;
                fd.Blocks.Add(new System.Windows.Documents.BlockUIContainer(t));

                if (isAddr)
                {
                    t = CreateTextBox(storeAddress, family, 12);
                    t.TextAlignment = TextAlignment.Left;
                    fd.Blocks.Add(new System.Windows.Documents.BlockUIContainer(t));
                }

                if (isTell)
                {
                    t = CreateTextBox(storeTell, family, 12);
                    t.TextAlignment = TextAlignment.Left;
                    fd.Blocks.Add(new System.Windows.Documents.BlockUIContainer(t));
                }

                t = CreateTextBox("ใบเสร็จรับเงิน/ใบกำกับภาษีอย่างย่อ", family, 12);
                t.Margin = new Thickness(0, 5, 0, 0);
                t.TextAlignment = TextAlignment.Center;
                fd.Blocks.Add(new System.Windows.Documents.BlockUIContainer(t));

                var g = new Grid();
                g.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    MinWidth = 35,
                    MaxWidth = 35,
                });
                g.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    MinWidth = 105,
                    MaxWidth = 105,
                });
                g.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    MinWidth = 75,
                    MaxWidth = 75,
                });

                int i = 0;
                foreach (var sp in SellProduct.chart)
                {
                    g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    var t1 = CreateTextBox(sp.Value.จำนวน.ToString(), family, 10);
                    t1.SetValue(Grid.ColumnProperty, 0);
                    t1.SetValue(Grid.RowProperty, i);
                    g.Children.Add(t1);

                    var t2 = CreateTextBox(sp.Value.ชื่อสินค้า.ToString(), family, 10);
                    t2.SetValue(Grid.ColumnProperty, 1);
                    t2.SetValue(Grid.RowProperty, i);
                    g.Children.Add(t2);

                    var t3 = CreateTextBox(String.Format("{0:n}", sp.Value.รวมราคา), family, 10);
                    t3.TextAlignment = TextAlignment.Right;
                    t3.SetValue(Grid.ColumnProperty, 2);
                    t3.SetValue(Grid.RowProperty, i);
                    g.Children.Add(t3);
                    i++;
                }

                fd.Blocks.Add(new System.Windows.Documents.BlockUIContainer(g));

                var totalGrid = new Grid();
                totalGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    MinWidth = 70,
                    MaxWidth = 70,
                });
                totalGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    MinWidth = 72.5,
                    MaxWidth = 72.5,
                });
                totalGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    MinWidth = 72.5,
                    MaxWidth = 72.5,
                });
                var size = 10;
                totalGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                var T = createGridTextbox("Total + Vat "+vat.Text+"%", family, size, 0, 0, 0);
                T.SetValue(Grid.ColumnSpanProperty,2);
                totalGrid.Children.Add(T);
                totalGrid.Children.Add(createGridTextbox(String.Format("{0:n}", calculateSumPrice()), family, size, 0, 2, 2));

                totalGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                totalGrid.Children.Add(createGridTextbox("Cash/Change", family, size, 1, 0, 0));
                totalGrid.Children.Add(createGridTextbox(String.Format("{0:n}", Convert.ToDouble(textPriceInput.Text)), family, size, 1, 1, 2));
                totalGrid.Children.Add(createGridTextbox(String.Format("{0:n}", Convert.ToDouble(textReturnPrice.Content.ToString())), family, size, 1, 2, 2));

                totalGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                totalGrid.Children.Add(createGridTextbox("#" + rand, family, size, 2, 0, 0));
                totalGrid.Children.Add(createGridTextbox(DateTime.Now.ToString("dd/MM/yyyy"), family, size, 2, 1, 2));
                totalGrid.Children.Add(createGridTextbox(DateTime.Now.ToString("HH:mm"), family, size, 2, 2, 2));

                fd.Blocks.Add(new System.Windows.Documents.BlockUIContainer(totalGrid));

                System.Windows.Documents.IDocumentPaginatorSource idpSource = fd;
                printDialog.PrintDocument(idpSource.DocumentPaginator, date);

                MessageBox.Show("ทำรายการเสร็จสิ้น", "Success !");
                refreshChart();
            }
        }

        public void refreshChart()
        {
            SellProduct.chart.Clear();
            dataSellProduct.ItemsSource = SellProduct.GetSellProducts();
            sumProductPrice.Content = "0";
            textPriceInput.Text = "";
            textReturnPrice.Content = "0";
            reloadTable();
            refreshReport(0);
        }


        public TextBlock createGridTextbox(string text, System.Windows.Media.FontFamily family, int fontSize, int row, int col, int alig)
        {
            var t = CreateTextBox(text, family, fontSize);
            t.TextAlignment = alig == 0 ? TextAlignment.Left : alig == 1 ? TextAlignment.Center : TextAlignment.Right;
            t.SetValue(Grid.ColumnProperty, col);
            t.SetValue(Grid.RowProperty, row);
            return t;
        }

        public TextBlock CreateTextBox(string text, System.Windows.Media.FontFamily family, int fontSize)
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

        public System.Windows.Documents.Paragraph createParagraph(string text, int col, int fontsize)
        {
            var p = new System.Windows.Documents.Paragraph();
            p.FontSize = fontsize;
            if (col == 0)
            {
                p.TextAlignment = TextAlignment.Left;
            }
            else if (col == 1)
            {
                p.TextAlignment = TextAlignment.Center;
            }
            else if (col == 2)
            {
                p.TextAlignment = TextAlignment.Right;
            }
            p.Inlines.Add(text);
            return p;
        }

        public System.Windows.Documents.TableCell createCell(System.Windows.Documents.Paragraph p)
        {
            var cell = new System.Windows.Documents.TableCell();
            cell.Blocks.Add(p);
            return cell;
        }

        private void saveSellAndExport(object sender, RoutedEventArgs e)
        {
            double sum = Convert.ToDouble(sumProductPrice.Content);
            Int32 priceInput = textPriceInput.Text != "" ? Convert.ToInt32(textPriceInput.Text) : 0;
            if (dataSellProduct.Items.Count > 0 && (priceInput - sum) >= 0)
            {
                ExportFullBinWindows fullBinWindows = new ExportFullBinWindows(this);
                fullBinWindows.Show();
            }

        }

        private void addToChart(object sender, RoutedEventArgs e)
        {
            string id = barcodeId.Text;
            if (!id.Equals(""))
            {
                queryChart(id, 1, true);
            }
        }

        public void queryChart(string id, int number, bool isAppenNumber)
        {
            var conn = DBHelper.Instance.getConnection();
            if (conn != null)
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM product_tb WHERE pId = '" + id + "';";
                Int32 rowCount = Convert.ToInt32(command.ExecuteScalar());
                if (rowCount > 0)
                {
                    command.CommandText = "SELECT* FROM product_tb WHERE pId = '" + id + "'";
                    var sdr = command.ExecuteReader();
                    sdr.Read();
                    int len = sdr.GetInt32(3);
                    pNameAdd.Text = sdr.GetString(1);
                    productPrice.Content = sdr.GetDouble(6);
                    if (SellProduct.chart.ContainsKey(id))
                    {
                        var sp = SellProduct.chart[id];
                        if (isAppenNumber)
                        {
                            if (sp.จำนวน < len)
                            {
                                int num = sp.จำนวน + number;
                                sp.จำนวน = num;
                                sp.รวมราคา = num * sp.ราคาขาย;
                            }
                            else
                            {
                                MessageBox.Show("ขออภัย สินค้าที่ท่านต้องการไม่เหลือเเล้ว", "Warning !");
                            }
                        }
                        else
                        {
                            if (number < len)
                            {
                                sp.จำนวน = number;
                                sp.รวมราคา = number * sp.ราคาขาย;
                            }
                            else
                            {
                                sp.จำนวน = len;
                                sp.รวมราคา = len * sp.ราคาขาย;
                            }
                        }
                    }
                    else
                    {
                        if (len > 0)
                        {
                            SellProduct sellProduct = new SellProduct();
                            sellProduct.รหัสสินค้า = id;
                            sellProduct.ชื่อสินค้า = sdr.GetString(1);
                            sellProduct.รายละเอียด = sdr.GetString(2);
                            sellProduct.จำนวน = 1;
                            sellProduct.หน่วยนับ = sdr.GetString(4);
                            sellProduct.ราคาขาย = sdr.GetDouble(6);
                            sellProduct.รวมราคา = sellProduct.ราคาขาย;

                            SellProduct.chart.Add(sellProduct.รหัสสินค้า, sellProduct);
                        }
                        else
                        {
                            MessageBox.Show("ขออภัย สินค้าที่ท่านต้องการไม่เหลือเเล้ว", "Warning !");
                        }

                    }

                    dataSellProduct.ItemsSource = SellProduct.GetSellProducts();
                    sumProductPrice.Content = calculateSumPrice();

                    sdr.Close();
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("ไม่พบสินค้านี้", "Warning !");
                    conn.Close();
                }
            }
        }

        public double calculateSumPrice()
        {
            double sum = 0;
            foreach (var sp in SellProduct.chart)
            {
                sum += sp.Value.รวมราคา;
            }

            Int32 v = 0;
            if (vat.Text != "")
            {
                v = Convert.ToInt32(vat.Text);
                if (v < 0)
                {
                    vat.Text = "0";
                    v = 0;
                }
            }
            else
            {
                vat.Text = "0";
            }
            sum += v * sum / 100;
            return sum;
        }

        private void clearChart(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("คุณเเน่ใจว่าจะล้างรายการ ใช่หรือไม่ ?", "ยืนยันการล้างรายการ", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                SellProduct.chart.Clear();
                dataSellProduct.ItemsSource = SellProduct.GetSellProducts();
                sumProductPrice.Content = "0";
            }
        }

        private void deleteRowChart(object sender, RoutedEventArgs e)
        {
            if (dataSellProduct.SelectedItems.Count > 0)
            {
                if (dataSellProduct.SelectedItems.Count > 1)
                {
                    var result = MessageBox.Show("คุณเเน่ใจว่าจะลบรายการที่เลือก ใช่หรือไม่ ?", "ยืนยันการลบรายการ", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (result == MessageBoxResult.Yes)
                    {
                        var rowData = dataSellProduct.SelectedItems;
                        foreach (SellProduct r in rowData)
                        {
                            var sp = SellProduct.chart[r.รหัสสินค้า];
                            if (sp.จำนวน > 1)
                            {
                                int num = sp.จำนวน - 1;
                                sp.จำนวน = num;
                                sp.รวมราคา = num * sp.ราคาขาย;
                            }
                            else
                            {
                                SellProduct.chart.Remove(r.รหัสสินค้า);
                            }
                        }
                        sumProductPrice.Content = calculateSumPrice();
                        dataSellProduct.ItemsSource = SellProduct.GetSellProducts();
                        if (dataSellProduct.Items.Count < 1)
                        {
                            sumProductPrice.Content = "0";
                            textReturnPrice.Content = "0";
                        }
                    }
                }
                else
                {
                    var rowData = (SellProduct)dataSellProduct.SelectedItem;
                    var sp = SellProduct.chart[rowData.รหัสสินค้า];
                    if (sp.จำนวน > 1)
                    {
                        int num = sp.จำนวน - 1;
                        sp.จำนวน = num;
                        sp.รวมราคา = num * sp.ราคาขาย;
                    }
                    else
                    {
                        SellProduct.chart.Remove(rowData.รหัสสินค้า);
                        if (dataSellProduct.Items.Count < 1)
                        {
                            sumProductPrice.Content = "0";
                            textReturnPrice.Content = "0";
                        }
                    }
                    sumProductPrice.Content = calculateSumPrice();
                    dataSellProduct.ItemsSource = SellProduct.GetSellProducts();
                }
            }
        }

        private void textPriceInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var sum = Convert.ToInt64(sumProductPrice.Content);
                var input = textPriceInput.Text != "" ? Convert.ToInt64(textPriceInput.Text) : -1;
                var v = Convert.ToInt32(vat.Text);
                if (sum >= 0 && input >= 0 && v >= 0)
                {
                    var price = sum - input;
                    textReturnPrice.Content = (-1 * price).ToString();
                }

            }
        }

        private void seeInDay(object sender, RoutedEventArgs e)
        {
            calenderReport.DisplayMode = CalendarMode.Month;
            calenderReport.SelectedDate = DateTime.Now;
        }

        private void seeInMount(object sender, RoutedEventArgs e)
        {
            calenderReport.DisplayMode = CalendarMode.Year;
        }

        private void seeInYear(object sender, RoutedEventArgs e)
        {
            calenderReport.DisplayMode = CalendarMode.Decade;
        }

        private void ExportData(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "PDF File | *.pdf",
                FileName = "report01",
                ValidateNames = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {

                string currentDir = Environment.CurrentDirectory;
                var directory = new DirectoryInfo(Path.GetFullPath(Path.Combine(currentDir, @"..\..\resource\THSarabunNew.ttf")));
                var bf = BaseFont.CreateFont(directory.ToString(), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                var fnt = new Font(bf, 16, Font.NORMAL);
                Document document = new Document(PageSize.A4, 10, 10, 10, 10);
                try
                {
                    var writer = PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));

                    document.Open();

                    string date = headerDate.Content.ToString();
                    //header.Add(storeName);

                    Paragraph headerName = new Paragraph();
                    headerName.SpacingBefore = 40f;
                    headerName.Font = fnt;
                    headerName.Add(storeName);
                    headerName.Alignment = Element.ALIGN_CENTER;
                    document.Add(headerName);

                    Paragraph hDate = new Paragraph();
                    hDate.SpacingAfter = 20f;
                    hDate.SpacingBefore = 20f;
                    hDate.Font = fnt;
                    hDate.Add(date);
                    hDate.Alignment = Element.ALIGN_CENTER;
                    document.Add(hDate);

                    Paragraph paragraphTable1 = new Paragraph();
                    paragraphTable1.SpacingAfter = 10f;

                    PdfPTable table = new PdfPTable(5);
                    table.TotalWidth = 515f;
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    float[] tbWidths = new float[5];
                    tbWidths[0] = 40f;
                    tbWidths[1] = 220f;
                    tbWidths[2] = 75f;
                    tbWidths[3] = 90f;
                    tbWidths[4] = 90f;
                    table.SetWidths(tbWidths);
                    table.LockedWidth = true;
                    PdfPCell cell = new PdfPCell();
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.FixedHeight = 75;
                    fnt.Size = 20;
                    cell.Phrase = new Phrase("ลำดับ", fnt);
                    table.AddCell(cell);
                    cell.Phrase = new Phrase("ชื่อสินค้า", fnt);
                    table.AddCell(cell);
                    cell.Phrase = new Phrase("จำนวนที่ขาย", fnt);
                    table.AddCell(cell);
                    cell.Phrase = new Phrase("เงินจากการขาย", fnt);
                    table.AddCell(cell);
                    cell.Phrase = new Phrase("กำไร(บาท)", fnt);
                    table.AddCell(cell);

                    cell.FixedHeight = 40;
                    fnt.Size = 16;
                    int i = 1;
                    foreach (var rp in ReportData.tables)
                    {
                        cell.Phrase = new Phrase(i.ToString(), fnt);
                        table.AddCell(cell);
                        cell.Phrase = new Phrase(rp.Value.ชื่อสินค้า.ToString(), fnt);
                        table.AddCell(cell);
                        cell.Phrase = new Phrase(rp.Value.จำนวนที่ขายได้.ToString(), fnt);
                        table.AddCell(cell);
                        cell.Phrase = new Phrase(rp.Value.เงินที่ได้.ToString(), fnt);
                        table.AddCell(cell);
                        cell.Phrase = new Phrase(rp.Value.กำไรที่ได้.ToString(), fnt);
                        table.AddCell(cell);
                        i++;
                    }

                    PdfPCell headerTotal = new PdfPCell();
                    headerTotal.FixedHeight = 40;
                    headerTotal.VerticalAlignment = Element.ALIGN_MIDDLE;
                    headerTotal.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerTotal.Colspan = 4;
                    headerTotal.Phrase = new Phrase("รายได้สุทธิรวมทั้งสิ้น", fnt);
                    table.AddCell(headerTotal);

                    PdfPCell total = new PdfPCell();
                    total.FixedHeight = 40;
                    total.VerticalAlignment = Element.ALIGN_MIDDLE;
                    total.HorizontalAlignment = Element.ALIGN_CENTER;
                    total.Phrase = new Phrase(calculateSumProfit().ToString(), fnt);
                    table.AddCell(total);

                    paragraphTable1.Add(table);
                    document.Add(paragraphTable1);
                    document.Close();
                    Process.Start(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาดบางอย่าง กรุณาลองใหม่อีกครั้ง", "Error !");
                }
            }
        }

        private void clickDate(object sender, SelectionChangedEventArgs e)
        {
            if (seeDay.IsChecked == true)
            {
                refreshReport(0);
            }
        }

        private double calculateSumProfit()
        {
            double sum = 0;
            foreach (var rp in ReportData.tables)
            {
                sum += rp.Value.กำไรที่ได้;
            }
            return sum;
        }

        private void changMode(object sender, EventArgs e)
        {
            if (seeDay.IsChecked == true)
            {
                refreshReport(0);
                calenderReport.DisplayMode = CalendarMode.Month;
            }
            else if (seeMount.IsChecked == true)
            {
                refreshReport(1);
                calenderReport.DisplayMode = CalendarMode.Year;
            }
            else if (seeYear.IsChecked == true)
            {
                refreshReport(2);
                calenderReport.DisplayMode = CalendarMode.Decade;
            }
        }

        private void mouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void change_vat(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (vat.Text == "")
                {
                    vat.Text = "7";
                }
                else
                {
                    var v = Convert.ToInt32(vat.Text);
                    if (v < 0)
                    {
                        vat.Text = "7";
                    }
                }
                sumProductPrice.Content = calculateSumPrice();
            }
        }

        private void changeNumber(object sender, MouseButtonEventArgs e)
        {
            if (dataSellProduct.SelectedIndex != -1)
            {
                var rowData = (SellProduct)dataSellProduct.SelectedItem;
                ChangeNumberWindow editProductWindow = new ChangeNumberWindow(this, rowData);
                editProductWindow.Show();
            }

        }


        private void exportPreviousBill_func(object sender, RoutedEventArgs e)
        {

            ExportPreviousBillWindow previousBillWindow = new ExportPreviousBillWindow(this);
            previousBillWindow.Show();
        }


    }
}