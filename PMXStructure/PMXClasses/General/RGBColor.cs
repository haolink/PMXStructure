using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace PMXStructure.PMXClasses.General
{
    public class RGBColor
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }

        public static RGBColor LoadFromStreamStatic(BinaryReader br)
        {
            RGBColor res = new RGBColor();
            res.LoadFromStream(br);
            return res;
        }

        public void LoadFromStream(BinaryReader br)
        {
            this.R = br.ReadSingle();
            this.G = br.ReadSingle();
            this.B = br.ReadSingle();
        }
    }
}
