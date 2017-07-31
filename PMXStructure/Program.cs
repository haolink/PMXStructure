using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PMXStructure.PMXClasses;

namespace PMXStructure
{
    class Program
    {
        static void Main(string[] args)
        {
            PMXModel md = PMXModel.LoadFromPMXFile(@"D:\mmd\Data\Model\Vocaloid\Miku\Appearance Miku\Appearance Miku.pmx");

            foreach(string tex in md.Textures)
            {
                Console.WriteLine(tex);
            }
            Console.ReadLine();
        }
    }
}
