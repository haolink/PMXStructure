﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.General;

namespace PMXStructure.PMXClasses.Parts.Morphs
{
    class PMXMorphOffsetBone : PMXMorphOffsetBase
    {
        public PMXBone Bone { get; set; }
        public PMXVector3 Translation { get; set; }
        public PMXQuaternion Rotation { get; set; }        

        public PMXMorphOffsetBone(PMXModel model, PMXMorph morph) : base(model, morph)
        {
            this.MorphTargetType = PMXMorph.MORPH_IDENTIFY_BONE;
            this.Translation = new PMXVector3();
            this.Rotation = new PMXQuaternion();
        }

        public override void FinaliseAfterImport()
        {
            throw new NotImplementedException();
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            int boneIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);

            this.Bone = this.Model.Bones[boneIndex];
            this.Translation = PMXVector3.LoadFromStreamStatic(br);
            this.Rotation = PMXQuaternion.LoadFromStreamStatic(br);
        }
    }
}
