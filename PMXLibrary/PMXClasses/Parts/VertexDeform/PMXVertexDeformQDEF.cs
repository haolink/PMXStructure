using System;

namespace PMXStructure.PMXClasses
{
    public class PMXVertexDeformQDEF : PMXVertexDeformBDEF4
    {
        public PMXVertexDeformQDEF(PMXModel model, PMXVertex vertex) : base(model, vertex)
        {
            this.deformIdentifier = PMXBaseDeform.DEFORM_IDENTIFY_QDEF;
        }
    }
}
