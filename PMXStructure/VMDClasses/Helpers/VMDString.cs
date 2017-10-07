using System;

using System.IO;
using System.Text;

namespace PMXStructure.VMDClasses
{
    public static class VMDString
    {
        private static Encoding ShiftJISEncoding = null;

        public static string ReadString(BinaryReader br, int length, Encoding encoding = null)
        {
            if(encoding == null)
            {
                if (ShiftJISEncoding == null)
                {
                    ShiftJISEncoding = Encoding.GetEncoding("shift-jis");
                }

                encoding = ShiftJISEncoding;
            }            

            byte[] buffer = new byte[length];
            br.BaseStream.Read(buffer, 0, length);

            return encoding.GetString(buffer).TrimEnd(new char[] { '\0' });
        }

        public static void WriteString(BinaryWriter bw, int length, string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                if (ShiftJISEncoding == null)
                {
                    ShiftJISEncoding = Encoding.GetEncoding("shift-jis");
                }

                encoding = ShiftJISEncoding;
            }


            str = str.Trim();

            str = str.Replace("\r\n", "\n");

            byte[] buffer = encoding.GetBytes(str);

            byte[] sendBuffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                sendBuffer[i] = 0x00;
            }
            Array.Copy(buffer, sendBuffer, Math.Min(buffer.Length, length));           

            bw.BaseStream.Write(sendBuffer, 0, length);
        }
    }
}
