using System;
using System.IO;
using System.Collections.Generic;

namespace PMXStructure.PMXClasses
{
    public class PMXMaterial : PMXBasePart
    {
        public const byte MATERIAL_DOUBLE_SIDED = 0x01;
        public const byte MATERIAL_GROUND_SHADOW = 0x02;
        public const byte MATERIAL_SELF_SHADOW = 0x04;
        public const byte MATERIAL_SELF_SHADOW_PLUS = 0x08;
        public const byte MATERIAL_EDGE_ENABLED = 0x10;
        public const byte MATERIAL_VERTEX_COLOR = 0x20;

        public enum PMXSphereMode
        {
            Disabled = 0,
            Multiplication = 1,
            Addition = 2,
            Texture = 3
        }

        public enum PMXGroundShadowType
        {
            Tri = 0,
            Point = 1,
            Line = 2
        }

        public string NameJP { get; set; }
        public string NameEN { get; set; }

        public PMXColorRGB Diffuse { get; set; }
        public float Alpha;
        public PMXColorRGB Specular { get; set; }
        public float SpecularFactor;
        public PMXColorRGB Ambient { get; set; }

        public bool EdgeEnabled { get; set; }
        public PMXColorRGBA EdgeColor { get; set; }
        public float EdgeSize { get; set; }

        public bool DoubleSided { get; set; }
        public bool GroundShadow { get; set; }
        public PMXGroundShadowType GroundShadowType { get; set; }
        public bool SelfShadow { get; set; }
        public bool SelfShadowPlus { get; set; }
        public bool VertexColor { get; set; }
        
        public string DiffuseTexture { get; set; }
        public string SphereTexture { get; set; }

        public PMXSphereMode SphereMode { get; set; }
        public bool StandardToon { get; set; }

        public sbyte StandardToonIndex { get; set; }
        private sbyte _pmdToonIndex; //Import only
        public string NonStandardToonTexture { get; set; }

        public string Comment { get; set; }
        public List<PMXTriangle> Triangles { get; private set; }
        
        public PMXMaterial(PMXModel model) : base(model)
        {
            this.Triangles = new List<PMXTriangle>();

            this.Diffuse = new PMXColorRGB();
            this.Alpha = 1.0f;

            this.Specular = new PMXColorRGB();
            this.Ambient = new PMXColorRGB();
            this.EdgeColor = new PMXColorRGBA(0.0f, 0.0f, 0.0f, 1.0f);

            this.DoubleSided = false;
            this.GroundShadow = true;
            this.SelfShadow = true;
            this.SelfShadowPlus = true;
            this.EdgeEnabled = true;
            this.EdgeSize = 1.0f;
            this.VertexColor = false;

            this.DiffuseTexture = null;
            this.SphereTexture = null;
            this.NonStandardToonTexture = null;

            this.StandardToon = true;
            this.StandardToonIndex = 0;
        }

        public override void FinaliseAfterImport()
        {
            //Not required
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.LoadFromStream(br, importSettings, null, null);
        }

        private string GetTextureFromIndex(int index, string[] textures)
        {
            if(index == -1)
            {
                return null;
            }
            if (index >= textures.Length || index < 0)
            {
                throw new InvalidDataException("Unable to identify texture - index value is: " + index.ToString());
            }
            return textures[index];
        }

        public void LoadFromStream(BinaryReader br, MMDImportSettings importSettings, string[] textures, List<PMXTriangle> triangles)
        {
            if(textures == null)
            {
                textures = new string[0];
            }

            int triangleVerticesCount;

            if (importSettings.Format == MMDImportSettings.ModelFormat.PMX)
            { //PMX format
                this.NameJP = PMXParser.ReadString(br, importSettings.TextEncoding);
                this.NameEN = PMXParser.ReadString(br, importSettings.TextEncoding);

                this.Diffuse = PMXColorRGB.LoadFromStreamStatic(br);
                this.Alpha = br.ReadSingle();
                this.Specular = PMXColorRGB.LoadFromStreamStatic(br);
                this.SpecularFactor = br.ReadSingle();
                this.Ambient = PMXColorRGB.LoadFromStreamStatic(br);

                byte flags = br.ReadByte();
                //Flag parsing
                //1st bit = double sided
                this.DoubleSided = ((flags & PMXMaterial.MATERIAL_DOUBLE_SIDED) != 0);
                //2nd bit = ground shadow
                this.GroundShadow = ((flags & PMXMaterial.MATERIAL_GROUND_SHADOW) != 0);
                //3rd bit - self shadow
                this.SelfShadow = ((flags & PMXMaterial.MATERIAL_SELF_SHADOW) != 0);
                //4th bit - self shadow+
                this.SelfShadowPlus = ((flags & PMXMaterial.MATERIAL_SELF_SHADOW_PLUS) != 0);
                //5th bit - has edge line
                this.EdgeEnabled = ((flags & PMXMaterial.MATERIAL_EDGE_ENABLED) != 0);
                //6th bit - shine with vertex colour
                this.VertexColor = ((flags & PMXMaterial.MATERIAL_VERTEX_COLOR) != 0);

                //7th and 8bit - Tri, Point or Line shadow
                int shadowType = ((flags & 0xC0) >> 6);
                this.GroundShadowType = (PMXGroundShadowType)shadowType;

                this.EdgeColor = PMXColorRGBA.LoadFromStreamStatic(br);
                this.EdgeSize = br.ReadSingle();

                int diffIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.TextureIndexLength);
                this.DiffuseTexture = this.GetTextureFromIndex(diffIndex, textures);

                int sphereIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.TextureIndexLength);
                this.SphereTexture = this.GetTextureFromIndex(sphereIndex, textures);

