using System;

using System.Text;
using System.IO;

namespace PMXStructure.PMXClasses.Helpers
{
    public static class PMDParser
    {
        public static string ReadString(BinaryReader br, int length, Encoding textEncoding)
        {
            byte[] buffer = new byte[length];
            br.BaseStream.Read(buffer, 0, length);

            int firstZero = Array.IndexOf<byte>(buffer, 0);
            if(firstZero < 0)
            {
                firstZero = length;
            }

            if(firstZero == 0)
            {
                return null;
            }

            byte[] resBuffer = new byte[firstZero];
            Array.Copy(buffer, resBuffer, firstZero);

            return textEncoding.GetString(resBuffer);
        }
    }
}
