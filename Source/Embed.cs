using System;
using System.Drawing;
using System.IO;

namespace F5
{
    using F5.James;
    public static class Embed
    {
        public static void Run(string imagePath, string message)
        {
            string directory = Path.GetDirectoryName(imagePath);
            string newImageName = Path.GetFileNameWithoutExtension(imagePath) + "Message.jpg";
            string newImagePath = Path.Combine(directory, newImageName);

            int quality = 80;

            using (Image image = Image.FromFile(imagePath))
            using (JpegEncoder jpg = new JpegEncoder(image, File.OpenWrite(newImagePath), quality))
            {
                jpg.Compress(message);
            }
        }
    }
}