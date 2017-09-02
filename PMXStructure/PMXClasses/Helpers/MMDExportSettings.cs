using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PMXStructure.PMXClasses.Parts;

namespace PMXStructure.PMXClasses.Helpers
{
    public class MMDExportSettings
    {
        public enum ModelFormat
        {
            PMX, PMD
        }

        public ModelFormat Format { get; private set; }

        private PMXBitLength _bitSettings;
        public PMXBitLength BitSettings
        {
            get
            {
                return this._bitSettings;
            }
        }

        public int ExportHash { get; set; } //For quicker vertex export

        public PMXModel Model { get; set; }

        public List<PMXVertex> BaseMorphVertices { get; set; }

        public Encoding TextEncoding { get; set; }
        public byte ExtendedUV { get; set; }

        public MMDExportSettings(ModelFormat format = ModelFormat.PMD)
        {
            this._bitSettings = new PMXBitLength();

            this.Format = format;
        }
    }
}
