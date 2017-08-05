using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts
{
    public class PMXTriangle : PMXBasePart
    {
        public PMXVertex Vertex1 { get; set; }
        public PMXVertex Vertex2 { get; set; }
        public PMXVertex Vertex3 { get; set; }

        public PMXTriangle(PMXModel model) : base(model)
        {
        }

        public override void FinaliseAfterImport()
        {
            throw new NotImplementedException();
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            int index1 = PMXParser.ReadIndex(br, importSettings.BitSettings.VertexIndexLength);
            int index2 = PMXParser.ReadIndex(br, importSettings.BitSettings.VertexIndexLength);
            int index3 = PMXParser.ReadIndex(br, importSettings.BitSettings.VertexIndexLength);

            this.Vertex1 = this.Model.Vertices[index1];
            this.Vertex2 = this.Model.Vertices[index2];
            this.Vertex3 = this.Model.Vertices[index3];
        }
    }
}
