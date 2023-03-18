using System;
using STH1123.ReedSolomon;

namespace MessageMaker
{

    public class ReedSolomon
    {
        public static byte[] Encode(List<byte> byteArray)
        {
            GenericGF field = new GenericGF(285, 256, 0);
            ReedSolomonEncoder rse = new ReedSolomonEncoder(field);

            for (int i = 0; i < 9; i++)
            {
                byteArray.Add(0x00);
            }

            int[] dataToEncode = byteArray.Select(x => (int)x).ToArray();

            rse.Encode(dataToEncode, 9);

            return dataToEncode.Select(x => (byte)x).ToArray();
        }

        public static byte[] Decode(List<byte> byteArray)
        {
            GenericGF field = new GenericGF(285, 256, 0);
            ReedSolomonDecoder rsd = new ReedSolomonDecoder(field);

            int[] dataToDecode = byteArray.Select(x => (int)x).ToArray();

            if (rsd.Decode(dataToDecode, 9))
            {
                return dataToDecode.Select(x => (byte)x).ToArray();
            }
            else
            {
                throw new Exception("Too many errors/erasures to correct");
            }


        }
    }

}