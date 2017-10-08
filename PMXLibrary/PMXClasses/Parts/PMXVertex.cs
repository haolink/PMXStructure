using System;
using System.Collections.Generic;
using System.IO;

namespace PMXStructure.PMXClasses
{
    public class PMXVertex : PMXBasePart
    {
        public PMXVector3 Position { get; set; }
        public PMXVector3 Normals { get; set; }

        public float OutlineMagnification { get; set; }

        public PMXVector2 UV { get; set; }

        public List<PMXQuaternion> AddedUVs { get; private set; }

        public PMXBaseDeform Deform { get; set; }

        private Dictionary<int, int> _exportHashNumbers; //Export only

        public PMXVertex(PMXModel model) : base(model)
        {
            this.AddedUVs = new List<PMXQuaternion>();

            this.Position = new PMXVector3();
            this.Normals = new PMXVector3();
            this.UV = new PMXVector2();

            PMXVertexDeformBDEF1 df = new PMXVertexDeformBDEF1(this.Model, this);
            this.Deform = df;

            if (this.Model.Bones.Count > 0)
            {
                df.Bone1 = this.Model.Bones[0];
            }

            this._exportHashNumbers = new Dictionary<int, int>();
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.Position = PMXVector3.LoadFromStreamStatic(br);
            this.Normals = PMXVector3.LoadFromStreamStatic(br);
            this.UV = PMXVector2.LoadFromStreamStatic(br);

            
            if (importSettings.Format == MMDImportSettings.ModelFormat.PMX)
            { //PMX format
                for (int i = 0; i < importSettings.ExtendedUV; i++)
                {
                    this.AddedUVs.Add(PMXQuaternion.LoadFromStreamStatic(br));
                }

                byte deformType = br.ReadByte();

                switch (deformType)
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
            else
            { //PMD format
                this.Deform = PMXVertexDeformBDEF2.DeformFromPMDFile(this.Model, this, br);

                this.OutlineMagnification = ((br.ReadByte() == 0) ? 1.0f : 0.0f);
            }
        }

        private static int cnt = 0;
        public override void FinaliseAfterImport()
        {
            try
            {
                this.Deform.FinaliseAfterImport();
                cnt++;
            } catch(Exception ex)
            {
                Console.WriteLine(cnt);
                throw ex;
            }
            
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            this.Position.WriteToStream(bw);
            this.Normals.WriteToStream(bw);        
            this.UV.WriteToStream(bw);

            if (exportSettings.Format == MMDExportSettings.ModelFormat.PMX)
            {
                //PMX format
                for (int i = 0; i < exportSettings.ExtendedUV; i++)
                {
                    PMXQuaternion set = ((i >= this.AddedUVs.Count) ? (new PMXQuaternion()) : this.AddedUVs[i]);
                    set.WriteToStream(bw);
                }
            
                this.Deform.WriteToStream(bw, exportSettings);

                bw.Write(this.OutlineMagnification);
            }
            else
            {
                this.Deform.WriteToStream(bw, exportSettings);

                bw.Write((byte)((this.OutlineMagnification > 0.2f) ? 0:1));
            }
        }

        /// <summary>
        /// Checks if the bone is part of a given model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int CheckIndexInModel(PMXVertex vtx, MMDExportSettings settings)
        {
            if (vtx == null)
            {
                throw new InvalidDataException("Vertex mustn't be null!");
            }

            if(vtx._exportHashNumbers.ContainsKey(settings.ExportHash))
            {
                return vtx._exportHashNumbers[settings.ExportHash];
            }

            PMXModel model = settings.Model;

            int index = model.Vertices.IndexOf(vtx);
            if (index < 0)
            {
                throw new InvalidDataException("Vertex not a member of model!");
            }
            return index;
        }

        /// <summary>
        /// Normalising the normal vector.
        /// </summary>
        public void NormalizeNormalVector()
        {
            this.Normals = this.Normals.Normalize();
        }

        /// <summary>
        /// Set a vertex index into the vertex to allow very quick exporting.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="settings"></param>
        public void AddIndexForExport(MMDExportSettings settings, int index)
        {
            this._exportHashNumbers[settings.ExportHash] = index;
        }

        /// <summary>
        /// Remove a vertex index into the vertex to allow very quick exporting.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="settings"></param>
        public void RemoveIndexForExport(MMDExportSettings settings)
        {
            if(this._exportHashNumbers.ContainsKey(settings.ExportHash))
            {
                this._exportHashNumbers.Remove(settings.ExportHash);
            }            
        }
    }
}
