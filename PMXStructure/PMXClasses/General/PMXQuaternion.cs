using System;
using System.IO;

namespace PMXStructure.PMXClasses.General
{
    class PMXQuaternion
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

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

    }
}
