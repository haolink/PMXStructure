﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts.VertexDeform
{
    public class PMXVertexDeformBDEF1 : PMXBaseDeform
    {
        public PMXBone Bone1 { get; set; } //Bone 1
        private int bone1Index; //Import only!

        public PMXVertexDeformBDEF1(PMXModel model, PMXVertex vertex) : base(model, vertex)
        {
            this.deformIdentifier = 0;
        }

        public override void FinaliseAfterImport()
        {
            //Not needed
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.bone1Index = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
        }
    }
}
