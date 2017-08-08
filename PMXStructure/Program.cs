using System;

using PMXStructure.PMXClasses;
using PMXStructure.PMXClasses.Parts;
using PMXStructure.PMXClasses.General;

using System.IO;

namespace PMXStructure
{
    class Program
    {
        static bool CompareTwoFile(string file1, string file2)
        {
            FileStream fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read, FileShare.Read);
            FileStream fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.Read);
            
            byte[] buffer1 = new byte[(int)fs1.Length];
            byte[] buffer2 = new byte[(int)fs2.Length];

            fs1.Read(buffer1, 0, (int)fs1.Length);
            fs2.Read(buffer2, 0, (int)fs2.Length);

            fs1.Close();
            fs2.Close();

            fs1 = null;
            fs2 = null;

            bool res = true;
            bool longer = false;
            string resString = "Both files identical.";

            for(int i = 0; i < Math.Max(buffer1.Length, buffer2.Length);i++)
            {
                if(i < buffer1.Length && i < buffer2.Length)
                {
                    if(buffer1[i] != buffer2[i])
                    {
                        res = false;
                        resString = "Files differ!";
                        break;
                    }
                }
                else if(i >= buffer1.Length)
                {
                    if(buffer2[i] == 0)
                    {
                        if(!longer)
                        {
                            resString = "File 2 has some trailing zeros (this shouldn't cause issues).";
                        }
                    }
                    else
                    {
                        resString = "File 1 is missing data.";
                        res = false;
                        break;
                    }
                }
                else
                {
                    if (buffer1[i] == 0)
                    {
                        if (!longer)
                        {
                            resString = "File 1 has some trailing zeros (this shouldn't cause issues).";
                        }
                    }
                    else
                    {
                        resString = "File 2 is missing data.";
                        res = false;
                        break;
                    }
                }
            }

