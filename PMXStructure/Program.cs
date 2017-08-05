using System;

using PMXStructure.PMXClasses;

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
                foreach (string file in inputFiles) 
                {
                    string fn = Path.GetFileNameWithoutExtension(file);
                    Console.Write(fn + " - ");
                    PMXModel md = PMXModel.LoadFromPMXFile(file);
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
            Console.ReadLine();
        }
    }
}
