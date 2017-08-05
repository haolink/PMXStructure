using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.Parts;

using PMXStructure.PMXClasses.Parts.VertexDeform;

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

        /// <summary>
        /// Determines the required BitLength for saving a PMX file.
        /// </summary>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        private byte DetermineRequiredBitLength(int itemCount)
        {
            if(itemCount < sbyte.MaxValue)
            {
                return 1;
            }
            if(itemCount < ushort.MaxValue - 1) //Very strange logic!
            {
                return 2;
            }
            return 4;
        }

        /// <summary>
        /// Only adds a string to a List if it's not null and doesn't already exist.
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="verifyAdd"></param>
        private void AddToListIfRequired(List<string> strings, string verifyAdd)
        {
            if(verifyAdd == null)
            {
                return;
            }

            if(!strings.Contains(verifyAdd))
            {
                strings.Add(verifyAdd);
            }
        }

        /// <summary>
        /// Saves a model to a stream.
        /// </summary>
        /// <param name="stream"></param>
        public void SaveToStream(Stream stream)
        {            
            List<string> requiredTextureList = new List<string>(); //List of textures to export
            int triangleCount = 0;
            foreach (PMXMaterial mat in this.Materials)
            {
                this.AddToListIfRequired(requiredTextureList, mat.DiffuseTexture);
                this.AddToListIfRequired(requiredTextureList, mat.SphereTexture);

                if(!mat.StandardToon)
                {
                    this.AddToListIfRequired(requiredTextureList, mat.NonStandardToonTexture);
                }

                triangleCount += mat.Triangles.Count;
            }

            int maxAddUV = 0;
            foreach(PMXVertex v in this.Vertices)
            {
                maxAddUV = Math.Max(v.AddedUVs.Count, maxAddUV);
            }
            if(maxAddUV > 4)
            {
                throw new InvalidDataException("Maximum Add UV data is 4");
            }

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            byte[] magic = Encoding.ASCII.GetBytes("PMX ");
            stream.Write(magic, 0, 4);

            Random rnd = new Random();
            int randomNumber = rnd.Next();

            PMXExportSettings settings = new PMXExportSettings();
            settings.Model = this;
            settings.ExportHash = randomNumber;
            settings.ExtendedUV = (byte)maxAddUV;
            float version = 2.0f;

            foreach (PMXJoint j in this.Joints)
            {
                if (j.Type != PMXJoint.JointType.SpringSixDOF)
                {
                    version = 2.1f;
                    break;
                }
            }

            bw.Write(version);

            //Flag length
            bw.Write((byte)8);

            //Encoding (UTF-16 LE only)
            settings.TextEncoding = Encoding.Unicode;
            bw.Write((byte)0);
            bw.Write((byte)settings.ExtendedUV);

            settings.BitSettings.VertexIndexLength = this.DetermineRequiredBitLength(this.Vertices.Count);
            settings.BitSettings.TextureIndexLength = this.DetermineRequiredBitLength(requiredTextureList.Count);
            settings.BitSettings.MaterialIndexLength = this.DetermineRequiredBitLength(this.Materials.Count);
            settings.BitSettings.BoneIndexLength = this.DetermineRequiredBitLength(this.Bones.Count);
            settings.BitSettings.MorphIndexLength = this.DetermineRequiredBitLength(this.Morphs.Count);
            settings.BitSettings.RigidBodyIndexLength = this.DetermineRequiredBitLength(this.RigidBodies.Count);

            bw.Write(settings.BitSettings.VertexIndexLength);
            bw.Write(settings.BitSettings.TextureIndexLength);
            bw.Write(settings.BitSettings.MaterialIndexLength);
            bw.Write(settings.BitSettings.BoneIndexLength);
            bw.Write(settings.BitSettings.MorphIndexLength);
            bw.Write(settings.BitSettings.RigidBodyIndexLength);

            PMXParser.WriteString(bw, settings.TextEncoding, this.NameJP);
            PMXParser.WriteString(bw, settings.TextEncoding, this.NameEN);
            PMXParser.WriteString(bw, settings.TextEncoding, this.DescriptionJP);
            PMXParser.WriteString(bw, settings.TextEncoding, this.DescriptionEN);

            //Vertices
            bw.Write((Int32)this.Vertices.Count);
            int vtxIndex = 0;

            foreach (PMXVertex v in this.Vertices)
            {
                v.AddIndexForExport(settings, vtxIndex);
                v.WriteToStream(bw, settings);
                vtxIndex++;
            }

            //Triangles
            bw.Write((Int32)(triangleCount * 3));

            foreach(PMXMaterial mat in this.Materials)
            {
                foreach(PMXTriangle t in mat.Triangles)
                {
                    t.WriteToStream(bw, settings);
                }
            }

            //Textures
            string[] textures = requiredTextureList.ToArray();
            bw.Write((Int32)textures.Length);

            foreach(string textureFile in textures)
            {
                PMXParser.WriteString(bw, settings.TextEncoding, textureFile);
            }

            //Materials
            bw.Write((Int32)this.Materials.Count);

            foreach(PMXMaterial mat in this.Materials)
            {
                mat.WriteToStream(bw, settings, textures);
            }

            //Bones
            bw.Write((Int32)this.Bones.Count);

            foreach (PMXBone bn in this.Bones)
            {
                bn.WriteToStream(bw, settings);
            }

            //Morphs
            bw.Write((Int32)this.Morphs.Count);

            foreach (PMXMorph mrph in this.Morphs)
            {
                mrph.WriteToStream(bw, settings);
            }

            //Displays
            bw.Write((Int32)this.DisplaySlots.Count);

            foreach (PMXDisplaySlot dsp in this.DisplaySlots)
            {
                dsp.WriteToStream(bw, settings);
            }

            //Rigid bodies
            bw.Write((Int32)this.RigidBodies.Count);

            foreach (PMXRigidBody bdy in this.RigidBodies)
            {
                bdy.WriteToStream(bw, settings);
            }

            //Joints
            bw.Write((Int32)this.Joints.Count);
            foreach (PMXJoint jt in this.Joints)
            {
                jt.WriteToStream(bw, settings);
            }

            //Reset triangles
            foreach (PMXVertex v in this.Vertices)
            {
                v.RemoveIndexForExport(settings);                
            }


            long length = ms.Position;
            ms.Seek(0, SeekOrigin.Begin);

            ms.CopyTo(stream);

            ms.Close();
            ms = null;
        }

        /// <summary>
        /// Saves a model to a file name. (PMX export only)
        /// </summary>
        /// <param name="filename">PMX file name.</param>
        public void SaveToFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);

            this.SaveToStream(fs);

            fs.Close();
            fs = null;
        }
    }    
}
