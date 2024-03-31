using System;
using System.IO;
using System.Text;

namespace F5
{
    using F5.Crypt;
    using F5.Ortega;
    using ImageSteganography;

    public class JpegExtract : IDisposable
    {
        private Stream output;
        private F5Random random;
        int extractedBit, pos;
        int availableExtractedBits = 0;
        int extractedFileLength = 0;
        int nBytesExtracted = 0;
        int extractedByte = 0;
        int shuffledIndex = 0;

        public JpegExtract(Stream output)
        {
            this.output = output;
            this.random = new F5Random();
        }

        byte[] PackBoolsInByteArray(bool[] bools)
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

        public string Extract(Stream input, DataEmbedder dataEmbedder)
        {
            int[] coeff;
            int i, n, k, hash, code;

            using (HuffmanDecode hd = new HuffmanDecode(input))
            {
                 coeff = hd.Decode();
            }

            //List<bool> result = dataEmbedder.DecodeMessage(coeff);
            //
            //byte[] byteArray = PackBoolsInByteArray(result.ToArray());

            return "";
            //return Encoding.UTF8.GetString(byteArray);

            // Decode byte array using UTF-8 encoding to get the original string
            //string originalString = Encoding.UTF8.GetString(byteArray);

            //Console.WriteLine("Permutation starts");
            //Permutation permutation = new Permutation(coeff.Length, this.random);
            //Console.WriteLine(coeff.Length + " indices shuffled");

            // extract length information
            //CalcEmbeddedLength(permutation, coeff);
            //k = (this.extractedFileLength >> 24) % 32;
            //n = (1 << k) - 1;
            //this.extractedFileLength &= 0x007fffff;

            //Console.WriteLine("Length of embedded file: " + extractedFileLength + " bytes");

            //if (n > 0)
            //{
            //    while (true)
            //    {
            //        hash = 0;
            //        code = 1;
            //        while (code <= n)
            //        {
            //            this.pos++;
            //            if (this.pos >= coeff.Length)
            //                goto leaveContext;
            //            this.shuffledIndex = permutation.GetShuffled(this.pos);
            //            this.extractedBit = ExtractBit(coeff);
            //            if (this.extractedBit == -1)
            //                continue;
            //            else if (this.extractedBit == 1)
            //                hash ^= code;
            //            code++;
            //        }

            //        for (i = 0; i < k; i++)
            //        {
            //            this.extractedByte |= (hash >> i & 1) << this.availableExtractedBits++;
            //            if (this.availableExtractedBits == 8)
            //            {
            //                WriteExtractedByte();
            //                // check for pending end of embedded data
            //                if (this.nBytesExtracted == this.extractedFileLength)
            //                    goto leaveContext;
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    while (++this.pos < coeff.Length && this.pos < permutation.Length)
            //    {
            //        this.shuffledIndex = permutation.GetShuffled(this.pos);
            //        this.extractedBit = ExtractBit(coeff);
            //        if (this.extractedBit == -1)
            //            continue;
            //        this.extractedByte |= this.extractedBit << this.availableExtractedBits++;
            //        if (this.availableExtractedBits == 8)
            //        {
            //            WriteExtractedByte();
            //            if (this.nBytesExtracted == extractedFileLength)
            //                break;
            //        }
            //    }
            //}
        leaveContext: ;
            if (this.nBytesExtracted < this.extractedFileLength)
            {
                Console.WriteLine("Incomplete file: only " + this.nBytesExtracted + 
                    " of " + this.extractedFileLength + " bytes extracted");
            }
        }

        /// <summary>
        /// extract length information
        /// </summary>
        private void CalcEmbeddedLength(Permutation permutation, int[] coeff)
        {
            this.extractedFileLength = 0;
            this.pos = -1;

            int i = 0;
            while (i < 32 && ++this.pos < coeff.Length)
            {
                this.shuffledIndex = permutation.GetShuffled(this.pos);
                this.extractedBit = ExtractBit(coeff);
                if (this.extractedBit == -1)
                    continue;
                this.extractedFileLength |= this.extractedBit << i++;
            }

            // remove pseudo random pad
            this.extractedFileLength ^= this.random.GetNextByte();
            this.extractedFileLength ^= this.random.GetNextByte() << 8;
            this.extractedFileLength ^= this.random.GetNextByte() << 16;
            this.extractedFileLength ^= this.random.GetNextByte() << 24;
        }
        private void WriteExtractedByte()
        {
            // remove pseudo random pad
            this.extractedByte ^= this.random.GetNextByte();
            this.output.WriteByte((byte)this.extractedByte);
            this.extractedByte = 0;
            this.availableExtractedBits = 0;
            this.nBytesExtracted++;
        }
        private int ExtractBit(int[] coeff)
        {
            int coeffVal;
            int mod64 = this.shuffledIndex % 64;
            if (mod64 == 0)
                return -1;
            this.shuffledIndex = this.shuffledIndex - mod64 + HuffmanDecode.deZigZag[mod64];
            coeffVal = coeff[this.shuffledIndex];
            if (coeffVal == 0)
                return -1;
            return coeffVal > 0 ? coeffVal & 1 : 1 - (coeffVal & 1);
        }

        #region IDisposable
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JpegExtract()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
                return;
            if (disposing)
                this.output.Dispose();
            this._disposed = true;
        }
        #endregion
    }
}