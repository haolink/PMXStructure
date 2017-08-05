using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts.VertexDeform
{
    public class PMXVertexDeformBDEF1 : PMXBaseDeform
    {
        public PMXBone Bone1 { get; set; } //Bone 1
        protected int bone1Index; //Import only!

        public PMXVertexDeformBDEF1(PMXModel model, PMXVertex vertex) : base(model, vertex)
        {
            this.deformIdentifier = PMXBaseDeform.DEFORM_IDENTIFY_BDEF1;
        }

        public override void FinaliseAfterImport()
        {
            this.Bone1 = this.Model.Bones[bone1Index];
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.bone1Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
        }

        public override void WriteToStream(BinaryWriter bw, PMXExportSettings exportSettings)
        {
            bw.Write(this.deformIdentifier);
            PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.Bone1, exportSettings));
        }
    }
}
