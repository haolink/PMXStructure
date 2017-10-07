using System;

using System.IO;
using PMXStructure.PMXClasses;

namespace PMXStructure.VMDClasses
{
    public class VMDBoneFrame
    {
        public string BoneName { get; set; }
        public uint FrameNumber { get; set; }
        public PMXVector3 Translation { get; set; }
        public PMXQuaternion Rotation { get; set; }

        public byte[] Bezier { get; set; }

        public VMDBoneFrame()
        {
            this.Translation = new PMXVector3();
            this.Rotation = new PMXQuaternion();

            this.Bezier = new byte[64];
        }            

        public void LoadFromStream(BinaryReader br)
        {
            this.BoneName = VMDString.ReadString(br, 15);
            this.FrameNumber = br.ReadUInt32();
            this.Translation = PMXVector3.LoadFromStreamStatic(br);
            this.Rotation = PMXQuaternion.LoadFromStreamStatic(br);

            this.Bezier = new byte[64];
            br.BaseStream.Read(this.Bezier, 0, 64);
        }

        public void SaveToStream(BinaryWriter bw)
        {
            VMDString.WriteString(bw, 15, this.BoneName);
            bw.Write(this.FrameNumber);
            this.Translation.WriteToStream(bw);
            this.Rotation.WriteToStream(bw);

            bw.BaseStream.Write(this.Bezier, 0, 64);
        }
    }
}
