using System;
using System.Collections.Generic;
using System.IO;

using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts
{
    public class PMXIK : PMXBasePart
    {
        private int boneTargetIndex; //Import only
        public PMXBone Target { get; set; }
        public int Loop { get; set; }
        public float RadianLimit { get; set; }

        public List<PMXIKLink> IKLinks { get; private set; }

        public PMXBone Bone { get; private set; }

        public PMXIK(PMXModel model, PMXBone bone) : base(model)
        {
            this.IKLinks = new List<PMXIKLink>();
            this.Bone = bone;
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            int linkCount;

            if(importSettings.Format == MMDImportSettings.ModelFormat.PMX)
            { //PMX IK
                this.boneTargetIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
                this.Loop = br.ReadInt32();
                this.RadianLimit = br.ReadSingle();

                linkCount = br.ReadInt32();                
            }
            else
            { //PMD IK
                this.boneTargetIndex = br.ReadUInt16();
                linkCount = (int)br.ReadByte();

                this.Loop = br.ReadUInt16();
                this.RadianLimit = br.ReadSingle();                
            }

            for (int i = 0; i < linkCount; i++)
            {
                PMXIKLink link = new PMXIKLink(this.Model, this);
                link.LoadFromStream(br, importSettings);
                this.IKLinks.Add(link);
            }
        }

        public override void FinaliseAfterImport()
        {
            if(this.boneTargetIndex == -1)
            {
                this.Target = null;
            }
            else
            {
                this.Target = this.Model.Bones[this.boneTargetIndex];
            }

            foreach(PMXIKLink link in this.IKLinks)
            {
                link.FinaliseAfterImport();
            }
        }

        public override void WriteToStream(BinaryWriter bw, PMXExportSettings exportSettings)
        {
            PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.Target, exportSettings, true));
            bw.Write((Int32)this.Loop);
            bw.Write(this.RadianLimit);

            bw.Write((Int32)this.IKLinks.Count);
            foreach(PMXIKLink link in this.IKLinks)
            {
                link.WriteToStream(bw, exportSettings);
            }
        }
    }
}
