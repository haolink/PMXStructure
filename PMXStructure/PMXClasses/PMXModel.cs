using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.Parts;
using System.IO;

namespace PMXStructure.PMXClasses
{
    public class PMXModel
    {
        private PMXModel()
        {
            this.Vertices = new List<PMXVertex>();
            this.Triangles = new List<PMXTriangle>();
            this.Textures = new List<string>();
        }

        public string NameJP { get; set; }
        public string NameEN { get; set; }
        public string DescriptionJP { get; set; }
        public string DescriptionEN { get; set; }

        public List<PMXVertex> Vertices { get; }
        public List<PMXTriangle> Triangles { get; }
        public List<string> Textures { get; }

        public static PMXModel LoadFromPMXFile(string pmxFile)
        {
            FileStream fs = new FileStream(pmxFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            
            byte[] buffer = new byte[3];
            fs.Read(buffer, 0, 3);
            string head = Encoding.ASCII.GetString(buffer);
            fs.Seek(1, SeekOrigin.Current);

            if(head != "PMX")
            {
                throw new Exception("Not a PMX file!");
            }

            BinaryReader br = new BinaryReader(fs);

            float PMXVersion = br.ReadSingle();
            if(PMXVersion != 2.0f && PMXVersion != 2.1f)
            {
                throw new Exception("Unsupported PMX Version!");
            }

            byte flags = br.ReadByte();
            if(flags != 8)
            {
                throw new Exception("Invalid PMX bytes version!");
            }

            MMDImportSettings settings = new MMDImportSettings(MMDImportSettings.ModelFormat.PMX);

            byte text_encoding = br.ReadByte();
            settings.TextEncoding = 
                (text_encoding == 1 ? Encoding.UTF8 :
                (text_encoding == 0 ? Encoding.Unicode : Encoding.GetEncoding(932)));

            settings.ExtendedUV = br.ReadByte();

            PMXModel md = new PMXModel();

            settings.BitSettings.VertexIndexLength = br.ReadByte();
            settings.BitSettings.TextureIndexLength = br.ReadByte();
            settings.BitSettings.MaterialIndexLength = br.ReadByte();
            settings.BitSettings.BoneIndexLength = br.ReadByte();
            settings.BitSettings.MorphIndexLength = br.ReadByte();
            settings.BitSettings.RigidBodyIndexLength = br.ReadByte();

            md.NameJP = PMXParser.ReadString(br, settings.TextEncoding);
            md.NameEN = PMXParser.ReadString(br, settings.TextEncoding);
            md.DescriptionJP = PMXParser.ReadString(br, settings.TextEncoding);
            md.DescriptionEN = PMXParser.ReadString(br, settings.TextEncoding);


            //Vertices
            uint vertexCount = br.ReadUInt32();

            for(int i = 0; i < vertexCount; i++)
            {
                PMXVertex v = new PMXVertex(md);
                v.LoadFromStream(br, settings);
                md.Vertices.Add(v);
            }

            //Triangles
            uint vertexRefCount = br.ReadUInt32();

            if(vertexRefCount % 3 != 0)
            {
                throw new Exception("Invalid triangle count!");
            }

            uint triangleCount = vertexRefCount / 3;

            for (int i = 0; i < triangleCount; i++)
            {
                PMXTriangle t = new PMXTriangle(md);
                t.LoadFromStream(br, settings);
                md.Triangles.Add(t);
            }

            //Textures
            uint textureCount = br.ReadUInt32();
            for (int i = 0; i < textureCount; i++)
            {
                string tex = PMXParser.ReadString(br, settings.TextEncoding);
                md.Textures.Add(tex);
            }


            return md;
        }
    }    
}
