using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.Parts;

namespace PMXStructure.PMXClasses.Parts.Morphs
{
    public class PMXMorphOffsetGroup : PMXMorphOffsetBase
    {
        private int morphTargetIndex; //Import only
        public PMXMorph MorphTarget { get; set; }
        public float Strength { get; set; }

        public PMXMorphOffsetGroup(PMXModel model, PMXMorph morph) : base(model, morph)
        {
            this.MorphTargetType = PMXMorph.MORPH_IDENTIFY_GROUP;
        }

        public override void FinaliseAfterImport()
        {
            throw new NotImplementedException();
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.morphTargetIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.MorphIndexLength);
            this.Strength = br.ReadSingle();
        }
    }
}
