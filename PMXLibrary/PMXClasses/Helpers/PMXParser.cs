using System;
using System.Text;

using System.IO;

namespace PMXStructure.PMXClasses
{
    public static class PMXParser
    {
        public static string ReadString(BinaryReader br, Encoding textEncoding)
        {
            int size = br.ReadInt32();
            if(size == 0)
            {
                return null;
            }
            byte[] buffer = new byte[size];
            br.BaseStream.Read(buffer, 0, size);
            return textEncoding.GetString(buffer);
        }

        public static void WriteString(BinaryWriter bw, Encoding textEncoding, string text)
        {
            Int32 size;
            if (text == null || text == "")
            {
                size = 0;
                bw.Write(size);                
            } else {
                byte[] buffer = textEncoding.GetBytes(text);
                size = buffer.Length;
                bw.Write(size);
                bw.BaseStream.Write(buffer, 0, size);
            }            
        }

        public static int ReadIndex(BinaryReader br, byte indexLength)
        {
            byte[] buffer = new byte[(int)indexLength];
            br.BaseStream.Read(buffer, 0, indexLength);

            int result = 0;

            switch(indexLength)
            {
                case 1:
                    result = (int)(buffer[0]);
                    if(result == byte.MaxValue)
                    {
                        result = -1;
                    }
                    break;
                case 2:
                    result = (int)(ushort)BitConverter.ToInt16(buffer, 0);                    
                    if(result == ushort.MaxValue)
                    {
                        result = -1;
                    }
                    break;
                case 4:
                    result = BitConverter.ToInt32(buffer, 0);
                    break;                
            }

            return result;
        }

        public static void WriteIndex(BinaryWriter bw, byte indexLength, int value)
        {
            byte[] buffer = new byte[(int)indexLength];
            switch (indexLength)
            {
                case 1:
                    if(value == -1)
                    {
                        buffer[0] = 0xFF;
                    }
                    else
                    {
                        buffer[0] = (byte)value;
                    }
                    break;
                case 2:
                    if (value == -1)
                    {
                        buffer[0] = 0xFF;
                        buffer[1] = 0xFF;
                    }
                    else
                    {
                        buffer = BitConverter.GetBytes((ushort)value);
                    }                    
                    break;
                case 4:
                    buffer = BitConverter.GetBytes((Int32)value);
                    break;
            }

            bw.BaseStream.Write(buffer, 0, indexLength);
        }        
    }
}
