using System;

using PMXStructure.PMXClasses;

namespace PMXStructure
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PMXModel md = PMXModel.LoadFromPMXFile(@"D:\mmd\Data\Model\Vocaloid\Miku\Appearance Miku\Appearance Miku.pmx");
                md.SaveToFile(@"D:\mmd\Data\Model\Vocaloid\Miku\Appearance Miku\Appearance Miku_resave.pmx");
                Console.WriteLine("Parsed 1");
                PMXModel.LoadFromPMXFile(@"D:\mmd\Data\Model\Selfies\Link\Link.pmx");
                Console.WriteLine("Parsed 2");
                PMXModel.LoadFromPMXFile(@"D:\mmd\Data\Model\Selfies\Luna\Luna (CHILD).pmx");
                Console.WriteLine("Parsed 3");
                PMXModel.LoadFromPMXFile(@"D:\mmd\Data\Model\GameModels\Neptunia\Nepgear\Nepggear.pmx");
                Console.WriteLine("Parsed 4");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
    }
}
