using System;
using System.IO;

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

        public override void WriteToStream(BinaryWriter bw, PMXExportSettings exportSettings)
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
    }
}
