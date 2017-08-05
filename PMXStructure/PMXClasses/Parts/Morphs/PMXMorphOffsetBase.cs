using System;

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
