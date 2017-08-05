using System;

namespace PMXStructure.PMXClasses.Parts.VertexDeform
{
    public abstract class PMXBaseDeform : PMXBasePart
    {
        protected byte deformIdentifier;

        public const byte DEFORM_IDENTIFY_BDEF1 = 0;
        public const byte DEFORM_IDENTIFY_BDEF2 = 1;
        public const byte DEFORM_IDENTIFY_BDEF4 = 2;
        public const byte DEFORM_IDENTIFY_SDEF = 3;
        public const byte DEFORM_IDENTIFY_QDEF = 4;

        public PMXVertex Vertex { get; private set; }

        public PMXBaseDeform(PMXModel model, PMXVertex vertex) : base(model)
        {
            this.deformIdentifier = 0;
            this.Vertex = vertex;
        }
    }
}
