using System;

using System.Collections.Generic;
using System.IO;

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

        protected void ExportToPMDBase(List<KeyValuePair<int, float>> sortKeys, BinaryWriter bw)
        {            
            if(sortKeys.Count == 0)
            {
                bw.Write((UInt16)0);
                bw.Write((UInt16)0);
                bw.Write((byte)100);
            }
            else if(sortKeys.Count == 1)
            {
                bw.Write((UInt16)(sortKeys[0].Key));
                bw.Write((UInt16)0);
                bw.Write((byte)100);
            }
            else
            {
                sortKeys.Sort(
                    delegate (KeyValuePair<int, float> pair1,
                              KeyValuePair<int, float> pair2)
                    {
                        return pair2.Value.CompareTo(pair1.Value);
                    }
                );

                float highest = sortKeys[0].Value;
                float next = sortKeys[1].Value;
                int highestIndex = sortKeys[0].Key;
                int nextIndex = sortKeys[1].Key;

                if(next <= 0.0f)
                {
                    bw.Write((UInt16)(highestIndex));
                    bw.Write((UInt16)0);
                    bw.Write((byte)100);
                }
                else
                {
                    float sum = highest + next;
                    byte weight = (byte)Math.Round((highest / sum) * 100.0f);

                    bw.Write((UInt16)(highestIndex));
                    bw.Write((UInt16)(nextIndex));
                    bw.Write((byte)weight);
                }
            }
        }
    }
}
