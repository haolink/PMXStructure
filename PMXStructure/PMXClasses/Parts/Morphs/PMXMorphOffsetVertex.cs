using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.General;

namespace PMXStructure.PMXClasses.Parts.Morphs
{
    class PMXMorphOffsetVertex : PMXMorphOffsetBase
    {
        public PMXVertex Vertex { get; set; }
        public PMXVector3 Translation { get; set; }

        public PMXMorphOffsetVertex(PMXModel model, PMXMorph morph) : base(model, morph)
        {
            this.MorphTargetType = PMXMorph.MORPH_IDENTIFY_VERTEX;

            this.Translation = new PMXVector3();
        }

        public override void FinaliseAfterImport()
        {
            throw new NotImplementedException();
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            int vtxIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.VertexIndexLength);

            this.Vertex = this.Model.Vertices[vtxIndex];
            this.Translation = PMXVector3.LoadFromStreamStatic(br);
        }
    }
}
