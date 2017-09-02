using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using PMXStructure.VMDClasses.Helpers;
using PMXStructure.VMDClasses.Parts;

namespace PMXStructure.VMDClasses
{
    public class VMDFile
    {
        public string ModelName { get; set; }
        public List<VMDBoneFrame> Bones { get; set; }
        public List<VMDMorphFrame> Morphs { get; set; }
        public List<VMDCameraFrame> Camera { get; set; }
        public List<VMDLightFrame> Light { get; set; }
        public List<VMDShadowFrame> Shadow { get; set; }
        public List<VMDIKFrame> IK { get; set; }

        public VMDFile()
        {
            this.Bones = new List<VMDBoneFrame>();
            this.Morphs = new List<VMDMorphFrame>();
            this.Camera = new List<VMDCameraFrame>();
            this.Light = new List<VMDLightFrame>();
            this.Shadow = new List<VMDShadowFrame>();
            this.IK = new List<VMDIKFrame>();
        }

        public static VMDFile LoadFromFile(string vmdFile)
        {
            FileStream fs = new FileStream(vmdFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            VMDFile vf = VMDFile.LoadFromStream(fs);

            fs.Close();
            fs = null;

            return vf;
        }

        public static VMDFile LoadFromStream(Stream s)
        {
            BinaryReader br = new BinaryReader(s);

            string head = VMDString.ReadString(br, 30, Encoding.ASCII);

            if(head != "Vocaloid Motion Data 0002")
            {
                br = null;
                return null;
            }

            
            VMDFile vf = new VMDFile();

            vf.ModelName = VMDString.ReadString(br, 20);

            uint boneCount = br.ReadUInt32();
            for (int i = 0; i < boneCount; i++)
            {
                VMDBoneFrame vbf = new VMDBoneFrame();
                vbf.LoadFromStream(br);
                vf.Bones.Add(vbf);
            }

            uint morphCount = br.ReadUInt32();
            for (int i = 0; i < morphCount; i++)
            {
                VMDMorphFrame vmf = new VMDMorphFrame();
                vmf.LoadFromStream(br);
                vf.Morphs.Add(vmf);
            }

            uint cameraCount = br.ReadUInt32();
            for (int i = 0; i < cameraCount; i++)
            {
                VMDCameraFrame vcf = new VMDCameraFrame();
                vcf.LoadFromStream(br);
                vf.Camera.Add(vcf);
            }

            uint lightCount = br.ReadUInt32();
            for (int i = 0; i < lightCount; i++)
            {
                VMDLightFrame vlf = new VMDLightFrame();
                vlf.LoadFromStream(br);
                vf.Light.Add(vlf);
            }

            uint shadowCount = br.ReadUInt32();
            for (int i = 0; i < shadowCount; i++)
            {
                VMDShadowFrame vsf = new VMDShadowFrame();
                vsf.LoadFromStream(br);
                vf.Shadow.Add(vsf);
            }

            uint ikCount = br.ReadUInt32();
            for (int i = 0; i < ikCount; i++)
            {
                VMDIKFrame vif = new VMDIKFrame();
                vif.LoadFromStream(br);
                vf.IK.Add(vif);
            }

            return vf;
        }

        public void SaveToFile(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);

            this.SaveToStream(fs);

            fs = null;           
        }

        public void SaveToStream(Stream s)
        {
            BinaryWriter bw = new BinaryWriter(s);

            VMDString.WriteString(bw, 30, "Vocaloid Motion Data 0002", Encoding.ASCII);

            VMDString.WriteString(bw, 20, this.ModelName);

            bw.Write((uint)this.Bones.Count);
            foreach(VMDBoneFrame vbf in this.Bones)
            {
                vbf.SaveToStream(bw);
            }

            bw.Write((uint)this.Morphs.Count);
            foreach (VMDMorphFrame vmf in this.Morphs)
            {
                vmf.SaveToStream(bw);
            }

            bw.Write((uint)this.Camera.Count);
            foreach (VMDCameraFrame vcf in this.Camera)
            {
                vcf.SaveToStream(bw);
            }

            bw.Write((uint)this.Light.Count);
            foreach (VMDLightFrame vlf in this.Light)
            {
                vlf.SaveToStream(bw);
            }

            bw.Write((uint)this.Shadow.Count);
            foreach (VMDShadowFrame vsf in this.Shadow)
            {
                vsf.SaveToStream(bw);
            }

            bw.Write((uint)this.IK.Count);
            foreach (VMDIKFrame vif in this.IK)
            {
                vif.SaveToStream(bw);
            }
        }
    }
}
