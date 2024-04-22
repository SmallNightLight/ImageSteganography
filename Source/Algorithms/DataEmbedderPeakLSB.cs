using BitMiracle.LibJpeg.Classic;

namespace ImageSteganography
{
    public class DataEmbedderPeakLSB : DataEmbedder
    {
        private int[] indexes = { 2, 3, 9, 10, 16, 17, 24 }; //maaybe with 1 and 8 but that reduces it visible but gains a lot of capacity

        public override bool EmbeddMessage(ref JBLOCK[][][] coefficients, bool[] data, out int embeddSize)
        {
            int channel = 0; //Luminance

            embeddSize = 0;

            if (data == null || data.Length == 0) return true;

            int messageLength = data.Count();
            int counter = 0;

            for (int i = 0; i < coefficients[channel].Length - 1; i++)
            {
                for (int j = 0; j < coefficients[channel][i].Length - 1; j++)
                {
                    foreach (int k in indexes)
                    {
                        bool bit;

                        if (counter < messageLength)
                        {
                            //Encode message
                            bit = data[counter];
                        }
                        else
                        {
                            //Encode random message
                            bit = _random.Next(0, 2) == 1;
                        }

                        int value = coefficients[channel][i][j][k];

                        //Skip 0 and 1 values to reduce noise (also reduces capacity)
                        if (value == 0 || value == 1) continue;

                        //Change coefficient
                        if (bit)
                        {
                            //Set lsb to 1
                            value = value | 1;
                        }
                        else
                        {
                            //Set lsb to 0
                            value = value & ~1;
                        }

                        coefficients[channel][i][j][k] = (short)value;
                        counter++;
                    }
                }
            }

            //Reached end of stream before all bits could be embedded
            embeddSize = counter / 8;
            return counter >= messageLength;
        }

        public override List<bool> DecodeMessage(JBLOCK[][][] coefficients)
        {
            int channel = 0; //Luminance

            List<bool> result = new List<bool>();

            for (int i = 0; i < coefficients[channel].Length - 1; i++)
            {
                for (int j = 0; j < coefficients[channel][i].Length - 1; j++)
                {
                    foreach (int k in indexes)
                    {
                        int value = coefficients[channel][i][j][k];

                        if (value == 0 || value == 1) continue;

                        result.Add((value & 1) != 0);
                    }
                }
            }

            return result;
        }
    }
}