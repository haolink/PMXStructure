using System;
using System.IO;
using System.Collections.Generic;

using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts.VertexDeform
{
    class PMXVertexDeformBDEF2 : PMXVertexDeformBDEF1
    {
        public float Bone1Weight { get; set; } //Bone 1 weighing
        public PMXBone Bone2 { get; set; } //Bone 2
        protected int bone2Index; //Import only!

        public PMXVertexDeformBDEF2(PMXModel model, PMXVertex vertex) : base(model, vertex)
        {
            this.deformIdentifier = PMXBaseDeform.DEFORM_IDENTIFY_BDEF2;
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            base.LoadFromStream(br, importSettings);
            this.bone2Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
            this.Bone1Weight = br.ReadSingle();
        }

        public override void FinaliseAfterImport()
        {
            base.FinaliseAfterImport();
            this.Bone2 = this.Model.Bones[bone2Index];
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            if(exportSettings.Format == MMDExportSettings.ModelFormat.PMX)
            {
                base.WriteToStream(bw, exportSettings);

                PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.Bone2, exportSettings));
                bw.Write(this.Bone1Weight);
            }
            else
            {
                int b1i = PMXBone.CheckIndexInModel(this.Bone1, exportSettings);
                int b2i = PMXBone.CheckIndexInModel(this.Bone2, exportSettings);
                
                List<KeyValuePair<int, float>> sortKeys = new List<KeyValuePair<int, float>>();
                if (b1i >= 0) { sortKeys.Add(new KeyValuePair<int, float>(b1i, this.Bone1Weight)); }
                if (b2i >= 0) { sortKeys.Add(new KeyValuePair<int, float>(b2i, 1.0f - this.Bone1Weight)); }                

                this.ExportToPMDBase(sortKeys, bw);
            }
        }

        public static PMXBaseDeform DeformFromPMDFile(PMXModel model, PMXVertex vertex, BinaryReader br)
        {
            ushort boneId1 = br.ReadUInt16();
            ushort boneId2 = br.ReadUInt16();
            byte weight = br.ReadByte();

            if(weight >= 100)
            { //BDEF1
                return PMXVertexDeformBDEF1.DeformFromPMDFile(model, vertex, boneId1);                
            } 
            else if(weight <= 0)
            { //BDEF 1 as well
                return PMXVertexDeformBDEF1.DeformFromPMDFile(model, vertex, boneId2);
            }            
            else
            { //BDEF2
                PMXVertexDeformBDEF2 res = new PMXVertexDeformBDEF2(model, vertex);
                res.bone1Index = (int)boneId1;
                res.bone2Index = (int)boneId2;
                res.Bone1Weight = (float)weight / 100.0f;
                return res;
            }
            
        }
    }
}
