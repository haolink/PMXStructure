using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.General;

namespace PMXStructure.PMXClasses.Parts.Morphs
{
    class PMXMorphOffsetMaterial : PMXMorphOffsetBase
    {
        public enum MaterialMorphOffsetType
        {
            Multiplication = 0,
            Addition = 1
        }

        public PMXMaterial Material { get; set; }
        public MaterialMorphOffsetType Type { get; set; }

        public PMXColorRGBA Diffuse { get; set; }
        public PMXColorRGBA Specular { get; set; }
        public PMXColorRGB Ambient { get; set; }

        public PMXColorRGBA EdgeColor { get; set; }
        public float EdgeSize { get; set; }

        public PMXColorRGBA TextureFactor { get; set; }
        public PMXColorRGBA SphereTextureFactor { get; set; }
        public PMXColorRGBA ToonTextureFactor { get; set; }

        public PMXMorphOffsetMaterial(PMXModel model, PMXMorph morph) : base(model, morph)
        {
            this.MorphTargetType = PMXMorph.MORPH_IDENTIFY_MATERIAL;

            this.Diffuse = new PMXColorRGBA();
            this.Specular = new PMXColorRGBA();
            this.Ambient = new PMXColorRGB();
            this.EdgeColor = new PMXColorRGBA();
            this.TextureFactor = new PMXColorRGBA();
            this.SphereTextureFactor = new PMXColorRGBA();
            this.ToonTextureFactor = new PMXColorRGBA();
        }

        public override void FinaliseAfterImport()
        {
            //Not required
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            int mtIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.MaterialIndexLength);

            if(mtIndex < 0)
            {
                this.Material = null;
            }
            else
            {
                this.Material = this.Model.Materials[mtIndex];
            }
            
            this.Type = (MaterialMorphOffsetType)(int)br.ReadByte();

            this.Diffuse = PMXColorRGBA.LoadFromStreamStatic(br);
            this.Specular = PMXColorRGBA.LoadFromStreamStatic(br);
            this.Ambient = PMXColorRGB.LoadFromStreamStatic(br);

            this.EdgeColor = PMXColorRGBA.LoadFromStreamStatic(br);
            this.EdgeSize = br.ReadSingle();

            this.TextureFactor = PMXColorRGBA.LoadFromStreamStatic(br);
            this.SphereTextureFactor = PMXColorRGBA.LoadFromStreamStatic(br);
            this.ToonTextureFactor = PMXColorRGBA.LoadFromStreamStatic(br);
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            PMXParser.WriteIndex(bw, exportSettings.BitSettings.MaterialIndexLength, PMXMaterial.CheckIndexInModel(this.Material, exportSettings, true));

            bw.Write((byte)(int)this.Type);

            this.Diffuse.WriteToStream(bw);
            this.Specular.WriteToStream(bw);
            this.Ambient.WriteToStream(bw);

            this.EdgeColor.WriteToStream(bw);
            bw.Write(this.EdgeSize);

            this.TextureFactor.WriteToStream(bw);
            this.SphereTextureFactor.WriteToStream(bw);
            this.ToonTextureFactor.WriteToStream(bw);
        }
    }
}
