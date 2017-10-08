using System;

using System.IO;
using PMXStructure.PMXClasses;

namespace PMXStructure.VMDClasses
{
    public class VMDLightFrame : MetaDataContainer
    {
        public uint FrameNumber { get; set; }
        public PMXColorRGB Color { get; set; }
        public PMXVector3 Location { get; set; }

        public VMDLightFrame()
        {
            this.Color = new PMXColorRGB();
            this.Location = new PMXVector3();
        }

        public void LoadFromStream(BinaryReader br)
        {
            this.FrameNumber = br.ReadUInt32();
            this.Color = PMXColorRGB.LoadFromStreamStatic(br);
            this.Location = PMXVector3.LoadFromStreamStatic(br);
        }

        public void SaveToStream(BinaryWriter bw)
        {
            bw.Write(this.FrameNumber);
            this.Color.WriteToStream(bw);
            this.Location.WriteToStream(bw);
        }

    }
}
