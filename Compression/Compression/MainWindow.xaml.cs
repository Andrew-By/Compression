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
        public Archiver Arc = new Archiver();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Arc;
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

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            string text = File.ReadAllText(FilePathTextBox.Text);
            Arc.LoadText(text);
            FileContentsTextBox.Text = text;
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            Arc.Compress();
            Arc.WriteToFile(FilePathTextBox.Text + ".wsa");
        }

        private void ReadFileButton_Click(object sender, RoutedEventArgs e)
        {
            Arc.ReadFromFile(FilePathTextBox.Text);
            FileContentsTextBox.Text = Arc.Decompress();
        }
    }
}
