using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PMXStructure.PMXClasses.Parts;

namespace PMXStructure.PMXClasses.Parts.Morphs
{
    public abstract class PMXMorphOffsetBase : PMXBasePart
    {
        protected PMXMorph Morph { get; set; }

        public int MorphTargetType { get; protected set; }

        public PMXMorphOffsetBase(PMXModel model, PMXMorph morph) : base(model)
        {
            this.MorphTargetType = -1;
            this.Morph = morph;
        }
    }
}
