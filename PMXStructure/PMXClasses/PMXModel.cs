﻿using System;
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

        public List<PMXVertex> Vertices { get; private set; }
        public List<PMXMaterial> Materials { get; private set; }
        public List<PMXBone> Bones { get; private set; }
        public List<PMXMorph> Morphs { get; private set; }
        public List<PMXDisplaySlot> DisplaySlots { get; private set; }
        public List<PMXRigidBody> RigidBodies { get; private set; }
        public List<PMXJoint> Joints { get; private set; }

        public PMXModel()
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

            if (head != "PMX")
            {
                throw new Exception("Not a PMX file!");
            }

            BinaryReader br = new BinaryReader(fs);

            float PMXVersion = br.ReadSingle();
            if (PMXVersion != 2.0f && PMXVersion != 2.1f)
            {
                throw new Exception("Unsupported PMX Version!");
            }

            byte flags = br.ReadByte();
            if (flags != 8)
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

            for (int i = 0; i < vertexCount; i++)
            {
                PMXVertex v = new PMXVertex(md);
                v.LoadFromStream(br, settings);
                md.Vertices.Add(v);
                allParts.Add(v);
            }

            //Triangles
            uint vertexRefCount = br.ReadUInt32();

            if (vertexRefCount % 3 != 0)
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

            //Materials
            uint materialCount = br.ReadUInt32();

            for (int i = 0; i < materialCount; i++)
            {
                PMXMaterial mt = new PMXMaterial(md);
                mt.LoadFromStream(br, settings, textures, importTriangles);
                md.Materials.Add(mt);
                allParts.Add(mt);
            }

            if (importTriangles.Count > 0)
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

            foreach (PMXBasePart part in allParts)
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
            if (itemCount < sbyte.MaxValue)
            {
                return 1;
            }
            if (itemCount < ushort.MaxValue - 1) //Very strange logic!
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
            if (verifyAdd == null)
            {
                return;
            }

            if (!strings.Contains(verifyAdd))
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

                if (!mat.StandardToon)
                {
                    this.AddToListIfRequired(requiredTextureList, mat.NonStandardToonTexture);
                }

                triangleCount += mat.Triangles.Count;
            }

            int maxAddUV = 0;
            foreach (PMXVertex v in this.Vertices)
            {
                maxAddUV = Math.Max(v.AddedUVs.Count, maxAddUV);
            }
            if (maxAddUV > 4)
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

            foreach (PMXMaterial mat in this.Materials)
            {
                foreach (PMXTriangle t in mat.Triangles)
                {
                    t.WriteToStream(bw, settings);
                }
            }

            //Textures
            string[] textures = requiredTextureList.ToArray();
            bw.Write((Int32)textures.Length);

            foreach (string textureFile in textures)
            {
                PMXParser.WriteString(bw, settings.TextEncoding, textureFile);
            }

            //Materials
            bw.Write((Int32)this.Materials.Count);

            foreach (PMXMaterial mat in this.Materials)
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
        /// Normalise everything.
        /// </summary>
        public void NormalizeNormalVectors()
        {
            foreach (PMXVertex v in this.Vertices)
            {
                v.NormalizeNormalVector();
            }
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

        public static PMXModel LoadFromPMDFile(string pmdFile)
        {
            FileStream fs = new FileStream(pmdFile, FileMode.Open, FileAccess.Read, FileShare.Read);

            byte[] buffer = new byte[3];
            fs.Read(buffer, 0, 3);
            string head = Encoding.ASCII.GetString(buffer);

            if (head != "Pmd")
            {
                throw new Exception("Not a PMD file!");
            }

            BinaryReader br = new BinaryReader(fs);

            float PMDVersion = br.ReadSingle();
            if (PMDVersion != 1.0f)
            {
                throw new Exception("Unsupported PMD Version!");
            }

            MMDImportSettings settings = new MMDImportSettings(MMDImportSettings.ModelFormat.PMD);
            List<PMXBasePart> allParts = new List<PMXBasePart>();

            settings.TextEncoding = Encoding.GetEncoding(932);
            settings.ExtendedUV = 0;

            PMXModel md = new PMXModel();

            md.NameJP = PMDParser.ReadString(br, 20, settings.TextEncoding);
            md.DescriptionJP = PMDParser.ReadString(br, 256, settings.TextEncoding);

            //Vertices
            uint vertexCount = br.ReadUInt32();

            for (int i = 0; i < vertexCount; i++)
            {
                PMXVertex v = new PMXVertex(md);
                v.LoadFromStream(br, settings);
                md.Vertices.Add(v);
                allParts.Add(v);
            }

            //Triangles
            uint vertexRefCount = br.ReadUInt32();

            if (vertexRefCount % 3 != 0)
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

            //Materials
            uint materialCount = br.ReadUInt32();

            for (int i = 0; i < materialCount; i++)
            {
                PMXMaterial mt = new PMXMaterial(md);
                mt.LoadFromStream(br, settings, null, importTriangles);
                md.Materials.Add(mt);

                mt.NameJP = "Material" + (i + 1).ToString(); //Initialise default names
                mt.NameEN = "Material" + (i + 1).ToString(); //Initialise default names

                allParts.Add(mt);
            }

            //Bones
            uint boneCount = (uint)br.ReadUInt16();
            
            for (int i = 0; i < boneCount; i++)
            {
                PMXBone bn = new PMXBone(md);
                bn.LoadFromStream(br, settings);
                md.Bones.Add(bn);
                allParts.Add(bn);
            }

            //PMD IKs - will be handled internally
            uint ikCount = (uint)br.ReadUInt16();
            for (int i = 0; i < ikCount; i++)
            {
                int boneId = br.ReadUInt16();
                PMXBone bn = md.Bones[boneId];
                bn.IK = new PMXIK(md, bn);
                bn.IK.LoadFromStream(br, settings);                
            }

            //Morphs
            uint mCount = (uint)br.ReadUInt16();
            for (int i = 0; i < mCount; i++)
            {
                PMXMorph mrph = new PMXMorph(md);
                mrph.LoadFromStream(br, settings);
                md.Morphs.Add(mrph);
                allParts.Add(mrph);
            }

            //Display groups - kinda insanely set up for PMD

            //Initialising root slot manually            
            md.DisplaySlots[0].References.Add(md.Bones[0]);
            allParts.Add(md.DisplaySlots[0]);

            //Exp slot is initialised differently (cause why not?)
            uint miCount = (uint)br.ReadByte();
            for (int i = 0; i < miCount; i++)
            {
                int morphId = br.ReadUInt16();
                md.DisplaySlots[1].References.Add(md.Morphs[morphId]);
            }
            allParts.Add(md.DisplaySlots[1]);

            //Display slots.. guess.. work completely different as well - first of all: Let's gather the names!
            uint nameCount = (uint)br.ReadByte();
            for (int i = 0; i < nameCount; i++)
            {
                PMXDisplaySlot ds = new PMXDisplaySlot(md);
                ds.NameJP = PMDParser.ReadString(br, 50, settings.TextEncoding);
                md.DisplaySlots.Add(ds);
                allParts.Add(ds);
            }

            //We've got the names - now let's put the bones in
            uint boneIndexCount = (uint)br.ReadUInt32();
            for (int i = 0; i < boneIndexCount; i++)
            {
                ushort bI = br.ReadUInt16();
                byte slot = br.ReadByte();

                md.DisplaySlots[slot + 1].References.Add(md.Bones[bI]);                
            }

            //Those were the display slots!

            //Does the model have English names?
            bool hasEnglishNames = false;
            if (br.BaseStream.Position < br.BaseStream.Length) //Not EOF yet
            {
                hasEnglishNames = (br.ReadByte() == 1);
            }

            if (hasEnglishNames) //Read English names
            {
                md.NameEN = PMDParser.ReadString(br, 20, settings.TextEncoding);
                md.DescriptionEN = PMDParser.ReadString(br, 256, settings.TextEncoding);
                foreach(PMXBone bn in md.Bones)
                {
                    bn.NameEN = PMDParser.ReadString(br, 20, settings.TextEncoding);
                }
                bool firstMorph = true;
                foreach (PMXMorph mrph in md.Morphs)
                {                    
                    if(firstMorph && mrph.NameJP == "base")
                    {
                        continue;
                    }
                    mrph.NameEN = PMDParser.ReadString(br, 20, settings.TextEncoding);
                    firstMorph = false;
                }
                for(int i = 2; i < md.DisplaySlots.Count; i++)
                {
                    md.DisplaySlots[i].NameEN = PMDParser.ReadString(br, 50, settings.TextEncoding);
                }
            }
            else //At least initialise them by using JP names
            {
                md.NameEN = md.NameJP;
                md.DescriptionEN = md.DescriptionJP;
                foreach (PMXBone bn in md.Bones)
                {
                    bn.NameEN = bn.NameJP;
                }
                foreach (PMXMorph mrph in md.Morphs)
                {
                    mrph.NameEN = mrph.NameJP;
                }
                for (int i = 2; i < md.DisplaySlots.Count; i++)
                {
                    md.DisplaySlots[i].NameEN = md.DisplaySlots[i].NameJP;
                }
            }

            //Are there special toon textures?
            string[] defaultToons = new string[]
            {
                "TOON01.bmp", "TOON02.bmp", "TOON03.bmp", "TOON04.bmp", "TOON05.bmp",
                "TOON06.bmp", "TOON07.bmp", "TOON08.bmp", "TOON09.bmp", "TOON10.bmp"
            };
            string[] thisModelToons = new string[10];

            if (br.BaseStream.Position < br.BaseStream.Length) //Not EOF yet
            {
                for(int i = 0; i < 10; i++)
                {
                    thisModelToons[i] = PMDParser.ReadString(br, 100, settings.TextEncoding);
                }
            }
            else
            {
                Array.Copy(defaultToons, thisModelToons, 10);
            }

            //Does the PMD file have physics?
            if (br.BaseStream.Position < br.BaseStream.Length) //Not EOF yet
            {
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
            }

            foreach (PMXBasePart part in allParts)
            {
                part.FinaliseAfterImport();
            }

            foreach (PMXMaterial mt in md.Materials)
            {
                mt.AssignToonForPMD(defaultToons, thisModelToons);
            }

            foreach (PMXBone bn in md.Bones)
            {
                bn.ParsePMDTwist();
                bn.CreateLocalCoodinateAxisForPMD();
                bn.UpdatePMDIKs();
            }

            if (md.Morphs[0].NameJP == "base")
            {
                md.Morphs.RemoveAt(0);
            }

            br = null;
            fs.Close();
            fs = null;

            return md;
        }
    }
}
