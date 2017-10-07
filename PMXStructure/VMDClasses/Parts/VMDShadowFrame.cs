using System;

using System.IO;

using PMXStructure.VMDClasses;

namespace PMXStructure.VMDClasses
{
    public class VMDShadowFrame
    {
        public uint FrameNumber { get; set; }
        public byte Mode { get; set; }
        public float Distance { get; set; }

        public VMDShadowFrame()
        {            
        }

        public void LoadFromStream(BinaryReader br)
        {
            this.FrameNumber = br.ReadUInt32();
            this.Mode = br.ReadByte();
            this.Distance = br.ReadSingle();
        }

        public void SaveToStream(BinaryWriter bw)
        {
            bw.Write(this.FrameNumber);
            bw.Write(this.Mode);
            bw.Write(this.Distance);
        }
    }
}
