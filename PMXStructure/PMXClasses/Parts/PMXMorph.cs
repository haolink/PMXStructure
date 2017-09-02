using System;
using System.IO;
using System.Collections.Generic;

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
            Eyes = 2,
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
            int offsets;
            byte morphType;

            if(importSettings.Format == MMDImportSettings.ModelFormat.PMX)
            { //PMX format
                this.NameJP = PMXParser.ReadString(br, importSettings.TextEncoding);
                this.NameEN = PMXParser.ReadString(br, importSettings.TextEncoding);

                this.Panel = (PanelType)(int)br.ReadByte();
                morphType = br.ReadByte();

                offsets = br.ReadInt32();                
            }
            else
            { //PMD format
                this.NameJP = PMDParser.ReadString(br, 20, importSettings.TextEncoding);

                offsets = br.ReadInt32();
                this.Panel = (PanelType)(int)br.ReadByte();

                morphType = PMXMorph.MORPH_IDENTIFY_VERTEX;
            }

            for (int i = 0; i < offsets; i++)
            {
                PMXMorphOffsetBase mb = null;

                switch (morphType)
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
            foreach(PMXMorphOffsetBase offset in this.Offsets)
            {
                offset.FinaliseAfterImport();
            }
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            if (exportSettings.Format == MMDExportSettings.ModelFormat.PMX)
            { //PMX format
                PMXParser.WriteString(bw, exportSettings.TextEncoding, this.NameJP);
                PMXParser.WriteString(bw, exportSettings.TextEncoding, this.NameEN);

                bw.Write((byte)(int)this.Panel);

                if (this.Offsets.Count == 0)
                {
                    bw.Write((byte)PMXMorph.MORPH_IDENTIFY_VERTEX);
                    bw.Write((Int32)0);
                }
                else
                {
                    byte morphTypeId = this.Offsets[0].MorphTargetType;
                    bw.Write((byte)morphTypeId);
                    bw.Write((Int32)this.Offsets.Count);

                    foreach (PMXMorphOffsetBase offset in this.Offsets)
                    {
                        if (offset.MorphTargetType != morphTypeId)
                        {
                            throw new InvalidDataException("Morph offset types mustn't be mixed types");
                        }
                        offset.WriteToStream(bw, exportSettings);
                    }
                }
            } //PMD format
            else
            {
                PMDParser.WriteString(bw, 20, exportSettings.TextEncoding, this.NameJP);
                bw.Write((Int32)this.Offsets.Count);
                bw.Write((byte)(int)this.Panel);
                foreach (PMXMorphOffsetBase offset in this.Offsets)
                {
                    if (!(offset is PMXMorphOffsetVertex))
                    {
                        throw new InvalidDataException("PMD only supports vertex morphs.");
                    }
                    offset.WriteToStream(bw, exportSettings);
                }
            }
        }

        /// <summary>
        /// Checks if the morph is part of a given model.
        /// </summary>
        /// <param name="bn"></param>
        /// <param name="exportSettings"></param>
        /// <param name="nullAcceptable"></param>
        /// <returns></returns>
        public static int CheckIndexInModel(PMXMorph mrph, MMDExportSettings exportSettings, bool nullAcceptable = true)
        {
            if (mrph == null)
            {
                if (nullAcceptable)
                {
                    return -1;
                }
                else
                {
                    throw new InvalidDataException("Morph mustn't be null!");
                }
            }

            PMXModel model = exportSettings.Model;

            int index = model.Morphs.IndexOf(mrph);
            if (index < 0)
            {
                throw new InvalidDataException("Morph not a member of model!");
            }
            return index;
        }
    }
}