                this.SphereMode = (PMXSphereMode)(int)(br.ReadByte());

                this.StandardToon = (br.ReadByte() == 1);
                if (this.StandardToon)
                {
                    this.StandardToonIndex = br.ReadSByte();
                }
                else
                {
                    int toonTexIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.TextureIndexLength);
                    this.NonStandardToonTexture = this.GetTextureFromIndex(toonTexIndex, textures);
                }

                this.Comment = PMXParser.ReadString(br, importSettings.TextEncoding);
                triangleVerticesCount = br.ReadInt32();
            }
            else
            { //PMD format
                this.Diffuse = PMXColorRGB.LoadFromStreamStatic(br);
                this.Alpha = br.ReadSingle();
                this.SpecularFactor = br.ReadSingle();
                this.Specular = PMXColorRGB.LoadFromStreamStatic(br);
                this.Ambient = PMXColorRGB.LoadFromStreamStatic(br);

                this._pmdToonIndex = br.ReadSByte();

                byte flags = br.ReadByte();

                this.EdgeEnabled = (flags == 1);
                this.GroundShadow = this.EdgeEnabled;

                triangleVerticesCount = br.ReadInt32();

                string textureFile = PMDParser.ReadString(br, 20, importSettings.TextEncoding);

                if(textureFile != null)
                {
                    string[] textureRefs = textureFile.Split(new char[] { '*' }, 2, StringSplitOptions.RemoveEmptyEntries);

                    this.DiffuseTexture = textureRefs[0];

                    if(textureRefs.Length > 1)
                    {
                        this.SphereTexture = textureRefs[1];
                    }
                }                
            }

            

            if(triangleVerticesCount % 3 != 0)
            {
                throw new InvalidDataException("Invalid triangle format!");
            }

            if(triangles != null)
            {
                int triangleCount = triangleVerticesCount / 3;

                if(triangleCount > triangles.Count)
                {
                    throw new InvalidDataException("Model doesn't have enough triangles!");
                }

                this.Triangles = triangles.GetRange(0, triangleCount);
                triangles.RemoveRange(0, triangleCount);
            }            
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            this.WriteToStream(bw, exportSettings, null);
        }

        /// <summary>
        /// Gets a texture index for exporting.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="textures"></param>
        /// <returns></returns>
        private int GetTextureIndex(string texture, string[] textures)
        {
            if(texture == null)
            {
                return -1;
            }

            int index = Array.IndexOf<string>(textures, texture);

            if (index >= 0)
            {
                return index;
            }
            else
            {
                throw new InvalidDataException("Texture " + texture + " is invalid");
            }
        }

        public void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings, string[] textures, string[] defaultToons = null)
        {
            if(exportSettings.Format == MMDExportSettings.ModelFormat.PMX)
            { //PMX Export
                if (textures == null)
                {
                    textures = new string[0];
                }

                PMXParser.WriteString(bw, exportSettings.TextEncoding, this.NameJP);
                PMXParser.WriteString(bw, exportSettings.TextEncoding, this.NameEN);

                this.Diffuse.WriteToStream(bw);
                bw.Write(this.Alpha);
                this.Specular.WriteToStream(bw);
                bw.Write(this.SpecularFactor);
                this.Ambient.WriteToStream(bw);

                byte flags = (byte)(((int)this.GroundShadowType) << 6);
                flags |= (byte)(this.DoubleSided ? PMXMaterial.MATERIAL_DOUBLE_SIDED : 0);
                flags |= (byte)(this.GroundShadow ? PMXMaterial.MATERIAL_GROUND_SHADOW : 0);
                flags |= (byte)(this.SelfShadow ? PMXMaterial.MATERIAL_SELF_SHADOW : 0);
                flags |= (byte)(this.SelfShadowPlus ? PMXMaterial.MATERIAL_SELF_SHADOW_PLUS : 0);
                flags |= (byte)(this.EdgeEnabled ? PMXMaterial.MATERIAL_EDGE_ENABLED : 0);
                flags |= (byte)(this.VertexColor ? PMXMaterial.MATERIAL_VERTEX_COLOR : 0);
                bw.Write(flags);

                this.EdgeColor.WriteToStream(bw);
                bw.Write(this.EdgeSize);

                PMXParser.WriteIndex(bw, exportSettings.BitSettings.TextureIndexLength, this.GetTextureIndex(this.DiffuseTexture, textures));
                PMXParser.WriteIndex(bw, exportSettings.BitSettings.TextureIndexLength, this.GetTextureIndex(this.SphereTexture, textures));

                bw.Write((byte)this.SphereMode);

                if (this.StandardToon)
                {
                    bw.Write((byte)1);
                    bw.Write(this.StandardToonIndex);
                }
                else
                {
                    bw.Write((byte)0);
                    PMXParser.WriteIndex(bw, exportSettings.BitSettings.TextureIndexLength, this.GetTextureIndex(this.NonStandardToonTexture, textures));
                }

                PMXParser.WriteString(bw, exportSettings.TextEncoding, this.Comment);

                int triangleVerticesCount = (this.Triangles.Count * 3);
                bw.Write((Int32)triangleVerticesCount);
            }
            else
            {
                if(defaultToons == null || textures == null)
                {
                    throw new InvalidDataException("Toon data required for PMD.");
                }

                //PMD format
                this.Diffuse.WriteToStream(bw);
                bw.Write(this.Alpha);
                bw.Write(this.SpecularFactor);
                this.Specular.WriteToStream(bw);
                this.Ambient.WriteToStream(bw);

                int toonIndex = -1;
                string toonFile = null;
                if (this.StandardToon && this.StandardToonIndex >= 0)
                {
                    if(this.StandardToonIndex < 10)
                    {
                        toonFile = defaultToons[this.StandardToonIndex];
                    }
                }
                else if(!this.StandardToon)
                {
                    toonFile = this.NonStandardToonTexture;                    
                }

                if(toonFile != null)
                {
                    int index = Array.IndexOf<string>(textures, toonFile);
                    if (index >= 0 && index < 10)
                    {
                        toonIndex = index;
                    }
                }

                bw.Write((sbyte)toonIndex);
                bw.Write((byte)(this.EdgeEnabled ? 1 : 0));

                bw.Write((Int32)(this.Triangles.Count * 3));

                string textureFile = "";
                if(this.DiffuseTexture != null)
                {
                    textureFile += this.DiffuseTexture;
                }
                if(this.SphereTexture != null)
                {
                    textureFile += "*" + this.DiffuseTexture;
                }

                PMDParser.WriteString(bw, 20, exportSettings.TextEncoding, textureFile);
            }            
        }


        /// <summary>
        /// Checks if the material is part of a given model.
        /// </summary>
        /// <param name="bn"></param>
        /// <param name="exportSettings"></param>
        /// <param name="nullAcceptable"></param>
        /// <returns></returns>
        public static int CheckIndexInModel(PMXMaterial mat, MMDExportSettings exportSettings, bool nullAcceptable = true)
        {
            if (mat == null)
            {
                if (nullAcceptable)
                {
                    return -1;
                }
                else
                {
                    throw new InvalidDataException("Material mustn't be null!");
                }
            }

            PMXModel model = exportSettings.Model;

            int index = model.Materials.IndexOf(mat);
            if (index < 0)
            {
                throw new InvalidDataException("Material not a member of model!");
            }
            return index;
        }

        /// <summary>
        /// Assigns toon texture for PMD files.
        /// </summary>
        /// <param name="defaultToons"></param>
        /// <param name="thisModelToons"></param>
        public void AssignToonForPMD(string[] defaultToons, string[] thisModelToons)
        {
            if(this._pmdToonIndex < 0 || this._pmdToonIndex >= 10)
            {
                this.StandardToon = true;
                this.StandardToonIndex = -1;
                return;
            }

            string toonName = thisModelToons[this._pmdToonIndex];
            this._pmdToonIndex = -1;

            string toonNameCL = toonName.ToLowerInvariant();

            int index = Array.IndexOf<string>(defaultToons, toonNameCL);
            if(index < 0)
            {
                this.StandardToon = false;
                this.NonStandardToonTexture = toonName;
            }
            else
            {
                this.StandardToon = true;
                this.StandardToonIndex = (sbyte)index;
            }
        }
    }
}
