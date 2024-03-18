using System;
using System.IO;
using System.Buffers;
using System.Runtime.InteropServices;
using JpegLibrary;
using JpegLibrary.Benchmarks;
using JpegLibrary.ColorConverters;

namespace ImageSteganography
{
    public class DataEmbedderLSB
    {
        public bool Enocode(string imagePath, string message)
        {
            if (TryLoadBitmap(imagePath, out Bitmap originalBitmap))
            {
                string directory = Path.GetDirectoryName(imagePath);
                string newImageName = Path.GetFileNameWithoutExtension(imagePath) + "Message.jpg";
                string newImagePath = Path.Combine(directory, newImageName);

                var encoder = new JpegEncoder();
                encoder.SetQuantizationTable(JpegStandardQuantizationTable.ScaleByQuality(JpegStandardQuantizationTable.GetLuminanceTable(JpegElementPrecision.Precision8Bit, 0), 75));
                encoder.SetQuantizationTable(JpegStandardQuantizationTable.ScaleByQuality(JpegStandardQuantizationTable.GetChrominanceTable(JpegElementPrecision.Precision8Bit, 1), 75));
                encoder.SetHuffmanTable(true, 0, JpegStandardHuffmanEncodingTable.GetLuminanceDCTable());
                encoder.SetHuffmanTable(false, 0, JpegStandardHuffmanEncodingTable.GetLuminanceACTable());
                encoder.SetHuffmanTable(true, 1, JpegStandardHuffmanEncodingTable.GetChrominanceDCTable());
                encoder.SetHuffmanTable(false, 1, JpegStandardHuffmanEncodingTable.GetChrominanceACTable());
                encoder.AddComponent(1, 0, 0, 0, 1, 1);     //Y component
                encoder.AddComponent(2, 1, 1, 1, 1, 1);     //Cb component
                encoder.AddComponent(3, 1, 1, 1, 1, 1);     //Cr component

                Color[] rgba = GetPixelsFromBitmap(originalBitmap);

                int width = originalBitmap.Width;
                int height = originalBitmap.Height;

                byte[] ycbcr = ArrayPool<byte>.Shared.Rent(3 * width * height);
                NullBufferWriter writer = new NullBufferWriter();

                try
                {
                    JpegRgbToYCbCrConverter.Shared.ConvertRgba32ToYCbCr8(MemoryMarshal.AsBytes(rgba.AsSpan()), ycbcr, width * height);

                    using (BinaryReader reader = new BinaryReader(new FileStream(imagePath, FileMode.Open)))
                    {
                        long length = reader.BaseStream.Length;
                        byte[] _inputBytes = reader.ReadBytes((int)length);

                        encoder.SetInputReader(new JpegBufferInputReader(width, height, 3, _inputBytes));
                        encoder.SetOutput(writer);

                        encoder.Encode();
                    }
                }
                catch (Exception exeption)
                {
                    Console.WriteLine(exeption.Message);
                    return false;
                }

                //Create new image from the new image data
                ArrayPool<byte>.Shared.Return(ycbcr);
                using (BinaryWriter outputStream = new BinaryWriter(new FileStream(newImagePath, FileMode.Create)))
                {
                    byte[] data = writer.GetMemory().ToArray();
                    outputStream.Write(data);
                    Console.WriteLine($"Saved image to {imagePath}");

                    return true;
                }
            }
            else
            {
                Console.WriteLine($"Failed to load image: {imagePath}");
                return false;
            }
        }

        public bool Decode(string imagePath, out string message)
        {
            message = "No decoding coded";
            return true;
        }

        public static bool TryLoadBitmap(string path, out Bitmap result)
        {
            try
            {
                result = new Bitmap(path);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error loading image: {exception.Message}");
                result = null;
                return false;
            }
        }

        static Color[] GetPixelsFromBitmap(Bitmap bitmap)
        {
            Color[] pixels = new Color[bitmap.Width * bitmap.Height];

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixels[y * bitmap.Width + x] = bitmap.GetPixel(x, y);
                }
            }

            return pixels;
        }
    }
}