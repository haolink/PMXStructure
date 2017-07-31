using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts
{
    public class PMXVertex : PMXBasePart
    {
        public enum VertexWeighingType
        {
            BDEF1 = 0,
            BDEF2 = 1,
            BDEF4 = 2,
            SDEF = 3,
            QDEF = 4
        }

        public class AddUVSet
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float W { get; set; }
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float NormalX { get; set; }
        public float NormalY { get; set; }
        public float NormalZ { get; set; }

        public float OutlineMagnification { get; set; }

        public float U { get; set; }
        public float V { get; set; }

        public List<AddUVSet> AddedUVs { get; private set; }

        public VertexWeighingType WeighingType { get; set; }

        public PMXBone Bone1 { get; set; } //Bone 1
        public float Bone1Weight { get; set; } //Bone 1 weighing
        private int bone1Index; //Import only!

        public PMXBone Bone2 { get; set; } //Bone 2
        public float Bone2Weight { get; set; } //Bone 2 weighing
        private int bone2Index; //Import only!

        public PMXBone Bone3 { get; set; } //Bone 3
        public float Bone3Weight { get; set; } //Bone 3 weighing
        private int bone3Index; //Import only!

        public PMXBone Bone4 { get; set; } //Bone 4
        public float Bone4Weight { get; set; } //Bone 4 weighing
        private int bone4Index; //Import only!

        //SDEF related settings
        public float SDEX_C_X { get; set; }
        public float SDEX_C_Y { get; set; }
        public float SDEX_C_Z { get; set; }
        public float SDEX_R0_X { get; set; }
        public float SDEX_R0_Y { get; set; }
        public float SDEX_R0_Z { get; set; }
        public float SDEX_R1_X { get; set; }
        public float SDEX_R1_Y { get; set; }
        public float SDEX_R1_Z { get; set; }

        public PMXVertex(PMXModel model) : base(model)
        {
            this.AddedUVs = new List<AddUVSet>();
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.X = br.ReadSingle();
            this.Y = br.ReadSingle();
            this.Z = br.ReadSingle();

            this.NormalX = br.ReadSingle();
            this.NormalY = br.ReadSingle();
            this.NormalZ = br.ReadSingle();

            this.U = br.ReadSingle();
            this.V = br.ReadSingle();

            for (int i = 0; i < importSettings.ExtendedUV; i++)
            {
                AddUVSet aus = new AddUVSet();
                aus.X = br.ReadSingle();
                aus.Y = br.ReadSingle();
                aus.Z = br.ReadSingle();
                aus.W = br.ReadSingle();
                this.AddedUVs.Add(aus);
            }

            this.WeighingType = (VertexWeighingType)br.BaseStream.ReadByte();

            switch(this.WeighingType)
            {
                case VertexWeighingType.BDEF1:
                    this.bone1Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
                    break;
                case VertexWeighingType.BDEF2:
                    this.bone1Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
                    this.Bone1Weight = br.ReadSingle();
                    this.bone2Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
                    break;
                case VertexWeighingType.BDEF4:
                case VertexWeighingType.QDEF:
                    this.bone1Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
                    this.Bone1Weight = br.ReadSingle();
                    this.bone2Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
                    this.Bone2Weight = br.ReadSingle();
                    this.bone3Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
                    this.Bone3Weight = br.ReadSingle();
                    this.bone4Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
                    this.Bone4Weight = br.ReadSingle();
                    break;
                case VertexWeighingType.SDEF:
                    this.bone1Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
                    this.Bone1Weight = br.ReadSingle();
                    this.bone2Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);

                    this.SDEX_C_X = br.ReadSingle();
                    this.SDEX_C_Y = br.ReadSingle();
                    this.SDEX_C_Z = br.ReadSingle();
                    this.SDEX_R0_X = br.ReadSingle();
                    this.SDEX_R0_Y = br.ReadSingle();
                    this.SDEX_R0_Z = br.ReadSingle();
                    this.SDEX_R1_X = br.ReadSingle();
                    this.SDEX_R1_Y = br.ReadSingle();
                    this.SDEX_R1_Z = br.ReadSingle();
                    break;
            }

            this.OutlineMagnification = br.ReadSingle();
        }

        public override void FinaliseAfterImport()
        {
            throw new NotImplementedException();
        }
    }
}
