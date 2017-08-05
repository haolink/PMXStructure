using System;
using System.Text;

namespace PMXStructure.PMXClasses.Helpers
{
    public class MMDImportSettings
    {
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
