using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts.VertexDeform
{
    class PMXVertexDeformBDEF2 : PMXVertexDeformBDEF1
    {
        public float Bone1Weight { get; set; } //Bone 1 weighing
        public PMXBone Bone2 { get; set; } //Bone 2
        protected int bone2Index; //Import only!

        public PMXVertexDeformBDEF2(PMXModel model, PMXVertex vertex) : base(model, vertex)
        {
            this.deformIdentifier = PMXBaseDeform.DEFORM_IDENTIFY_BDEF2;
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            base.LoadFromStream(br, importSettings);
            this.bone2Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
            this.Bone1Weight = br.ReadSingle();
        }

        public override void FinaliseAfterImport()
        {
            base.FinaliseAfterImport();
            this.Bone2 = this.Model.Bones[bone2Index];
        }
    }
}
