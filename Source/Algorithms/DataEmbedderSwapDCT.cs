using BitMiracle.LibJpeg.Classic;

namespace ImageSteganography
{
    public class DataEmbedderSwapDCT : DataEmbedder
    {
        private List<Pair> _pairs = [new Pair(2, 16), new Pair(3, 24), new Pair(9, 18), new Pair(10, 17), new Pair(11, 25), new Pair(4, 32)];
        private int _minDistance = 1;
        private int _maxDistance = 3;

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
                    foreach (Pair pair in _pairs)
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

                        int c1 = coefficients[channel][i][j][pair.Number1];
                        int c2 = coefficients[channel][i][j][pair.Number2];

                        

                        int d = Diff(c1, c2);

                        if (d < _minDistance || d > _maxDistance) continue;

                        //Change coefficients
                        if (bit)
                        {
                            if (c1 < c2)
                            {
                                //Swap
                                (c1, c2) = (c2, c1);
                            }
                        }
                        else
                        {
                            if (c1 > c2)
                            {
                                //Swap
                                (c1, c2) = (c2, c1);
                            }
                        }

                        coefficients[channel][i][j][pair.Number1] = (short)c1;
                        coefficients[channel][i][j][pair.Number2] = (short)c2;
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
                    foreach (Pair pair in _pairs)
                    {
                        int c1 = coefficients[channel][i][j][pair.Number1];
                        int c2 = coefficients[channel][i][j][pair.Number2];

                        int d = Diff(c1, c2);

                        if (d < _minDistance || d > _maxDistance) continue;

                        result.Add(c1 > c2);
                    }
                }
            }

            return result;
        }

        private int Diff(int a, int b) => Math.Abs(a - b);
    }

    public class Pair
    {
        public int Number1, Number2;

        public Pair(int n1, int n2)
        {
            Number1 = n1;
            Number2 = n2;
        }
    }
}