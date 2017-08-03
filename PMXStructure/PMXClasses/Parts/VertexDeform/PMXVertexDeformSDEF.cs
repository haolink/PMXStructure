using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using PMXStructure.PMXClasses.General;
using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts.VertexDeform
{
    class PMXVertexDeformSDEF : PMXVertexDeformBDEF2
    {
        public PMXVector3 C;
        public PMXVector3 R0;
        public PMXVector3 R1;

        public PMXVertexDeformSDEF(PMXModel model, PMXVertex vertex) : base(model, vertex)
        {
            this.deformIdentifier = PMXBaseDeform.DEFORM_IDENTIFY_SDEF;
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            base.LoadFromStream(br, importSettings);

            this.C = PMXVector3.LoadFromStreamStatic(br);
            this.R0 = PMXVector3.LoadFromStreamStatic(br);
            this.R1 = PMXVector3.LoadFromStreamStatic(br);
        }
    }
}
