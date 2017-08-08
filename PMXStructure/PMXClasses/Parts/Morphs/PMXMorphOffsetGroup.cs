using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;

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
            this.MorphTarget = this.Model.Morphs[this.morphTargetIndex];
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.morphTargetIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.MorphIndexLength);
            this.Strength = br.ReadSingle();
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            PMXParser.WriteIndex(bw, exportSettings.BitSettings.MorphIndexLength, PMXMorph.CheckIndexInModel(this.Morph, exportSettings, false));
            bw.Write(this.Strength);
        }
    }
}
