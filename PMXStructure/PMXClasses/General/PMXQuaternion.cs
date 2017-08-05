using System;
using System.IO;

namespace PMXStructure.PMXClasses.General
{
    public class PMXQuaternion
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public PMXQuaternion()
        {
            this.X = 0.0f;
            this.Y = 0.0f;
            this.Z = 0.0f;
            this.W = 0.0f;
        }            

        public static PMXQuaternion LoadFromStreamStatic(BinaryReader br)
        {
            PMXQuaternion res = new PMXQuaternion();
            res.LoadFromStream(br);
            return res;
        }

        public void LoadFromStream(BinaryReader br)
        {
            this.X = br.ReadSingle();
            this.Y = br.ReadSingle();
            this.Z = br.ReadSingle();
            this.W = br.ReadSingle();
        }

        public void WriteToStream(BinaryWriter bw)
        {
            bw.Write(this.X);
            bw.Write(this.Y);
            bw.Write(this.Z);
            bw.Write(this.W);
        }
    }
}
