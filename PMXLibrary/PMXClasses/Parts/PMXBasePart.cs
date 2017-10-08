using System;
using System.IO;
using System.Collections.Generic;

namespace PMXStructure.PMXClasses
{
    public abstract class PMXBasePart : MetaDataContainer
    {
        private PMXModel _model;

        /// <summary>
        /// Model this part belongs to.
        /// </summary>
        protected PMXModel Model { get
            {
                return this._model;
            }
        }

        /// <summary>
        /// Base constructor for a model part.
        /// </summary>
        /// <param name="model">Model this part belongs to.</param>
        public PMXBasePart(PMXModel model)
        {
            this._model = model;
        }

        /// <summary>
        /// Reads this part from an existing model file.
        /// </summary>
        /// <param name="br">Reader for the model file</param>
        /// <param name="importSettings">Settings for the import</param>
        public abstract void LoadFromStream(BinaryReader br, MMDImportSettings importSettings);

        /// <summary>
        /// Finalisation of the import - several info of a model is only available after other data has been imported.
        /// </summary>
        public abstract void FinaliseAfterImport();

        /// <summary>
        /// Write this part to an existing model.
        /// </summary>
        /// <param name="bw">Writer for the output.</param>
        /// <param name="exportSettings">Export settings.</param>
        public abstract void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings);        
    }
}
