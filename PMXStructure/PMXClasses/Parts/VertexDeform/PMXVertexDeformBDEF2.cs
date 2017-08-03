using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts.VertexDeform
{
    class PMXVertexDeformBDEF2 : PMXVertexDeformBDEF1
    {
        public float Bone1Weight { get; set; } //Bone 1 weighing
        public PMXBone Bone2 { get; set; } //Bone 2
        private int bone2Index; //Import only!

        public PMXVertexDeformBDEF2(PMXModel model, PMXVertex vertex) : base(model, vertex)
        {
            this.deformIdentifier = PMXBaseDeform.DEFORM_IDENTIFY_BDEF2;
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            base.LoadFromStream(br, importSettings);
            this.Bone1Weight = br.ReadSingle();
            this.bone2Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
        }
    }
}
