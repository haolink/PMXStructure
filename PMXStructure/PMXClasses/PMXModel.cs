﻿using System;
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
        public string NameJP { get; set; }
        public string NameEN { get; set; }
        public string DescriptionJP { get; set; }
        public string DescriptionEN { get; set; }

        public List<PMXVertex> Vertices { get; private set;  }
        public List<PMXMaterial> Materials { get; private set; }
        public List<PMXBone> Bones { get; private set; }
        public List<PMXMorph> Morphs { get; private set; }

        private PMXModel()
        {
            this.Vertices = new List<PMXVertex>();
            this.Materials = new List<PMXMaterial>();
            this.Bones = new List<PMXBone>();
            this.Morphs = new List<PMXMorph>();
        }

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
            List<PMXTriangle> importTriangles = new List<PMXTriangle>();

            for (int i = 0; i < triangleCount; i++)
            {
                PMXTriangle t = new PMXTriangle(md);
                t.LoadFromStream(br, settings);
                importTriangles.Add(t);
            }

            //Textures
            uint textureCount = br.ReadUInt32();

            List<string> importTextures = new List<string>();        

            for (int i = 0; i < textureCount; i++)
            {
                string tex = PMXParser.ReadString(br, settings.TextEncoding);
                importTextures.Add(tex);
            }
            string[] textures = importTextures.ToArray();

            //Textures
            uint materialCount = br.ReadUInt32();

            for (int i = 0; i < materialCount; i++)
            {
                PMXMaterial mt = new PMXMaterial(md);
                mt.LoadFromStream(br, settings, textures, importTriangles);
                md.Materials.Add(mt);
            }

            if(importTriangles.Count > 0)
            {
                throw new InvalidDataException("Model materials don't cover all triangles!");
            }

            //Bones
            uint boneCount = br.ReadUInt32();

            for (int i = 0; i < boneCount; i++)
            {
                PMXBone bn = new PMXBone(md);
                bn.LoadFromStream(br, settings);
                md.Bones.Add(bn);
            }

            //Morphs
            uint morphCount = br.ReadUInt32();

            for (int i = 0; i < morphCount; i++)
            {
                PMXMorph mrph = new PMXMorph(md);
                mrph.LoadFromStream(br, settings);
                md.Morphs.Add(mrph);
            }


            br.BaseStream.Close();
            
            br = null;
            fs = null;

            return md;
        }
    }    
}
