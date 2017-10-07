using System;
using System.IO;

namespace PMXStructure.PMXClasses
{
    public abstract class PMXBasePart
    {
        private PMXModel _model;

        protected PMXModel Model { get
            {
                return this._model;
            }
        }

        public PMXBasePart(PMXModel model)
        {
            this._model = model;
        }

        public abstract void LoadFromStream(BinaryReader br, MMDImportSettings importSettings);

        public abstract void FinaliseAfterImport();

        public abstract void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings);
    }
}
