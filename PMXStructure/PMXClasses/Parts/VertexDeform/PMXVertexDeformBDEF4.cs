using System;
using System.IO;
using System.Collections.Generic;

using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts.VertexDeform
{
    class PMXVertexDeformBDEF4 : PMXVertexDeformBDEF2
    {
        public float Bone2Weight { get; set; } //Bone 2 weighing

        public PMXBone Bone3 { get; set; } //Bone 3
        public float Bone3Weight { get; set; } //Bone 3 weighing
        private int bone3Index; //Import only!

        public PMXBone Bone4 { get; set; } //Bone 4
        public float Bone4Weight { get; set; } //Bone 4 weighing
        private int bone4Index; //Import only!

        public PMXVertexDeformBDEF4(PMXModel model, PMXVertex vertex) : base(model, vertex)
        {
            this.deformIdentifier = PMXBaseDeform.DEFORM_IDENTIFY_BDEF4;
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.bone1Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
            this.bone2Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
            this.bone3Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
            this.bone4Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
            this.Bone1Weight = br.ReadSingle();
            this.Bone2Weight = br.ReadSingle();
            this.Bone3Weight = br.ReadSingle();
            this.Bone4Weight = br.ReadSingle();            
        }

        public override void FinaliseAfterImport()
        {
            base.FinaliseAfterImport();
            this.Bone3 = this.Model.Bones[bone3Index];
            this.Bone4 = this.Model.Bones[bone4Index];
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            if(exportSettings.Format == MMDExportSettings.ModelFormat.PMX)
            {
                bw.Write(this.deformIdentifier);

                PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.Bone1, exportSettings));
                PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.Bone2, exportSettings));
                PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.Bone3, exportSettings));
                PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.Bone4, exportSettings));
                bw.Write(this.Bone1Weight);
                bw.Write(this.Bone2Weight);
                bw.Write(this.Bone3Weight);
                bw.Write(this.Bone4Weight);
            }            
            else
            {
                int b1i = PMXBone.CheckIndexInModel(this.Bone1, exportSettings);
                int b2i = PMXBone.CheckIndexInModel(this.Bone2, exportSettings);
                int b3i = PMXBone.CheckIndexInModel(this.Bone3, exportSettings);
                int b4i = PMXBone.CheckIndexInModel(this.Bone4, exportSettings);

                List<KeyValuePair<int, float>> sortKeys = new List<KeyValuePair<int, float>>();
                if (b1i >= 0) { sortKeys.Add(new KeyValuePair<int, float>(b1i, this.Bone1Weight)); }
                if (b2i >= 0) { sortKeys.Add(new KeyValuePair<int, float>(b2i, this.Bone2Weight)); }
                if (b3i >= 0) { sortKeys.Add(new KeyValuePair<int, float>(b3i, this.Bone3Weight)); }
                if (b4i >= 0) { sortKeys.Add(new KeyValuePair<int, float>(b4i, this.Bone4Weight)); }

                this.ExportToPMDBase(sortKeys, bw);
            }
        }
    }
}
