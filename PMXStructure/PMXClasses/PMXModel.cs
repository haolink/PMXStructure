using System;
using System.Collections.Generic;
using System.Text;

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
        public List<PMXDisplaySlot> DisplaySlots { get; private set; }
        public List<PMXRigidBody> RigidBodies { get; private set; }
        public List<PMXJoint> Joints { get; private set; }

        private PMXModel()
        {
            this.Vertices = new List<PMXVertex>();
            this.Materials = new List<PMXMaterial>();
            this.Bones = new List<PMXBone>();
            this.Morphs = new List<PMXMorph>();

            this.DisplaySlots = new List<PMXDisplaySlot>();
            this.DisplaySlots.Add(new PMXDisplaySlot(this) { NameEN = "Root", NameJP = "Root" });
            this.DisplaySlots.Add(new PMXDisplaySlot(this) { NameEN = "Exp", NameJP = "表情" });

            this.RigidBodies = new List<PMXRigidBody>();
            this.Joints = new List<PMXJoint>();
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
            List<PMXBasePart> allParts = new List<PMXBasePart>();

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
                allParts.Add(v);
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
                allParts.Add(t);
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
                allParts.Add(mt);
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
                allParts.Add(bn);
            }

            //Morphs
            uint morphCount = br.ReadUInt32();

            for (int i = 0; i < morphCount; i++)
            {
                PMXMorph mrph = new PMXMorph(md);
                mrph.LoadFromStream(br, settings);
                md.Morphs.Add(mrph);
                allParts.Add(mrph);
            }

            //Display frames
            md.DisplaySlots.Clear();
            uint displayCount = br.ReadUInt32();

            for (int i = 0; i < displayCount; i++)
            {
                PMXDisplaySlot ds = new PMXDisplaySlot(md);
                ds.LoadFromStream(br, settings);
                md.DisplaySlots.Add(ds);
                allParts.Add(ds);
            }

            //Rigid bodies
            uint rigidBodyCount = br.ReadUInt32();

            for (int i = 0; i < rigidBodyCount; i++)
            {
                PMXRigidBody rb = new PMXRigidBody(md);
                rb.LoadFromStream(br, settings);
                md.RigidBodies.Add(rb);
                allParts.Add(rb);
            }

            //Joints
            uint jointsCount = br.ReadUInt32();

            for (int i = 0; i < jointsCount; i++)
            {
                PMXJoint jt = new PMXJoint(md);
                jt.LoadFromStream(br, settings);
                md.Joints.Add(jt);
                allParts.Add(jt);
            }

            br.BaseStream.Close();
            
            br = null;
            fs = null;

            foreach(PMXBasePart part in allParts)
            {
                part.FinaliseAfterImport();
            }

            return md;
        }
    }    
}
