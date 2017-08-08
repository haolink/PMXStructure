using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.General;

namespace PMXStructure.PMXClasses.Parts.Morphs
{
    class PMXMorphOffsetUV : PMXMorphOffsetBase
    {
        public PMXVertex Vertex { get; set; }
        public PMXVector2 UVTranslation { get; set; }
        public PMXVector2 UVTranslation2 { get; set; }

        public enum UVAddIndexType
        {
            UV,
            AddUV1,
            AddUV2,
            AddUV3,
            AddUV4
        }

        public PMXMorphOffsetUV(PMXModel model, PMXMorph morph, UVAddIndexType index = UVAddIndexType.UV) : base(model, morph)
        {
            switch (index)
            {
                case UVAddIndexType.UV:
                    this.MorphTargetType = PMXMorph.MORPH_IDENTIFY_UV;
                    break;
                case UVAddIndexType.AddUV1:
                    this.MorphTargetType = PMXMorph.MORPH_IDENTIFY_UV_EXTENDED1;
                    break;
                case UVAddIndexType.AddUV2:
                    this.MorphTargetType = PMXMorph.MORPH_IDENTIFY_UV_EXTENDED2;
                    break;
                case UVAddIndexType.AddUV3:
                    this.MorphTargetType = PMXMorph.MORPH_IDENTIFY_UV_EXTENDED3;
                    break;
                case UVAddIndexType.AddUV4:
                    this.MorphTargetType = PMXMorph.MORPH_IDENTIFY_UV_EXTENDED4;
                    break;
                default:
                    this.MorphTargetType = PMXMorph.MORPH_IDENTIFY_UV;
                    break;
            }

            this.UVTranslation = new PMXVector2();
            this.UVTranslation2 = new PMXVector2();
        }

        public override void FinaliseAfterImport()
        {
            //Not required
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            int vtxIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.VertexIndexLength);

            this.Vertex = this.Model.Vertices[vtxIndex];
            this.UVTranslation = PMXVector2.LoadFromStreamStatic(br);
            this.UVTranslation2 = PMXVector2.LoadFromStreamStatic(br);
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            PMXParser.WriteIndex(bw, exportSettings.BitSettings.VertexIndexLength, PMXVertex.CheckIndexInModel(this.Vertex, exportSettings));

            this.UVTranslation.WriteToStream(bw);
            this.UVTranslation2.WriteToStream(bw);
        }
    }
}
