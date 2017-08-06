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
            if(importSettings.Format == MMDImportSettings.ModelFormat.PMX)
            { //PMX format
                this.boneIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);

                this.HasLimits = (br.ReadByte() == 1);
                if (this.HasLimits)
                {
                    this.Minimum = PMXVector3.LoadFromStreamStatic(br);
                    this.Maximum = PMXVector3.LoadFromStreamStatic(br);
                }
            }
            else
            {
                this.boneIndex = br.ReadUInt16();                
            }            
        }

        public override void FinaliseAfterImport()
        {
            if(boneIndex == -1)
            {
                this.Bone = null;
            }
            else
            {
                this.Bone = this.Model.Bones[this.boneIndex];
            }
        }

        public override void WriteToStream(BinaryWriter bw, PMXExportSettings exportSettings)
        {
            PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.Bone, exportSettings, true));

            if(this.HasLimits)
            {
                bw.Write((byte)1);
                this.Minimum.WriteToStream(bw);
                this.Maximum.WriteToStream(bw);
            }
            else
            {
                bw.Write((byte)0);
            }
        }
    }
}
