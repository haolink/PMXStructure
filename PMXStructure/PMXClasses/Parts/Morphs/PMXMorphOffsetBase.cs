using System;

namespace PMXStructure.PMXClasses.Parts.Morphs
{
    public abstract class PMXMorphOffsetBase : PMXBasePart
    {
        protected PMXMorph Morph { get; set; }

        public byte MorphTargetType { get; protected set; }

        public PMXMorphOffsetBase(PMXModel model, PMXMorph morph) : base(model)
        {
            this.MorphTargetType = 0;
            this.Morph = morph;
        }
    }
}
