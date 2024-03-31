using BitMiracle.LibJpeg.Classic;
using System.Drawing.Imaging;

namespace ImageSteganography
{
    public abstract class DataEmbedder
    {
        public abstract bool EmbeddMessage(ref JBLOCK[][] coefficients, bool[] data, int blockCountX, int blockCountY);

        public abstract List<bool> DecodeMessage(JBLOCK[][] coefficients, int blockCountX, int blockCountY);


        public bool Enocode(string imagePath, string message, out string resultMessage)
        {
            //Set quality
            int quality = 80;

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
                int blockCountX = width / 8;
                int blockCountY = height / 8;

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
                var coefficients = JBlock[0].Access(0, blockCountY);

                //Get message bits
                var messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
                var messageBits = ReadBits(new MemoryStream(messageBytes)).ToArray();

                //Embedd message into coefficients
                EmbeddMessage(ref coefficients, messageBits, blockCountX, blockCountY);

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

                resultMessage = "Encoded image";
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
                int blockCountX = width / 8;
                int blockCountY = height / 8;

                //Get coefficients
                jvirt_array<JBLOCK>[] JBlock = jpegData.jpeg_read_coefficients();
                var coefficients = JBlock[0].Access(0, blockCountY);

                //Read
                List<bool> messageBits = DecodeMessage(coefficients, blockCountX, blockCountY);
                byte[] messageBytes = ConvertBitsToBytes(messageBits.ToArray());
                message = System.Text.Encoding.UTF8.GetString(messageBytes);

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
    }
}