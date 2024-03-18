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
            DataEmbedderLSB dataEmbedderLSB = new DataEmbedderLSB();
            if (dataEmbedderLSB.Enocode(EncodingPath.Text, EncodingMessage.Text))
            {
                //Succesfully encoded the image
            }
        }

        private void DecodeButton_Click(object sender, EventArgs e)
        {
            DataEmbedderLSB dataEmbedderLSB = new DataEmbedderLSB();
            
            if (dataEmbedderLSB.Decode(EncodingPath.Text, out string message))
            {
                //Succesfully decoded the image
                DecodingMessage.Text = message;
            }
            else
            {
                DecodingMessage.Text = "An error occured while decoding the image";
            }
        }
    }
}