using System;
using System.Collections.Generic;
using System.IO;

using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts
{
    public class PMXDisplaySlot : PMXBasePart
    {
        public const byte REF_IDENTIFY_BONE = 0;
        public const byte REF_IDENTIFY_MORPH = 1;

        public string NameJP { get; set; }
        public string NameEN { get; set; }

        public List<PMXBasePart> References { get; private set; }

        public PMXDisplaySlot(PMXModel model) : base(model)
        {
            this.References = new List<PMXBasePart>();
        }

        public override void FinaliseAfterImport()
        {
            throw new NotImplementedException();
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.NameJP = PMXParser.ReadString(br, importSettings.TextEncoding);
            this.NameEN = PMXParser.ReadString(br, importSettings.TextEncoding);

            byte flag = br.ReadByte(); //Ignored will be automatically generated on export.

            int refCount = br.ReadInt32();

            for(int i = 0; i < refCount; i++)
            {
                byte refType = br.ReadByte();
                switch (refType)
                {
                    case PMXDisplaySlot.REF_IDENTIFY_BONE:
                        int boneIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
                        this.References.Add(this.Model.Bones[boneIndex]);
                        break;
                    case PMXDisplaySlot.REF_IDENTIFY_MORPH:
                        int morphIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.MorphIndexLength);
                        this.References.Add(this.Model.Bones[morphIndex]);
                        break;
                }
            }
        }
    }
}
