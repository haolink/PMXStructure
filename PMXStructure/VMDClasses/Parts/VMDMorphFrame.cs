using System;

using System.IO;
using PMXStructure.VMDClasses.Helpers;

namespace PMXStructure.VMDClasses.Parts
{
    public class VMDMorphFrame
    {
        public string MorphName { get; set; }
        public uint FrameNumber { get; set; }
        public float Weight { get; set; }

        public void LoadFromStream(BinaryReader br)
        {
            this.MorphName = VMDString.ReadString(br, 15);
            this.FrameNumber = br.ReadUInt32();
            this.Weight = br.ReadSingle();
        }

        public void SaveToStream(BinaryWriter bw)
        {
            VMDString.WriteString(bw, 15, this.MorphName);
            bw.Write(this.FrameNumber);
            bw.Write(this.Weight);            
        }
    }
}
