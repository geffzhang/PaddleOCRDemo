using Microsoft.Win32;
using OpenCvSharp;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.KnownModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaddleSharpDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            if(ofd.ShowDialog() is true)
            {
                var file = ofd.FileName;
                FileTextBox.Text = file;
            }
        }

        private void ParseButton_Click(object sender, RoutedEventArgs e)
        {
            var file = FileTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(file))
            {
                return ;
            }
            Parse(file);
        }

        private void ResultTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Handled = true;
            }
        }

        private void ResultTextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            object text = e.Data.GetData(DataFormats.FileDrop);
            if(text is string[] files)
            {
                var file = files.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(file))
                {
                    return;
                }
                FileTextBox.Text = file;
                Parse(file);
            }
        }

        private async void Parse(string file)
        {
            if (!File.Exists(file))
            {
                MessageBox.Show("File Not Exist");
                return;
            }

            await Task.Run(async () =>
            {
                try
                {
                    await KnownOCRModel.PPOcrV2.EnsureAll();
                    using PaddleOcrAll all = new PaddleOcrAll(KnownOCRModel.PPOcrV2.RootDirectory, KnownOCRModel.PPOcrV2.KeyPath);
                    using Mat src = Cv2.ImRead(file);
                    var text = all.Run(src).Text;
                    // Console.WriteLine(text);

                    this.Dispatcher.Invoke(() =>
                    {
                        ResultTextBox.Text = text;
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Parse Fail. {ex.Message}", "Error");
                }
            });

        }

    }
}
