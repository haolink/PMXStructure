using System;
using System.IO;

namespace PMXStructure.PMXClasses.General
{
    public class PMXColorRGBA
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public PMXColorRGBA()
        {
            this.R = 0.0f;
            this.G = 0.0f;
            this.B = 0.0f;
            this.A = 0.0f;
        }

        public static PMXColorRGBA LoadFromStreamStatic(BinaryReader br)
        {
            PMXColorRGBA res = new PMXColorRGBA();
            res.LoadFromStream(br);
            return res;
        }

        public void LoadFromStream(BinaryReader br)
        {
            this.R = br.ReadSingle();
            this.G = br.ReadSingle();
            this.B = br.ReadSingle();
            this.A = br.ReadSingle();
        }

        public void WriteToStream(BinaryWriter bw)
        {
            bw.Write(this.R);
            bw.Write(this.G);
            bw.Write(this.B);
            bw.Write(this.A);
        }
    }
}
