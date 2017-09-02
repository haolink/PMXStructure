using System;


using System.IO;
using PMXStructure.PMXClasses.General;

using PMXStructure.VMDClasses.Helpers;

namespace PMXStructure.VMDClasses.Parts
{
    public class VMDCameraFrame
    {
        public uint FrameNumber { get; set; }
        public float Distance { get; set; }
        public PMXVector3 Position { get; set; }
        public PMXVector3 Rotation { get; set; }

        public byte[] Bezier { get; set; }

        public uint ViewAngle { get; set; }
        public bool Perspective { get; set; }

        public VMDCameraFrame()
        {
            this.Position = new PMXVector3();
            this.Rotation = new PMXVector3();

            this.Perspective = true;

            this.Bezier = new byte[24];
        }

        public void LoadFromStream(BinaryReader br)
        {
            this.FrameNumber = br.ReadUInt32();
            this.Distance = br.ReadSingle();
            this.Position = PMXVector3.LoadFromStreamStatic(br);
            this.Rotation = PMXVector3.LoadFromStreamStatic(br);
            this.Rotation.X *= (-1);

            this.Bezier = new byte[24];
            br.BaseStream.Read(this.Bezier, 0, 24);

            this.ViewAngle = br.ReadUInt32();
            this.Perspective = (br.ReadByte() == 1);
        }

        public void SaveToStream(BinaryWriter bw)
        {
            bw.Write(this.FrameNumber);
            bw.Write(this.Distance);
            this.Position.WriteToStream(bw);

            this.Rotation.X *= (-1);
            this.Rotation.WriteToStream(bw);
            this.Rotation.X *= (-1);
            
            bw.BaseStream.Write(this.Bezier, 0, 24);

            bw.Write(this.ViewAngle);
            bw.Write((byte)(this.Perspective ? 1 : 0));
        }
    }
}
