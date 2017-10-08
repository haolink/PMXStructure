using System;
using System.Collections.Generic;
using System.Text;

namespace PMXStructure.PMXClasses
{
    public class MetaDataContainer
    {
        private Dictionary<string, object> _metaData = null;

        /// <summary>
        /// Checks if a metadata key is available.
        /// </summary>
        /// <param name="key">Key of the meta data</param>
        /// <returns>Whether value exists or not</returns>
        public bool HasMetadata(string key)
        {
            if (this._metaData == null)
            {
                return false;
            }

            return this._metaData.ContainsKey(key);
        }

        /// <summary>
        /// Sets metadata value.
        /// </summary>
        /// <param name="key">Key of the meta data</param>
        /// <param name="value">Value</param>
        public void SetMetadata(string key, object value)
        {
            if (this._metaData == null)
            {
                this._metaData = new Dictionary<string, object>();
            }

            if (this._metaData.ContainsKey(key))
            {
                this._metaData[key] = value;
            }
            else
            {
                this._metaData.Add(key, value);
            }
        }

        /// <summary>
        /// Queries meta data with a given key.
        /// </summary>
        /// <param name="key">Key to query</param>
        /// <returns>Value of the given key</returns>
        public object GetMetadata(string key)
        {
            if (this._metaData == null)
            {
                throw new ArgumentException("Key not found");
            }

            if (!this._metaData.ContainsKey(key))
            {
                throw new ArgumentException("Key not found");
            }
            else
            {
                return this._metaData[key];
            }
        }

        /// <summary>
        /// Removes a metadata value with a given key.
        /// </summary>
        /// <param name="key">Key to remove</param>
        public void RemoveMetadata(string key)
        {
            if (this._metaData == null)
            {
                throw new ArgumentException("Key not found");
            }

            if (!this._metaData.ContainsKey(key))
            {
                throw new ArgumentException("Key not found");
            }
            else
            {
                this._metaData.Remove(key);
            }
        }
    }
}
