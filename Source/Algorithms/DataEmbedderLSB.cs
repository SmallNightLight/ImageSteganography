using F5;
using F5.James;
using F5.Ortega;
using System.Text;

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

        public override bool Decode(string imagePath, out string message, out string resultMessage)
        {
            try
            {
                int[] coefficients;

                using (HuffmanDecode hd = new HuffmanDecode(File.OpenRead(imagePath)))
                {
                    coefficients = hd.Decode();
                }

                List<bool> result = DecodeMessage(coefficients);
                byte[] byteArray = ConvertBitsToBytes(result.ToArray());

                message = Encoding.UTF8.GetString(byteArray);
                resultMessage = "Encoded message";
                return true;
            }
            catch (Exception ex)
            {
                message = "";
                resultMessage = "Error: " + ex.Message;
                return false;
            }
        }

        public override void EmbeddMessage(ref int[] coefficients, bool[] data)
        {
            int messageLength = data.Count();
            int counter = 0;

            for (int i = 0; i < coefficients.Length; i += 64)
            {
                for (int j = 0; j < 64; j++)
                {
                    int value = coefficients[i + j];

                    if (value == 0 || value == 1) continue;

                    
                    if (data[counter])
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

                    counter++;

                    if (counter >= messageLength) 
                        return;
                }
            }

            Console.WriteLine("Not enough space for message");
        }

        public override List<bool> DecodeMessage(int[] coefficients)
        {
            List<bool> result = new List<bool>();

            for (int i = 0; i < coefficients.Length; i += 64)
            {
                for (int j = 0; j < 64; j++)
                {
                    int value = coefficients[i + j];

                    if (value == 0 || value == 1) continue;

                    bool bit = (value & 1) == 1;
                    result.Add(bit);
                }
            }

            return result;
        }

        byte[] ConvertBitsToBytes(bool[] bools)
        {
            int len = bools.Length;
            int bytes = len >> 3;
            if ((len & 0x07) != 0) ++bytes;
            byte[] arr2 = new byte[bytes];
            for (int i = 0; i < bools.Length; i++)
            {
                if (bools[i])
                    arr2[i >> 3] |= (byte)(1 << (i & 0x07));
            }

            return arr2;
        }
    }
}