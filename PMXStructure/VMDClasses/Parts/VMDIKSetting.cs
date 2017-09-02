using System;

using System.IO;
using PMXStructure.PMXClasses.General;

using PMXStructure.VMDClasses.Helpers;

namespace PMXStructure.VMDClasses.Parts
{
    public class VMDIKSetting
    {
        public string IKName { get; set; }
        public bool Enable { get; set; }

        public VMDIKSetting()
        {            
        }

        public void LoadFromStream(BinaryReader br)
        {
            this.IKName = VMDString.ReadString(br, 20);
            this.Enable = (br.ReadByte() == 1);            
        }

        public void SaveToStream(BinaryWriter bw)
        {
            VMDString.WriteString(bw, 20, this.IKName);
            bw.Write((byte)(this.Enable ? 1 : 0));
        }
    }
}
