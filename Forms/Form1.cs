using F5;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ImageSteganography
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OpenEncodeFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                EncodingPath.Text = fileDialog.FileName;
                EncodeImageName.Text = fileDialog.SafeFileName;
            }
        }

        private void OpenDecodeFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                DecodingPath.Text = fileDialog.FileName;
                DecodeImageName.Text = fileDialog.SafeFileName;
            }
        }

        private void EncodeButton_Click(object sender, EventArgs e)
        {
            Embed.Run(EncodingPath.Text, EncodingMessage.Text);

            //DataEmbedderLSB dataEmbedderLSB = new DataEmbedderLSB();
            //if (dataEmbedderLSB.Enocode(EncodingPath.Text, EncodingMessage.Text))
            //{
            //    AddToConsole("Succesfully encoded the image");
            //}
            //else
            //{
            //    AddToConsole("Failed to encoded the image");
            //}
        }

        private void DecodeButton_Click(object sender, EventArgs e)
        {
            DecodingMessage.Text = Extract.Run(DecodingPath.Text);

            //DataEmbedderLSB dataEmbedderLSB = new DataEmbedderLSB();

            //if (dataEmbedderLSB.Decode(DecodingPath.Text, out string message))
            //{
            //    AddToConsole("Succesfully decoded the image");
            //    DecodingMessage.Text = message;
            //}
            //else
            //{
            //    AddToConsole("An error occured while decoding the image");
            //}
        }

        private void AddToConsole(string message)
        {
            ImageConsole.Text = message;
        }
    }
}