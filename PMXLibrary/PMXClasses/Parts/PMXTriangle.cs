using System;
using System.IO;

namespace PMXStructure.PMXClasses
{
    public class PMXTriangle : PMXBasePart
    {
        public PMXVertex Vertex1 { get; set; }
        public PMXVertex Vertex2 { get; set; }
        public PMXVertex Vertex3 { get; set; }

        public PMXTriangle(PMXModel model) : base(model)
        {
        }

        public PMXTriangle(PMXModel model, PMXVertex vertex1, PMXVertex vertex2, PMXVertex vertex3) : this(model)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Vertex3 = vertex3;
        }

        public override void FinaliseAfterImport()
        {
            //Not required
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            int index1, index2, index3;

            if (importSettings.Format == MMDImportSettings.ModelFormat.PMX)
            { //PMX
                index1 = PMXParser.ReadIndex(br, importSettings.BitSettings.VertexIndexLength);
                index2 = PMXParser.ReadIndex(br, importSettings.BitSettings.VertexIndexLength);
                index3 = PMXParser.ReadIndex(br, importSettings.BitSettings.VertexIndexLength);
            } 
            else
            { //PMD
                index1 = (int)br.ReadUInt16();
                index2 = (int)br.ReadUInt16();
                index3 = (int)br.ReadUInt16();
            }            

            this.Vertex1 = this.Model.Vertices[index1];
            this.Vertex2 = this.Model.Vertices[index2];
            this.Vertex3 = this.Model.Vertices[index3];
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            byte indexLength = ((exportSettings.Format == MMDExportSettings.ModelFormat.PMX) ? exportSettings.BitSettings.VertexIndexLength : (byte)2);

            PMXParser.WriteIndex(bw, indexLength, PMXVertex.CheckIndexInModel(this.Vertex1, exportSettings));
            PMXParser.WriteIndex(bw, indexLength, PMXVertex.CheckIndexInModel(this.Vertex2, exportSettings));
            PMXParser.WriteIndex(bw, indexLength, PMXVertex.CheckIndexInModel(this.Vertex3, exportSettings));
        }
    }
}
