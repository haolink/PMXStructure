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

            string res = textEncoding.GetString(resBuffer);

            res = res.Replace("\n", "\r\n");

            return res;
        }

        public static void WriteString(BinaryWriter bw, int length, Encoding textEncoding, string str)
        {
            if(str == null)
            {
                str = "";
            }

            str = str.Trim();

            str = str.Replace("\r\n", "\n");

            byte[] buffer = textEncoding.GetBytes(str);

            byte[] sendBuffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                sendBuffer[i] = 0x00;
            }
            Array.Copy(buffer, sendBuffer, Math.Min(buffer.Length, length));
            for (int i = buffer.Length + 1; i < length; i++)
            {
                sendBuffer[i] = 0xFD;
            }

            bw.BaseStream.Write(sendBuffer, 0, length);
        }
    }
}
