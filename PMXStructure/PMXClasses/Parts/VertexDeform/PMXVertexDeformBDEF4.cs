using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.deformIdentifier = 2;
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            base.LoadFromStream(br, importSettings);

            this.Bone2Weight = br.ReadSingle();
            this.bone3Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
            this.Bone3Weight = br.ReadSingle();
            this.bone4Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
            this.Bone4Weight = br.ReadSingle();
        }
    }
}
