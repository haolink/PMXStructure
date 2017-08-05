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
            this.boneTargetIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
            this.Loop = br.ReadInt32();
            this.RadianLimit = br.ReadSingle();

            int linkCount = br.ReadInt32();
            for(int i = 0; i < linkCount; i++)
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
    }
}
