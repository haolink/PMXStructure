using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PMXStructure.PMXClasses.Parts;

namespace PMXStructure.PMXClasses.Parts.VertexDeform
{
    public abstract class PMXBaseDeform : PMXBasePart
    {
        protected byte deformIdentifier;

        public PMXVertex Vertex { get; private set; }

        public PMXBaseDeform(PMXModel model, PMXVertex vertex) : base(model)
        {
            this.deformIdentifier = 0;
            this.Vertex = vertex;
        }
    }
}