            Console.WriteLine(resString);
            return res;
        }

        static void Main(string[] args)
        {
            PMXModel md;

            string tmpFile = @"D:\mmd\temp.pmx";
            string[] inputFiles = new string[]
            {
                @"D:\mmd\Data\Model\Vocaloid\Miku\Appearance Miku\Appearance Miku.pmx",
                @"D:\mmd\Data\Model\Selfies\Link\Link.pmx",
                @"D:\mmd\Data\Model\Selfies\Luna\Luna (CHILD).pmx",
                @"D:\mmd\Data\Model\GameModels\Neptunia\Nepgear\Nepggear.pmx",
                @"D:\mmd\Data\Model\Vocaloid\Miku\on_Hexa_v102\onda_mod_Hexa_v102Nぽにて.pmx",
                @"D:\mmd\Data\Model\GameModels\Zelda\zelda\zelda.pmx",
                @"D:\mmd\Data\Model\Anime\No Game No Life\Shiro\Shiro.Pmx",
                @"D:\mmd\Data\Model\Anime\No Game No Life\Shiro\Shiros phone.Pmx",
                @"D:\mmd\Data\Model\Vocaloid\Luka\TDA Cheerleader Luka\TDA Cheerleader Luka.pmx", //Bad test case
                @"D:\mmd\Data\Model\Vocaloid\Luka\Luka_v3.3\Luka.pmx",
                @"D:\mmd\Data\Model\GameModels\Splatoon\きの式イカ\inkling.pmx"
            };

            /*try
            {*/
            /*    foreach (string file in inputFiles) 
                {
                    string fn = Path.GetFileNameWithoutExtension(file);
                    Console.Write(fn + " - ");
                    md = PMXModel.LoadFromPMXFile(file);
                    Console.Write(" written - ");
                    md.SaveToFile(tmpFile);

                    if(!CompareTwoFile(file, tmpFile))
                    {                        
                        break;
                    }

                    Console.WriteLine("");
                }
            /*}
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }*/

            //WriteTestModel();

            md = PMXModel.LoadFromPMDFile(@"D:\mmd\MikuMikuDance\UserFile\Model\初音ミクVer2.pmd");

            md.SaveToPMDFile(@"D:\mmd\MikuMikuDance\UserFile\Model\MikuV2.pmd");
            md.SaveToFile(@"D:\mmd\MikuMikuDance\UserFile\Model\MikuV2.pmx");

            //Console.WriteLine(md.Materials.Count);

            Console.ReadLine();
        }

        private static void WriteTestModel()
        {
            PMXModel md = new PMXModel();
            md.NameEN = "Cube";
            md.NameJP = "Cube";
            md.DescriptionEN = "Cube Desc";
            md.DescriptionJP = "Cube Desc";

            PMXBone bn = new PMXBone(md);
            bn.NameJP = "Root";
            bn.NameEN = "Root";
            bn.Position = new PMXVector3(0.0f, 0.0f, 0.0f);
            bn.HasChildBone = false;
            bn.ChildVector = new PMXVector3(0.0f, 15.0f, 0.0f);
            bn.Translatable = true;

            md.Bones.Add(bn);

            PMXVector3[,] vertices = new PMXVector3[8, 3]
            {
                { new PMXVector3(-5.0f,  0.0f, -5.0f), new PMXVector3(-1.0f, -1.0f, -1.0f), new PMXVector3(0.0f, 1.0f, 0.0f) },
                { new PMXVector3(-5.0f,  10.0f, -5.0f), new PMXVector3(-1.0f,  1.0f, -1.0f), new PMXVector3(0.0f, 0.0f, 0.0f) },
                { new PMXVector3( 5.0f,  0.0f, -5.0f), new PMXVector3( 1.0f, -1.0f, -1.0f), new PMXVector3(1.0f, 1.0f, 0.0f) },
                { new PMXVector3( 5.0f,  10.0f, -5.0f), new PMXVector3( 1.0f,  1.0f, -1.0f), new PMXVector3(1.0f, 0.0f, 0.0f) },
                { new PMXVector3(-5.0f,  0.0f,  5.0f), new PMXVector3(-1.0f, -1.0f,  1.0f), new PMXVector3(0.0f, 0.0f, 0.0f) },
                { new PMXVector3(-5.0f,  10.0f,  5.0f), new PMXVector3(-1.0f,  1.0f,  1.0f), new PMXVector3(0.0f, 1.0f, 0.0f) },
                { new PMXVector3( 5.0f,  0.0f,  5.0f), new PMXVector3( 1.0f, -1.0f,  1.0f), new PMXVector3(1.0f, 0.0f, 0.0f) },
                { new PMXVector3( 5.0f,  10.0f,  5.0f), new PMXVector3( 1.0f,  1.0f,  1.0f), new PMXVector3(1.0f, 1.0f, 0.0f) },
            };

            for (int i = 0; i < 8; i++)
            {
                PMXVertex vtx = new PMXVertex(md);
                vtx.Position = vertices[i, 0];
                vtx.Normals = vertices[i, 1];
                vtx.UV = new PMXVector2(vertices[i, 2].X, vertices[i, 2].Y);

                md.Vertices.Add(vtx);
            }

            PMXMaterial mat = new PMXMaterial(md);
            mat.NameJP = "Base";
            mat.NameEN = "Base";
            mat.DiffuseTexture = "arrow.png";
            mat.Diffuse = new PMXColorRGB(1.0f, 1.0f, 1.0f);
            mat.Specular = new PMXColorRGB(0.0f, 0.0f, 0.0f);
            mat.Ambient = new PMXColorRGB(0.7f, 0.7f, 0.7f);

            int[,] triangleBases = new int[12, 3]
            {
                { 0, 1, 2 }, { 1, 3, 2 },
                { 6, 4, 0 }, { 0, 2, 6 },
                { 2, 3, 6 }, { 3, 7, 6 },
                { 4, 5, 0 }, { 5, 1, 0 },
                { 1, 5, 3 }, { 5, 7, 3 },
                { 4, 6, 5 }, { 6, 7, 5 }
            };

            for (int i = 0; i < 12; i++)
            {
                PMXTriangle mxt = new PMXTriangle(md, md.Vertices[triangleBases[i, 0]], md.Vertices[triangleBases[i, 1]], md.Vertices[triangleBases[i, 2]]);
                mat.Triangles.Add(mxt);
            }
            md.Materials.Add(mat);

            md.DisplaySlots[0].References.Add(bn);

            md.NormalizeNormalVectors();
            md.SaveToFile(@"D:\mmd\cube.pmx");
        }
    }
}
