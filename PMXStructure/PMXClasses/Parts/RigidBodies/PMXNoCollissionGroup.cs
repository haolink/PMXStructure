using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;
using System.Collections;

namespace PMXStructure.PMXClasses.Parts.RigidBodies
{
    public class PMXNoCollissionGroup : PMXBasePart, ICollection, IEnumerable
    {
        public PMXRigidBody RigidBody { get; private set; }

        public int Count => 16;

        public bool IsReadOnly => false;

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        private BitArray _noCollissionGroup;


        public PMXNoCollissionGroup(PMXModel model, PMXRigidBody body) : base(model)
        {
            this.RigidBody = body;
            this._noCollissionGroup = new BitArray(16, false);
        }

        public bool this[int index]
        {
            get
            {
                if(index >= 16 || index < 0)
                {
                    throw new InvalidDataException("Only supports up to 16 collission groups");
                }
                return this._noCollissionGroup[index];
            }
            set
            {
                if (index >= 16 || index < 0)
                {
                    throw new InvalidDataException("Only supports up to 16 collission groups");
                }

                this._noCollissionGroup[index] = value;
            }
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            byte[] buffer = new byte[2];           
            br.BaseStream.Read(buffer, 0, 2);

            for(int i = 0; i < 2; i++)
            {
                buffer[i] = (byte)(~((int)buffer[i]) & 0xFF);
            }

            this._noCollissionGroup = new BitArray(buffer);
        }

        public override void FinaliseAfterImport()
        {
            //Not required
        }


        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            byte[] buffer = new byte[2];

            this._noCollissionGroup.CopyTo(buffer, 0);

            for (int i = 0; i < 2; i++)
            {
                buffer[i] = (byte)(~((int)buffer[i]) & 0xFF);
            }

            bw.BaseStream.Write(buffer, 0, 2);
        }

        public void Add(bool item)
        {
            throw new InvalidOperationException("Fixed size");
        }

        public void Clear()
        {
            throw new InvalidOperationException("Fixed size");
        }

        public bool Contains(bool item)
        {
            foreach(bool b in this._noCollissionGroup)
            {
                if(b == item)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(bool[] array, int arrayIndex)
        {
            throw new InvalidOperationException("Fixed size");
        }

        public bool Remove(bool item)
        {
            throw new InvalidOperationException("Fixed size");
        }

        public IEnumerator GetEnumerator()
        {
            return this._noCollissionGroup.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new InvalidOperationException("Fixed size");
        }

    }
}
