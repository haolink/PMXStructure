using System;
using System.Collections.Generic;
using System.Text;

namespace PMXStructure.PMXClasses
{
    public class PMXBitLength
    {
        public byte VertexIndexLength { get; set; }
        public byte TextureIndexLength { get; set; }
        public byte MaterialIndexLength { get; set; }
        public byte BoneIndexLength { get; set; }
        public byte MorphIndexLength { get; set; }
        public byte RigidBodyIndexLength { get; set; }
    }
}
