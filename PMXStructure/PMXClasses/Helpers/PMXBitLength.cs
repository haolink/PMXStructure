using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMXStructure.PMXClasses.Helpers
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
