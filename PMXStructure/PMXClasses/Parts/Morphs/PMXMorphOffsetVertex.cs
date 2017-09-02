using System;
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
            //Not required
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            int vtxIndex;

            if(importSettings.Format == MMDImportSettings.ModelFormat.PMX)
            { //PMX
                vtxIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.VertexIndexLength);
            }
            else
            { //PMD
                vtxIndex = br.ReadInt32();

                if(importSettings.BaseMorph != null)
                {
                    if(vtxIndex < importSettings.BaseMorph.Offsets.Count)
                    {
                        PMXMorphOffsetVertex mov = (PMXMorphOffsetVertex)importSettings.BaseMorph.Offsets[vtxIndex];
                        vtxIndex = this.Model.Vertices.IndexOf(mov.Vertex);
                    }
                }
            }

            this.Vertex = this.Model.Vertices[vtxIndex];
            this.Translation = PMXVector3.LoadFromStreamStatic(br);
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            if (exportSettings.Format == MMDExportSettings.ModelFormat.PMX)
            { //PMX
                PMXParser.WriteIndex(bw, exportSettings.BitSettings.VertexIndexLength, PMXVertex.CheckIndexInModel(this.Vertex, exportSettings));
            }
            else
            { //PMD
                PMXParser.WriteIndex(bw, 4, exportSettings.BaseMorphVertices.IndexOf(this.Vertex));
            }

            this.Translation.WriteToStream(bw);
        }
    }
}
