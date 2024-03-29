using System;
using System.IO;
using System.Text;

namespace F5
{
    public static class Extract
    {
        public static string Run(string imagePath)
        {
            Stream outputStream = new MemoryStream();

            using (JpegExtract extractor = new JpegExtract(outputStream))
            {
                extractor.Extract(File.OpenRead(imagePath));

                byte[] buffer = new byte[outputStream.Length];
                outputStream.Read(buffer, 0, buffer.Length);

                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}