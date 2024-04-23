using BitMiracle.LibJpeg.Classic;
using System.Drawing.Imaging;

namespace ImageSteganography
{
    public abstract class DataEmbedder
    {
       protected Random _random = new Random();

        public abstract bool EmbeddMessage(ref JBLOCK[][][] coefficients, bool[] data, out int embeddSize);

        public abstract List<bool> DecodeMessage(JBLOCK[][][] coefficients);


        public bool Encode(string imagePath, int quality, string message, out string resultMessage)
        {
            //Get new image path
            if (imagePath == null)
            {
                resultMessage = "No image path specified";
                return false;
            }

            string? directory = Path.GetDirectoryName(imagePath);
            if (directory == null)
            {
                resultMessage = "Directory not found";
                return false;
            }

            string newImageName = Path.GetFileNameWithoutExtension(imagePath) + "Message.jpg";
            string newImagePath = Path.Combine(directory, newImageName);

            try
            {
                //Create new bitmap out of original image
                var img = new Bitmap(imagePath);
                var width = img.Width;
                var height = img.Height;

                int calh = (int)Math.Ceiling(img.Height / 8.0);
                int calw = (int)Math.Ceiling(img.Width / 8.0);
                int wblocks0 = calw % 2 == 0 ? calw : calw + 1;
                int hblocks0 = calh % 2 == 0 ? calh : calh + 1;
                int wblocks1 = calw % 2 == 0 ? calw / 2 : (calw + 1) / 2;
                int hblocks1 = calh % 2 == 0 ? calh / 2 : (calh + 1) / 2;

                //Create a new stream with the jpeg image data using the bitmap
                MemoryStream memoryStream = new MemoryStream();
                var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encParams = new EncoderParameters(1);
                encParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                img.Save(memoryStream, encoder, encParams);
                img.Dispose();

                //Decompress jpeg stream and access the coefficient
                jpeg_decompress_struct oJpegDecompress = new jpeg_decompress_struct();
                oJpegDecompress.jpeg_stdio_src(memoryStream);
                oJpegDecompress.jpeg_read_header(true);
                
                jvirt_array<JBLOCK>[] JBlock = oJpegDecompress.jpeg_read_coefficients();
                JBLOCK[][][] coefficients =
                {
                    JBlock[0].Access(0, hblocks0),
                    JBlock[1].Access(0, hblocks1),
                    JBlock[2].Access(0, hblocks1)
                };

                //Get message bits
                var messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
                
                //Embedd message size first (4 bytes)
                byte[] sizeBytes = BitConverter.GetBytes(messageBytes.Length);
                byte[] messageWithSize = new byte[sizeBytes.Length + messageBytes.Length];
                Array.Copy(sizeBytes, messageWithSize, sizeBytes.Length);
                Array.Copy(messageBytes, 0, messageWithSize, sizeBytes.Length, messageBytes.Length);

                //Read bits
                var messageBits = ReadBits(new MemoryStream(messageWithSize)).ToArray();

                //Embedd message into coefficients
                EmbeddMessage(ref coefficients, messageBits, out int embeddSize);

                //Compress the jpeg finally
                oJpegDecompress.jpeg_finish_decompress();
                memoryStream.Close();

                //Create a new file with the embedded data
                FileStream objFileStreamMegaMap = File.Create(newImagePath);
                jpeg_compress_struct oJpegCompress = new jpeg_compress_struct();

                oJpegCompress.jpeg_stdio_dest(objFileStreamMegaMap);
                oJpegDecompress.jpeg_copy_critical_parameters(oJpegCompress);
                oJpegCompress.Image_height = height;
                oJpegCompress.Image_width = width;
                oJpegCompress.jpeg_write_coefficients(JBlock);
                oJpegCompress.jpeg_finish_compress();
                objFileStreamMegaMap.Close();
                oJpegDecompress.jpeg_abort_decompress();


                //Calculate PSNR
                var original = new Bitmap(imagePath);
                var modified = new Bitmap(newImagePath);

                MemoryStream memoryStream2 = new MemoryStream();
                var encoder2 = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encParams2 = new EncoderParameters(1);
                encParams2.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                original.Save(memoryStream2, encoder2, encParams2);

                Bitmap jpegOriginal = new Bitmap(memoryStream2);

                double psnr = CalculatePSNR(jpegOriginal, modified);

                original.Dispose();
                jpegOriginal.Dispose();
                modified.Dispose();

                resultMessage = $"Encoded image (capacity: {embeddSize} bytes) (PSNR: {psnr})";
                return true;
            }
            catch (Exception ex)
            {
                resultMessage = ex.Message;
                return false;
            }
        }

