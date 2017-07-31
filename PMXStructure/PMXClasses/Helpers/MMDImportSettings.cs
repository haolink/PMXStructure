using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMXStructure.PMXClasses.Helpers
{
    public class MMDImportSettings
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

        public enum ModelFormat
        {
            PMX, PMD
        }

        public ModelFormat Format { get; private set; }

        private PMXBitLength _bitSettings;
        public PMXBitLength BitSettings { get
            {
                return this._bitSettings;
            }
        }

        public Encoding TextEncoding { get; set; }
        public byte ExtendedUV { get; set; }

        public float PMXFileVersion { get; private set; }


        public MMDImportSettings(ModelFormat format)
        {
            this.Format = format;

            this._bitSettings = new PMXBitLength();
        }
    }
}
