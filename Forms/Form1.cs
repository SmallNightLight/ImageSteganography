namespace ImageSteganography
{
    public partial class Form1 : Form
    {
        public string EncodeFilePath, DecodeFilePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void OpenEncodeFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                EncodeFilePath = fileDialog.FileName;
                EncodingPath.Text = fileDialog.FileName;
                EncodeImageName.Text = fileDialog.SafeFileName;
            }
        }

        private void OpenDecodeFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                DecodeFilePath = fileDialog.FileName;
                DecodingPath.Text = fileDialog.FileName;
                DecodeImageName.Text = fileDialog.SafeFileName;
            }
        }
    }
}
