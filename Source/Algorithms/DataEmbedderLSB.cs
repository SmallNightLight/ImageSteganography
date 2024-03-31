using BitMiracle.LibJpeg;
using BitMiracle.LibJpeg.Classic;
using F5;
using F5.James;
using F5.Ortega;
using System.Drawing.Imaging;
using System.Drawing;
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

                IncImageDCT(imagePath, newImagePath);

                //using (Image image = Image.FromFile(imagePath))
                //{
                //    using (JpegEncoder jpg = new JpegEncoder(image, File.OpenWrite(newImagePath), quality))
                //    {
                //        jpg.Encode(message, this);
                //    }
                //}
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
                jpeg_decompress_struct cinfo = new jpeg_decompress_struct();
                FileStream objFileStreamHeaderImage = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                cinfo.jpeg_stdio_src(objFileStreamHeaderImage);
                cinfo.jpeg_read_header(true);
                var coeffs = cinfo.jpeg_read_coefficients();
                int height = cinfo.Image_height / 8;
                int width = cinfo.Image_width / 8;

                int[] coefficients = new int[height * width * 64];

                var dct = coeffs[0].Access(0, height);
                var d1 = coeffs[1].Access(0, height / 2);
                var d2 = coeffs[2].Access(0, height / 2);

                int index = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < 64; k++)
                        {
                            coefficients[index++] = dct[i][j][k]; //h w k
                        }
                    }
                }

                //using (HuffmanDecode hd = new HuffmanDecode(File.OpenRead(imagePath)))
                //{
                //    coefficients = hd.Decode();
                //}

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

        public static void IncImageDCT(string fullPath, string newPath)
        {
            var img = new Bitmap(fullPath);
            var width = img.Width;
            var height = img.Height;
            int hd = height / 8;
            int wd = width / 8;

            MemoryStream memoryStream = new MemoryStream();
            img.Save(memoryStream, ImageFormat.Jpeg);
            img.Dispose();

            jpeg_decompress_struct oJpegDecompress = new jpeg_decompress_struct();
            oJpegDecompress.jpeg_stdio_src(memoryStream);
            oJpegDecompress.jpeg_read_header(true);
            jvirt_array<JBLOCK>[] JBlock = oJpegDecompress.jpeg_read_coefficients();
            var block = JBlock[0].Access(0, hd);
            
            Random r = new Random();

            List<bool> b = new List<bool>();
            for (int i = 0; i < hd; i++)
            {
                for (int j = 0; j < wd; j++)
                {
                    for(int k = 0; k < 64; k++)
                    {
                        int value = block[i][j][k];

                        if (value == 0 || value == 1) continue;

                        if (r.Next(0, 2) == 0)
                        {
                            //Set lsb to 0
                            b.Add(true);
                            value = value & ~1;
                        }
                        else
                        {
                            //Set lsb to 1
                            b.Add(false);
                            value = value | 1;
                        }
                        block[i][j][k] = (short)value;
                    }
                }
            }

            oJpegDecompress.jpeg_finish_decompress();
            memoryStream.Close();

            FileStream objFileStreamMegaMap = File.Create(newPath);
            jpeg_compress_struct oJpegCompress = new jpeg_compress_struct();

            oJpegCompress.jpeg_stdio_dest(objFileStreamMegaMap);
            oJpegDecompress.jpeg_copy_critical_parameters(oJpegCompress);
            oJpegCompress.Image_height = height;
            oJpegCompress.Image_width = width;
            oJpegCompress.jpeg_write_coefficients(JBlock);
            oJpegCompress.jpeg_finish_compress();
            objFileStreamMegaMap.Close();
            oJpegDecompress.jpeg_abort_decompress();
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

                    bool bit = (value & 1) == 0;
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