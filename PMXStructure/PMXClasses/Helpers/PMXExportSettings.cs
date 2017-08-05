using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMXStructure.PMXClasses.Helpers
{
    public class PMXExportSettings
    {
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

        public Encoding TextEncoding { get; set; }
        public byte ExtendedUV { get; set; }

        public PMXExportSettings()
        {
            this._bitSettings = new PMXBitLength();
        }
    }
}
