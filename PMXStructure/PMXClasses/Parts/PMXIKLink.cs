using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.General;

namespace PMXStructure.PMXClasses.Parts
{
    public class PMXIKLink : PMXBasePart
    {
        protected PMXIK IK { get; private set; }

        private int boneIndex; //Import only
        public PMXBone Bone { get; set; }
        public bool HasLimits { get; set; }
        public PMXVector3 Minimum { get; set; }
        public PMXVector3 Maximum { get; set; }


        public PMXIKLink(PMXModel model, PMXIK ik) : base(model)
        {
            this.IK = ik;
            this.Minimum = new PMXVector3();
            this.Maximum = new PMXVector3();
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.boneIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);

            this.HasLimits = (br.ReadByte() == 1);
            if(this.HasLimits)
            {
                this.Minimum = PMXVector3.LoadFromStreamStatic(br);
                this.Maximum = PMXVector3.LoadFromStreamStatic(br);
            }
        }

        public override void FinaliseAfterImport()
        {
            throw new NotImplementedException();
        }
    }
}
