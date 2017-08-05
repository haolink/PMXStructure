using System;
using System.IO;

namespace PMXStructure.PMXClasses.General
{
    public class PMXColorRGB
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }

        public PMXColorRGB() : this(0.0f, 0.0f, 0.0f)
        {            
        }

        public PMXColorRGB(float R, float G, float B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public static PMXColorRGB LoadFromStreamStatic(BinaryReader br)
        {
            PMXColorRGB res = new PMXColorRGB();
            res.LoadFromStream(br);
            return res;
        }

        public void LoadFromStream(BinaryReader br)
        {
            this.R = br.ReadSingle();
            this.G = br.ReadSingle();
            this.B = br.ReadSingle();
        }

        public void WriteToStream(BinaryWriter bw)
        {
            bw.Write(this.R);
            bw.Write(this.G);
            bw.Write(this.B);
        }
    }
}
