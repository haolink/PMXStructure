using System;
using System.IO;

namespace PMXStructure.PMXClasses.General
{
    public class PMXVector2
    {
        public float U { get; set; }
        public float V { get; set; }

        public static PMXVector2 LoadFromStreamStatic(BinaryReader br)
        {
            PMXVector2 res = new PMXVector2();
            res.LoadFromStream(br);
            return res;
        }

        public void LoadFromStream(BinaryReader br)
        {
            this.U = br.ReadSingle();
            this.V = br.ReadSingle();
        }
    }
}
