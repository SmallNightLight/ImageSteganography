namespace ImageSteganography
{
    public partial class Form1 : Form
    {
        private DataEmbedder _dataEmbedder;

        public Form1()
        {
            InitializeComponent();
            AlgorithmDropDown.SelectedItem = "LSB";
            _dataEmbedder = new DataEmbedderLSB();
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
            if (_dataEmbedder == null) return;

            _dataEmbedder.Encode(EncodingPath.Text, EncodingMessage.Text, out string resultMessage);
            AddToConsole(resultMessage);
        }

        private void DecodeButton_Click(object sender, EventArgs e)
        {
            if (_dataEmbedder == null) return;

            _dataEmbedder.Decode(DecodingPath.Text, out string message, out string resultMessage);
            AddToConsole(resultMessage);
            DecodingMessage.Text = message;
        }

        private void AddToConsole(string message)
        {
            ImageConsole.Text = message;
        }

        private void AlgorithmDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            object? selector = AlgorithmDropDown.SelectedItem;

            if (selector == null) return;

            string? algorithm = selector.ToString();
            switch (algorithm)
            {
                case "LSB":
                    _dataEmbedder = new DataEmbedderLSB();
                    break;
                case "LSBExclude01":
                    _dataEmbedder = new DataEmbedderLSBExcl01();
                    break;
                case "PeakLSB":
                    _dataEmbedder = new DataEmbedderPeakLSB();
                    break;
                case "PickC":
                    _dataEmbedder = new DataEmbedderPickC();
                    break;
            }
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            string result = "";

            for(int i = 0; i < 64; i++)
            {
                _dataEmbedder = new DataEmbedderPickC();
                (_dataEmbedder as DataEmbedderPickC).SetIndex(i);

                _dataEmbedder.Encode(EncodingPath.Text, EncodingMessage.Text, out string resultMessage);

                result += $"\n{i}: " + resultMessage;
            }

            result += "END";
        }
    }
}