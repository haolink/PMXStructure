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

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            if (exportSettings.Format == MMDExportSettings.ModelFormat.PMX)
            { //PMX IK
                PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.Bone, exportSettings, true));

                if (this.HasLimits)
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
            else
            { //PMD IK
                PMXParser.WriteIndex(bw, 2, PMXBone.CheckIndexInModel(this.Bone, exportSettings, true));
            }
        }

        private static readonly string[] ThighBones = new string[]
        {
            "左ひざ", "右ひざ"
        };

        /// <summary>
        /// Updates IKs of PMD files for legs.
        /// </summary>
        public void UpdatePMDIKs()
        {
            if(this.Bone != null)
            {
                string bnName = this.Bone.NameJP;
                if(Array.IndexOf<string>(PMXIKLink.ThighBones, bnName) >= 0)
                {
                    this.HasLimits = true;
                    this.Minimum = new PMXVector3((float)((-1) * Math.PI), 0.0f, 0.0f);
                    this.Maximum = new PMXVector3((float)((-1.0f / 360.0f) * Math.PI), 0.0f, 0.0f);
                }
            }
        }
    }
}
