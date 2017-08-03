using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.General;

namespace PMXStructure.PMXClasses.Parts
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

        public byte StandardToonIndex { get; set; }
        public string NonStandardToonTexture { get; set; }

        public string Comment { get; set; }
        public List<PMXTriangle> Triangles { get; private set; }
        
        public PMXMaterial(PMXModel model) : base(model)
        {
            this.Triangles = new List<PMXTriangle>();

            this.Diffuse = new PMXColorRGB();
            this.Specular = new PMXColorRGB();
            this.Ambient = new PMXColorRGB();
            this.EdgeColor = new PMXColorRGBA();
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

            this.NameJP = PMXParser.ReadString(br, importSettings.TextEncoding);
            this.NameEN = PMXParser.ReadString(br, importSettings.TextEncoding);

            this.Diffuse = PMXColorRGB.LoadFromStreamStatic(br);
            this.Alpha = br.ReadSingle();
            this.Specular = PMXColorRGB.LoadFromStreamStatic(br);
            this.SpecularFactor = br.ReadSingle();
            this.Ambient = PMXColorRGB.LoadFromStreamStatic(br);

            byte flags = br.ReadByte();

            this.EdgeColor = PMXColorRGBA.LoadFromStreamStatic(br);
            this.EdgeSize = br.ReadSingle();

            int diffIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.TextureIndexLength);
            this.DiffuseTexture = this.GetTextureFromIndex(diffIndex, textures);            

            int sphereIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.TextureIndexLength);
            this.SphereTexture = this.GetTextureFromIndex(diffIndex, textures);

            this.SphereMode = (PMXSphereMode)(int)(br.ReadByte());

            this.StandardToon = (br.ReadByte() == 1);
            if(this.StandardToon)
            {
                this.StandardToonIndex = br.ReadByte();
            }
            else
            {
                int toonTexIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.TextureIndexLength);
                this.NonStandardToonTexture = this.GetTextureFromIndex(toonTexIndex, textures);
            }

            this.Comment = PMXParser.ReadString(br, importSettings.TextEncoding);
            int triangleVerticesCount = br.ReadInt32();

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
        }
    }
}
