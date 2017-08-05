using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;

namespace PMXStructure.PMXClasses.Parts
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
    }
}
