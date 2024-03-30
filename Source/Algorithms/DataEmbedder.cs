using F5.Util;

namespace ImageSteganography
{
    public abstract class DataEmbedder
    {
        public abstract bool Enocode(string imagePath, string message, out string resultMessage);

        public abstract bool Decode(string imagePath, out string message, out string resultMessage);

        public abstract void EmbeddMessage(ref int[] coefficients, bool[] data);

        public abstract List<bool> DecodeMessage(int[] coefficients);
    }
}