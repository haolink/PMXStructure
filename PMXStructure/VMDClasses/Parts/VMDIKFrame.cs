using System;

using System.IO;
using System.Collections.Generic;
using PMXStructure.PMXClasses;

namespace PMXStructure.VMDClasses
{
    public class VMDIKFrame
    {
        public uint FrameNumber { get; set; }
        public bool Show { get; set; }
        public List<VMDIKSetting> IKLinks { get; set; }

        public VMDIKFrame()
        {
            this.IKLinks = new List<VMDIKSetting>();
        }

        public void LoadFromStream(BinaryReader br)
        {
            this.FrameNumber = br.ReadUInt32();
            this.Show = (br.ReadByte() == 1);

            uint settingsCount = br.ReadUInt32();

            for(int i = 0; i < settingsCount; i++)
            {
                VMDIKSetting vis = new VMDIKSetting();
                vis.LoadFromStream(br);
                this.IKLinks.Add(vis);
            }
        }

        public void SaveToStream(BinaryWriter bw)
        {
            bw.Write(this.FrameNumber);
            bw.Write((byte)(this.Show ? 1 : 0));

            bw.Write((uint)this.IKLinks.Count);
            foreach(VMDIKSetting ikl in this.IKLinks)
            {
                ikl.SaveToStream(bw);
            }
        }
    }
}
