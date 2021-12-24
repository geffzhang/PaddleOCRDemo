using OpenCvSharp;
using ScreenCapturerNS;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.KnownModels;
using System.Data;

namespace PaddleSharpWinFormDemo
{
    public partial class MainForm : Form
    {
        Bitmap bmp;
        string fileName;

        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            var imagebyte = File.ReadAllBytes(ofd.FileName);
            bmp = new Bitmap(new MemoryStream(imagebyte));
            pictureBox1.BackgroundImage = bmp;
            fileName = ofd.FileName;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            if (bmp == null)
                return;

            Parse(fileName);

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

                    this.Invoke(() =>
                    {
                        this.richTextBox1.Text = text;
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