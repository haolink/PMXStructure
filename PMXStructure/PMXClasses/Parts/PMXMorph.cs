using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.Parts.Morphs;

namespace PMXStructure.PMXClasses.Parts
{
    public class PMXMorph : PMXBasePart
    {
        public const byte MORPH_IDENTIFY_GROUP = 0;
        public const byte MORPH_IDENTIFY_VERTEX = 1;
        public const byte MORPH_IDENTIFY_BONE = 2;
        public const byte MORPH_IDENTIFY_UV = 3;
        public const byte MORPH_IDENTIFY_UV_EXTENDED1 = 4;
        public const byte MORPH_IDENTIFY_UV_EXTENDED2 = 5;
        public const byte MORPH_IDENTIFY_UV_EXTENDED3 = 6;
        public const byte MORPH_IDENTIFY_UV_EXTENDED4 = 7;
        public const byte MORPH_IDENTIFY_MATERIAL = 8;

        public enum PanelType
        {
            None = 0,
            Brows = 1,
            Lips = 2,
            Mouth = 3,
            Other = 4
        }

        public string NameJP { get; set; }
        public string NameEN { get; set; }

        public PanelType Panel { get; set; }

        public List<PMXMorphOffsetBase> Offsets { get; }

        public PMXMorph(PMXModel model) : base(model)
        {
            this.Offsets = new List<PMXMorphOffsetBase>();
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.NameJP = PMXParser.ReadString(br, importSettings.TextEncoding);
            this.NameEN = PMXParser.ReadString(br, importSettings.TextEncoding);

            this.Panel = (PanelType)(int)br.ReadByte();
            byte morphType = br.ReadByte();

            int offsets = br.ReadInt32();

            for(int i = 0; i < offsets; i++)
            {
                PMXMorphOffsetBase mb = null;

                switch(morphType)
                {
                    case PMXMorph.MORPH_IDENTIFY_GROUP:
                        mb = new PMXMorphOffsetGroup(this.Model, this);
                        break;
                    case PMXMorph.MORPH_IDENTIFY_VERTEX:
                        mb = new PMXMorphOffsetVertex(this.Model, this);
                        break;
                    case PMXMorph.MORPH_IDENTIFY_BONE:
                        mb = new PMXMorphOffsetBone(this.Model, this);
                        break;
                    case PMXMorph.MORPH_IDENTIFY_UV:
                        mb = new PMXMorphOffsetUV(this.Model, this, PMXMorphOffsetUV.UVAddIndexType.UV);
                        break;
                    case PMXMorph.MORPH_IDENTIFY_UV_EXTENDED1:
                        mb = new PMXMorphOffsetUV(this.Model, this, PMXMorphOffsetUV.UVAddIndexType.AddUV1);
                        break;
                    case PMXMorph.MORPH_IDENTIFY_UV_EXTENDED2:
                        mb = new PMXMorphOffsetUV(this.Model, this, PMXMorphOffsetUV.UVAddIndexType.AddUV2);
                        break;
                    case PMXMorph.MORPH_IDENTIFY_UV_EXTENDED3:
                        mb = new PMXMorphOffsetUV(this.Model, this, PMXMorphOffsetUV.UVAddIndexType.AddUV3);
                        break;
                    case PMXMorph.MORPH_IDENTIFY_UV_EXTENDED4:
                        mb = new PMXMorphOffsetUV(this.Model, this, PMXMorphOffsetUV.UVAddIndexType.AddUV4);
                        break;
                    case PMXMorph.MORPH_IDENTIFY_MATERIAL:
                        mb = new PMXMorphOffsetMaterial(this.Model, this);
                        break;
                    default:
                        throw new InvalidDataException("Unknown morph type " + morphType);                        
                }
                mb.LoadFromStream(br, importSettings);
                this.Offsets.Add(mb);
            }
        }

        public override void FinaliseAfterImport()
        {
            throw new NotImplementedException();
        }
    }
}
