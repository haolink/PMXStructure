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
    }
}
