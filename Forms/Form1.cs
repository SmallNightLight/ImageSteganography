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
            dataEmbedderLSB.Enocode(EncodingPath.Text, EncodingMessage.Text, out string resultMessage);
            AddToConsole(resultMessage);
        }

        private void DecodeButton_Click(object sender, EventArgs e)
        {
            DataEmbedderLSB dataEmbedderLSB = new DataEmbedderLSB();

            dataEmbedderLSB.Decode(DecodingPath.Text, out string message, out string resultMessage);
            AddToConsole(resultMessage);
            DecodingMessage.Text = message;
        }

        private void AddToConsole(string message)
        {
            ImageConsole.Text = message;
        }
    }
}