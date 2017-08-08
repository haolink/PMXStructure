using System;
using System.IO;

using PMXStructure.PMXClasses.General;
using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts.VertexDeform
{
    class PMXVertexDeformSDEF : PMXVertexDeformBDEF2
    {
        public PMXVector3 C;
        public PMXVector3 R0;
        public PMXVector3 R1;

        public PMXVertexDeformSDEF(PMXModel model, PMXVertex vertex) : base(model, vertex)
        {
            this.deformIdentifier = PMXBaseDeform.DEFORM_IDENTIFY_SDEF;
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            base.LoadFromStream(br, importSettings);

            this.C = PMXVector3.LoadFromStreamStatic(br);
            this.R0 = PMXVector3.LoadFromStreamStatic(br);
            this.R1 = PMXVector3.LoadFromStreamStatic(br);
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            base.WriteToStream(bw, exportSettings);

            if(exportSettings.Format == MMDExportSettings.ModelFormat.PMX)
            {
                this.C.WriteToStream(bw);
                this.R0.WriteToStream(bw);
                this.R1.WriteToStream(bw);
            }            
        }
    }
}
