using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.General;
using PMXStructure.PMXClasses.Parts.VertexDeform;

namespace PMXStructure.PMXClasses.Parts
{
    public class PMXVertex : PMXBasePart
    {       
        public class AddUVSet
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float W { get; set; }
        }

        public PMXVector3 Position { get; set; }
        public PMXVector3 Normals { get; set; }

        public float OutlineMagnification { get; set; }

        public PMXVector2 UV { get; set; }

        public List<AddUVSet> AddedUVs { get; private set; }

        public PMXBaseDeform Deform { get; set; }        

        public PMXVertex(PMXModel model) : base(model)
        {
            this.AddedUVs = new List<AddUVSet>();

            this.Position = new PMXVector3();
            this.Normals = new PMXVector3();
            this.UV = new PMXVector2();
            this.Deform = new PMXVertexDeformBDEF1(this.Model, this);
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.Position = PMXVector3.LoadFromStreamStatic(br);
            this.Normals = PMXVector3.LoadFromStreamStatic(br);
            this.UV = PMXVector2.LoadFromStreamStatic(br);            

            for (int i = 0; i < importSettings.ExtendedUV; i++)
            {
                AddUVSet aus = new AddUVSet();
                aus.X = br.ReadSingle();
                aus.Y = br.ReadSingle();
                aus.Z = br.ReadSingle();
                aus.W = br.ReadSingle();
                this.AddedUVs.Add(aus);
            }

            byte deformType = br.ReadByte();
            
            switch(deformType)
            {
                case PMXBaseDeform.DEFORM_IDENTIFY_BDEF1:
                    this.Deform = new PMXVertexDeformBDEF1(this.Model, this);
                    break;
                case PMXBaseDeform.DEFORM_IDENTIFY_BDEF2:
                    this.Deform = new PMXVertexDeformBDEF2(this.Model, this);
                    break;
                case PMXBaseDeform.DEFORM_IDENTIFY_BDEF4:
                    this.Deform = new PMXVertexDeformBDEF4(this.Model, this);
                    break;
                case PMXBaseDeform.DEFORM_IDENTIFY_SDEF:
                    this.Deform = new PMXVertexDeformSDEF(this.Model, this);
                    break;
                case PMXBaseDeform.DEFORM_IDENTIFY_QDEF:
                default:
                    this.Deform = new PMXVertexDeformQDEF(this.Model, this);
                    break;
            }
            this.Deform.LoadFromStream(br, importSettings);

            this.OutlineMagnification = br.ReadSingle();
        }

        public override void FinaliseAfterImport()
        {
            throw new NotImplementedException();
        }
    }
}
