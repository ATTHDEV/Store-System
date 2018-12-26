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

namespace StoreSystem
{
    /// <summary>
    /// Interaction logic for CreatedBarcodeWindow.xaml
    /// </summary>
    public partial class CreatedBarcodeWindow : Window
    {
        MainWindow mainWindow = null;
        public CreatedBarcodeWindow()
        {
            InitializeComponent();
        }

        public CreatedBarcodeWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            len.Text = "1";
        }

        private void incCopy(object sender, RoutedEventArgs e)
        {
            Int32 pLen = Convert.ToInt32(len.Text) + 1;
            len.Text = pLen.ToString();
        }

        private void decCopy(object sender, RoutedEventArgs e)
        {
            Int32 pLen = Convert.ToInt32(len.Text) - 1;
            len.Text = pLen > 1 ? pLen.ToString() : "1";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "PDF File | *.pdf",
                FileName = "allBarcode01",
                ValidateNames = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string currentDir = Environment.CurrentDirectory;
                var directory = new DirectoryInfo(Path.GetFullPath(Path.Combine(currentDir, @"resource\THSarabunNew.ttf")));
                var bf = BaseFont.CreateFont(directory.ToString(), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                //BaseFont bf = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                var fnt = new Font(bf, 16, Font.NORMAL);
                Document document = new Document(PageSize.A4, 10, 10, 10, 10);
                try
                {
                    var writer = PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    document.Open();
                    PdfContentByte content = writer.DirectContent;
                    PdfPTable table = new PdfPTable(3);
                    table.SetTotalWidth(new float[] { 170, 170, 170});
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.SpacingAfter = 5;
                    table.DefaultCell.Border = Rectangle.NO_BORDER;
         
                    var barcode = new Barcode128();
                    barcode.CodeType = Barcode.CODE128;
                    barcode.ChecksumText = true;
                    //barcode.GenerateChecksum = true;
                    var conn = DBHelper.Instance.getConnection();
                    if (conn != null)
                    {
                        conn.Open();
                        var command = conn.CreateCommand();
                        command.CommandText = "SELECT * FROM product_tb";
                        SQLiteDataReader sdr = command.ExecuteReader();
                        var p = new Paragraph();
                        int num = 0;
                        while (sdr.Read())
                        {
                            Paragraph paragraph = new Paragraph(sdr.GetString(1) + "\n",fnt);
                            paragraph.Alignment = Element.ALIGN_CENTER;
                            barcode.Code = sdr.GetString(0);
                            Image image = barcode.CreateImageWithBarcode(content, null, null);
                         
                            PdfPCell cell = new PdfPCell();
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.Padding = 10;
                            cell.Border  = Rectangle.NO_BORDER;
                            cell.AddElement(paragraph);
                            cell.AddElement(image);

                            table.AddCell(cell);
                            num++;
                        }
                        sdr.Close();
                        conn.Close();
                        int check = num % 3;
                        for (int i = 3 - check; i > 0; i--)
                        {
                            table.AddCell("");
                        }
                        document.Add(table);
                    }
                    document.Close();
                    Process.Start(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาดบางอย่าง กรุณาลองใหม่อีกครั้ง " + ex.StackTrace);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (id.Text == "") return;
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "PDF File | *.pdf",
                FileName = "barcode01",
                ValidateNames = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string currentDir = Environment.CurrentDirectory;
                var directory = new DirectoryInfo(Path.GetFullPath(Path.Combine(currentDir, @"resource\THSarabunNew.ttf")));
                var bf = BaseFont.CreateFont(directory.ToString(), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                var fnt = new Font(bf, 16, Font.NORMAL);
                Document document = new Document(PageSize.A4, 10, 10, 10, 10);
                try
                {
                    var writer = PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    document.Open();
                    PdfContentByte content = writer.DirectContent;
                    PdfPTable table = new PdfPTable(3);
                    table.SetTotalWidth(new float[] { 170, 170, 170 });
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.SpacingAfter = 30;
                    table.DefaultCell.Border = Rectangle.NO_BORDER;

                    var barcode = new Barcode128();
                    barcode.CodeType = Barcode.CODE128;
                    barcode.ChecksumText = true;
                    //barcode.GenerateChecksum = true;
                    var conn = DBHelper.Instance.getConnection();
                    if (conn != null)
                    {
                        conn.Open();
                        var command = conn.CreateCommand();
                        command.CommandText = "SELECT * FROM product_tb where pId = "+id.Text;
                        SQLiteDataReader sdr = command.ExecuteReader();
                        sdr.Read();
                        var p = new Paragraph();
                        int num = Convert.ToInt32(len.Text);
                        for(int i = 0; i < num; i++)
                        {
                            Paragraph paragraph = new Paragraph(sdr.GetString(1) + "\n", fnt);
                            paragraph.Alignment = Element.ALIGN_CENTER;
                            barcode.Code = sdr.GetString(0);
                            Image image = barcode.CreateImageWithBarcode(content, null, null);

                            PdfPCell cell = new PdfPCell();
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.Padding = 10;
                            cell.Border = Rectangle.NO_BORDER;
                            cell.AddElement(paragraph);
                            cell.AddElement(image);

                            table.AddCell(cell);
                        }
                        sdr.Close();
                        conn.Close();
                        int check = num % 3;
                        for (int i = 3 - check; i > 0; i--)
                        {
                            table.AddCell("");
                        }
                        document.Add(table);
                    }
                    document.Close();
                    Process.Start(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาดบางอย่าง กรุณาลองใหม่อีกครั้ง " + ex.StackTrace);
                }
            }
        }
    }
}
