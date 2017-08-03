using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMXStructure.PMXClasses.Parts.VertexDeform
{
    class PMXVertexDeformQDEF : PMXVertexDeformBDEF4
    {
        public PMXVertexDeformQDEF(PMXModel model, PMXVertex vertex) : base(model, vertex)
        {
            this.deformIdentifier = PMXBaseDeform.DEFORM_IDENTIFY_QDEF;
        }
    }
}
