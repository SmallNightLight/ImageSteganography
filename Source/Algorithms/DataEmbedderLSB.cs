using F5.James;

namespace ImageSteganography
{
    public class DataEmbedderLSB : DataEmbedder
    {
        public override bool Enocode(string imagePath, string message, out string resultMessage)
        {
            try
            {
                string directory = Path.GetDirectoryName(imagePath);
                string newImageName = Path.GetFileNameWithoutExtension(imagePath) + "Message.jpg";
                string newImagePath = Path.Combine(directory, newImageName);

                int quality = 80;

                using (Image image = Image.FromFile(imagePath))
                {
                    using (JpegEncoder jpg = new JpegEncoder(image, File.OpenWrite(newImagePath), quality))
                    {
                        jpg.Encode(message, this);
                    }
                }
                resultMessage = "Success";
                return true;
            }
            catch (Exception ex)
            {
                resultMessage = "Error: " + ex.Message;
                return false;
            }
        }

        public override bool Decode(string imagePath, out string resultMessage)
        {
            resultMessage = "No decoding coded";
            return false;
        }

        public override void EmbeddMessage(ref int[] coefficients)
        {
            Random random = new Random();

            int counter = 0;

            for (int i = 0; i < coefficients.Length; i += 64 * 3)
            {
                for (int j = 0; j < 64; j++)
                {
                    int value = coefficients[i + j];

                    if (value == 0 || value == 1) continue;

                    counter++;

                    if (random.Next(0, 2) == 0)
                    {
                        //Set lsb to 0
                        int newValue = value & ~1;
                        coefficients[i + j] = newValue;
                    }
                    else
                    {
                        //Set lsb to 1
                        int newValue = value | 1;
                        coefficients[i + j] = newValue;
                    }
                }
            }

            Console.WriteLine(counter);
        }
    }
}