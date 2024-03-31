using BitMiracle.LibJpeg.Classic;

namespace ImageSteganography
{
    public class DataEmbedderLSB : DataEmbedder
    {
        public override bool EmbeddMessage(ref JBLOCK[][] coefficients, bool[] data, int blockCountX, int blockCountY)
        {
            if (data == null || data.Length == 0) return true;

            int messageLength = data.Count();
            int counter = 0;

            for (int i = 0; i < blockCountY; i++)
            {
                for (int j = 0; j < blockCountX; j++)
                {
                    for (int k = 0; k < 64; k++)
                    {
                        int value = coefficients[i][j][k];

                        if (value == 0 || value == 1) continue;

                        if (data[counter++])
                        {
                            //Set lsb to 1
                            value = value | 1;
                        }
                        else
                        {
                            //Set lsb to 0
                            value = value & ~1;
                        }

                        coefficients[i][j][k] = (short)value;

                        if (counter >= messageLength)
                        {
                            //Message embedded before end of stream reached
                            return true;
                        }
                    }
                }
            }

            //Reached end of stream before all bits could be embedded
            return false;
        }

        public override List<bool> DecodeMessage(JBLOCK[][] coefficients, int blockCountX, int blockCountY)
        {
            List<bool> result = new List<bool>();

            for (int i = 0; i < blockCountY; i++)
            {
                for (int j = 0; j < blockCountX; j++)
                {
                    for (int k = 0; k < 64; k++)
                    {
                        int value = coefficients[i][j][k];

                        if (value == 0 || value == 1) continue;

                        result.Add((value & 1) != 0);
                    }
                }
            }

            return result;
        }
    }
}