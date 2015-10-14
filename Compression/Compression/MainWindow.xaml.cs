using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Compression.Models;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Compression
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ByteStats Stats = new ByteStats();
        public Byte[] FileContents;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Stats;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result == true)
            {
                FilePathTextBox.Text = dialog.FileName;
            }
        }

        private void Process_text()
        {
            Stats.Clear();
            FileContents = File.ReadAllBytes(FilePathTextBox.Text);
            foreach (var b in FileContents)
                Stats.Add(b);
            Stats.UpdateFrequency();
            Stats.Span();
        }

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            Process_text();

            FileContentsTextBox.Text = Encoding.UTF8.GetString(FileContents);
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            String text = String.Empty;
            switch (CharsetComboBox.SelectedIndex)
            {
                case 1:
                    text = Encoding.GetEncoding("KOI8-R").GetString(FileContents);
                    break;
                case 2:
                    text = Encoding.GetEncoding("iso-8859-5").GetString(FileContents);
                    break;
                case 3:
                    text = Encoding.GetEncoding(866).GetString(FileContents);
                    break;
                case 4:
                    text = Encoding.GetEncoding("windows-1251").GetString(FileContents);
                    break;
                default:
                    Encoding.UTF8.GetString(FileContents);
                    break;
            }

            FileContentsTextBox.Text = text;
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            var codes = from s in Stats.Bytes select new ByteCode(s.Byte, s.Code);
            Archieve arc = new Archieve(codes.ToList());

            using (FileStream stream = File.Create(@"C:\Users\andre\data"))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, arc);
                FileContentsTextBox.Text = stream.Length.ToString();
            }
        }
    }
}
