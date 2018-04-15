using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMXStructure.PMXClasses
{
    public class InheritableType<T>
    {
        private Type _storedType;

        public Type StoredType { get { return _storedType; } }

        public InheritableType(Type typeToStore)
        {
            if(typeToStore.IsAssignableFrom(typeof(T))) 
            {
                this._storedType = typeToStore;
            }
            else
            {
                throw new ArgumentException("Requested type does not inherit from wished base type.");
            }
        }
    }
}
