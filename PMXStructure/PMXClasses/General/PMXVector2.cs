using System;
using System.IO;

namespace PMXStructure.PMXClasses.General
{
    public class PMXVector2
    {
        public float U { get; set; }
        public float V { get; set; }

        public PMXVector2()
        {
            this.U = 0.0f;
            this.V = 0.0f;            
        }

        public PMXVector2(float U, float V)
        {
            this.U = U;
            this.V = V;
        }

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

        public void WriteToStream(BinaryWriter bw)
        {
            bw.Write(this.U);
            bw.Write(this.V);            
        }
    }
}