        public bool Decode(string imagePath, out string message, out string resultMessage)
        {
            try
            {
                //Read jpeg image data
                jpeg_decompress_struct jpegData = new jpeg_decompress_struct();
                FileStream imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                jpegData.jpeg_stdio_src(imageStream);
                jpegData.jpeg_read_header(true);

                //Get images data
                int width = jpegData.Image_width;
                int height = jpegData.Image_height;

                int calh = (int)Math.Ceiling(height / 8.0);
                int calw = (int)Math.Ceiling(width / 8.0);
                int wblocks0 = calw % 2 == 0 ? calw : calw + 1;
                int hblocks0 = calh % 2 == 0 ? calh : calh + 1;
                int wblocks1 = calw % 2 == 0 ? calw / 2 : (calw + 1) / 2;
                int hblocks1 = calh % 2 == 0 ? calh / 2 : (calh + 1) / 2;

                //Get coefficients
                jvirt_array<JBLOCK>[] JBlock = jpegData.jpeg_read_coefficients();
                //int v = jpegData.Input_iMCU_row;
                JBLOCK[][][] coefficients =
                {
                    JBlock[0].Access(0, hblocks0),
                    JBlock[1].Access(0, hblocks1),
                    JBlock[2].Access(0, hblocks1)
                };

                //Read
                List<bool> messageBits = DecodeMessage(coefficients);
                byte[] messageBytes = ConvertBitsToBytes(messageBits.ToArray());

                if (messageBytes.Length < 4)
                {
                    message = "Error";
                    resultMessage = "Could not read enough bytes to read any message";
                    return false;
                }

                //Read size
                byte[] messageSizeBytes = new byte[4];
                Array.Copy(messageBytes, messageSizeBytes, 4);
                int messageSize = BitConverter.ToInt32(messageSizeBytes);

                byte[] finalMessage = new byte[messageSize];
                Array.Copy(messageBytes, 4, finalMessage, 0, messageSize);

                message = System.Text.Encoding.UTF8.GetString(finalMessage);

                //Close file
                imageStream.Close();

                resultMessage = "Decoded message";
                return true;
            }
            catch (Exception e)
            {
                resultMessage = e.Message;
                message = "Error";
                return false;
            }
        }


        //Helper methods

        public IEnumerable<bool> ReadBits(Stream stream)
        {
            int readByte;
            while ((readByte = stream.ReadByte()) >= 0)
            {
                for (int i = 7; i >= 0; i--)
                    yield return ((readByte >> i) & 1) == 1;
            }
        }

        byte[] ConvertBitsToBytes(bool[] bools)
        {
            int numBytes = (bools.Length + 7) / 8;
            byte[] bytes = new byte[numBytes];

            for (int i = 0; i < bools.Length; i++)
            {
                if (bools[i])
                    bytes[i / 8] |= (byte)(1 << (7 - (i % 8)));
            }

            return bytes;
        }

        /// <summary>
        /// Calculates the peak signal to noise ratio
        /// </summary>
        public static double CalculatePSNR(Bitmap originalImage, Bitmap compressedImage)
        {
            if (originalImage.Size != compressedImage.Size)
                throw new ArgumentException("Images must have the same dimensions.");

            int width = originalImage.Width;
            int height = originalImage.Height;

            double mse = 0;

            //Calculate Mean Squared Error (MSE)
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color originalPixel = originalImage.GetPixel(x, y);
                    Color compressedPixel = compressedImage.GetPixel(x, y);

                    int diffR = originalPixel.R - compressedPixel.R;
                    int diffG = originalPixel.G - compressedPixel.G;
                    int diffB = originalPixel.B - compressedPixel.B;

                    mse += diffR * diffR + diffG * diffG + diffB * diffB;
                }
            }

            mse /= (width * height * 3);

            double maxPixelValue = 255;
            double psnr = 10 * Math.Log10((maxPixelValue * maxPixelValue) / mse);

            return psnr;
        }
    }
}