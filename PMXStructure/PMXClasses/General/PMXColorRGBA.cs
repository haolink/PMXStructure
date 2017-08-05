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

        public PMXColorRGBA() : this(0.0f, 0.0f, 0.0f, 0.0f)
        {
        }

        public PMXColorRGBA(float R, float G, float B) : this(R, G, B, 0.0f)
        {            
        }

        public PMXColorRGBA(float R, float G, float B, float A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
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
