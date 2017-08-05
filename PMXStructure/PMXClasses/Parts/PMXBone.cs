using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.General;

namespace PMXStructure.PMXClasses.Parts
{
    public class PMXBone : PMXBasePart
    {
        public enum BoneExternalModificationType
        {
            None = 0,
            Rotation = 1,
            Translation = 2,
            Both = 3
        }

        public const ushort BONE_TAILPOS_IS_BONE = 0x0001;
        public const ushort BONE_CAN_ROTATE = 0x0002;
        public const ushort BONE_CAN_TRANSLATE = 0x0004;
        public const ushort BONE_IS_VISIBLE = 0x0008;
        public const ushort BONE_CAN_MANIPULATE = 0x0010;
        public const ushort BONE_IS_IK = 0x0020;
        public const ushort BONE_IS_EXTERNAL_ROTATION = 0x0100;
        public const ushort BONE_IS_EXTERNAL_TRANSLATION = 0x0200;
        public const ushort BONE_HAS_FIXED_AXIS = 0x0400;
        public const ushort BONE_HAS_LOCAL_COORDINATE = 0x0800;
        public const ushort BONE_IS_AFTER_PHYSICS_DEFORM = 0x1000;
        public const ushort BONE_IS_EXTERNAL_PARENT_DEFORM = 0x2000;

        public string NameJP { get; set; }
        public string NameEN { get; set; }

        public PMXVector3 Position { get; set; }

        private int parentIndex; //Import only
        public PMXBone Parent { get; set; }

        public int Layer { get; set; }

        public bool HasChildBone { get; set; }
        private int childBoneIndex; //Import only
        public PMXBone ChildBone { get; set; }
        public PMXVector3 ChildVector { get; set; }

        public bool Rotatable { get; set; }
        public bool Translatable { get; set; }
        public bool Visible { get; set; }
        public bool Operating { get; set; }

        public BoneExternalModificationType ExternalModificationType { get; set; }
        private int externalBoneIndex; //Import only
        public float ExternalBoneEffect { get; set; }
        public PMXBone ExternalBone { get; set; }

        public bool FixedAxis { get; set; }
        public PMXVector3 AxisLimit { get; set; }

        public bool LocalCoordinates { get; set; }
        public PMXVector3 LocalCoordinatesX { get; set; }
        public PMXVector3 LocalCoordinatesZ { get; set; }

        public bool HasExternalParent { get; set; }
        public int ExternalParentKey { get; set; }

        public bool TransformPhysicsFirst { get; set; }

        public PMXIK IK { get; set; }

        public PMXBone(PMXModel model) : base(model)
        {
            this.Rotatable = true;
            this.Translatable = false;
            this.HasChildBone = true;
            this.Operating = true;
            this.Visible = true;

            this.Position = new PMXVector3();
            this.ChildVector = new PMXVector3();
            this.AxisLimit = new PMXVector3();
            this.LocalCoordinatesX = new PMXVector3();
            this.LocalCoordinatesZ = new PMXVector3();
        }

        public override void FinaliseAfterImport()
        {
            if(this.parentIndex == -1)
            {
                this.Parent = null;
            } else
            {
                this.Parent = this.Model.Bones[this.parentIndex];
            }

            if(this.HasChildBone)
            {
                if (this.childBoneIndex == -1)
                {
                    this.ChildBone = null;
                }
                else
                {
                    this.ChildBone = this.Model.Bones[this.childBoneIndex];
                }
            }            

            if (this.ExternalModificationType != BoneExternalModificationType.None)
            {
                if(this.externalBoneIndex == -1)
                {
                    this.ExternalBone = null;
                }
                else
                {
                    this.ExternalBone = this.Model.Bones[this.externalBoneIndex];
                }                
            }
            else
            {
                this.ExternalBone = null;
            }
            
            if(this.IK != null)
            {
                this.IK.FinaliseAfterImport();
            }
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.NameJP = PMXParser.ReadString(br, importSettings.TextEncoding);
            this.NameEN = PMXParser.ReadString(br, importSettings.TextEncoding);

            this.Position = PMXVector3.LoadFromStreamStatic(br);

            this.parentIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);

            this.Layer = br.ReadInt32();

            short flags = br.ReadInt16();

            this.HasChildBone = ((flags & PMXBone.BONE_TAILPOS_IS_BONE) != 0);
            if(this.HasChildBone)
            {
                this.childBoneIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
            }
            else
            {
                this.ChildVector = PMXVector3.LoadFromStreamStatic(br);
            }

            this.Rotatable = ((flags & PMXBone.BONE_CAN_ROTATE) != 0);
            this.Translatable = ((flags & PMXBone.BONE_CAN_TRANSLATE) != 0);
            this.Visible = ((flags & PMXBone.BONE_IS_VISIBLE) != 0);
            this.Operating = ((flags & PMXBone.BONE_CAN_MANIPULATE) != 0);            

            bool extRotation = ((flags & PMXBone.BONE_IS_EXTERNAL_ROTATION) != 0);
            bool extTranslation = ((flags & PMXBone.BONE_IS_EXTERNAL_TRANSLATION) != 0);

            int rotFlag = 0;
            if(extRotation)
            {
                rotFlag |= 1;
            }
            if (extTranslation)
            {
                rotFlag |= 2;
            }
            this.ExternalModificationType = (BoneExternalModificationType)rotFlag;
            if(this.ExternalModificationType != BoneExternalModificationType.None)
            {
                this.externalBoneIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
                this.ExternalBoneEffect = br.ReadSingle();
            }

            this.FixedAxis = ((flags & PMXBone.BONE_HAS_FIXED_AXIS) != 0);
            if (this.FixedAxis)
            {
                this.AxisLimit = PMXVector3.LoadFromStreamStatic(br);
            }

            this.LocalCoordinates = ((flags & PMXBone.BONE_HAS_LOCAL_COORDINATE) != 0);
            if(this.LocalCoordinates)
            {
                this.LocalCoordinatesX = PMXVector3.LoadFromStreamStatic(br);
                this.LocalCoordinatesZ = PMXVector3.LoadFromStreamStatic(br);
            }

            this.HasExternalParent = ((flags & PMXBone.BONE_IS_EXTERNAL_PARENT_DEFORM) != 0);
            if(this.HasExternalParent)
            {
                this.ExternalParentKey = br.ReadInt32();
            }

            this.TransformPhysicsFirst = ((flags & PMXBone.BONE_IS_AFTER_PHYSICS_DEFORM) != 0);

            bool isIKBone = ((flags & PMXBone.BONE_IS_IK) != 0);
            if(isIKBone)
            {
                PMXIK ikData = new PMXIK(this.Model, this);
                ikData.LoadFromStream(br, importSettings);
                this.IK = ikData;
            } else
            {
                this.IK = null;
            }
        }

        public override void WriteToStream(BinaryWriter bw, PMXExportSettings exportSettings)
        {
            short flags = 0x0000;

            flags |= (short)(this.HasChildBone ? PMXBone.BONE_TAILPOS_IS_BONE : 0x0000);

            flags |= (short)(this.Rotatable ? PMXBone.BONE_CAN_ROTATE : 0x0000);
            flags |= (short)(this.Translatable ? PMXBone.BONE_CAN_TRANSLATE : 0x0000);
            flags |= (short)(this.Visible ? PMXBone.BONE_IS_VISIBLE : 0x0000);
            flags |= (short)(this.Operating ? PMXBone.BONE_CAN_MANIPULATE : 0x0000);

            flags |= (short)((this.ExternalModificationType == BoneExternalModificationType.Both || this.ExternalModificationType == BoneExternalModificationType.Rotation) ? PMXBone.BONE_IS_EXTERNAL_ROTATION : 0x0000);
            flags |= (short)((this.ExternalModificationType == BoneExternalModificationType.Both || this.ExternalModificationType == BoneExternalModificationType.Translation) ? PMXBone.BONE_IS_EXTERNAL_TRANSLATION : 0x0000);

            flags |= (short)(this.FixedAxis ? PMXBone.BONE_HAS_FIXED_AXIS : 0x0000);
            flags |= (short)(this.LocalCoordinates ? PMXBone.BONE_HAS_LOCAL_COORDINATE : 0x0000);
            flags |= (short)(this.HasExternalParent ? PMXBone.BONE_IS_EXTERNAL_PARENT_DEFORM : 0x0000);

            flags |= (short)(this.TransformPhysicsFirst ? PMXBone.BONE_IS_AFTER_PHYSICS_DEFORM : 0x0000);

            flags |= (short)((this.IK != null) ? PMXBone.BONE_IS_IK : 0x0000);

            PMXParser.WriteString(bw, exportSettings.TextEncoding, this.NameJP);
            PMXParser.WriteString(bw, exportSettings.TextEncoding, this.NameEN);

            this.Position.WriteToStream(bw);

            PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.Parent, exportSettings, true));

            bw.Write((Int32)this.Layer);
            bw.Write((Int16)flags);

            if(this.HasChildBone)
            {
                PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.ChildBone, exportSettings, true));
            }
            else
            {
                this.ChildVector.WriteToStream(bw);
            }

            if (this.ExternalModificationType != BoneExternalModificationType.None)
            {
                PMXParser.WriteIndex(bw, exportSettings.BitSettings.BoneIndexLength, PMXBone.CheckIndexInModel(this.ExternalBone, exportSettings, true));
                bw.Write(this.ExternalBoneEffect);
            }

            if (this.FixedAxis)
            {
                this.AxisLimit.WriteToStream(bw);                
            }

            if (this.LocalCoordinates)
            {
                this.LocalCoordinatesX.WriteToStream(bw);
                this.LocalCoordinatesZ.WriteToStream(bw);
            }

            if (this.HasExternalParent)
            {
                bw.Write((Int32)this.ExternalParentKey);
            }

            if(this.IK != null)
            {
                this.IK.WriteToStream(bw, exportSettings);
            }
        }

        /// <summary>
        /// Checks if the bone is part of a given model.
        /// </summary>
        /// <param name="bn"></param>
        /// <param name="exportSettings"></param>
        /// <param name="nullAcceptable"></param>
        /// <returns></returns>
        public static int CheckIndexInModel(PMXBone bn, PMXExportSettings exportSettings, bool nullAcceptable = true)
        {
            if(bn == null)
            {
                if(nullAcceptable)
                {
                    return -1;
                }
                else
                {
                    throw new InvalidDataException("Bone mustn't be null!");
                }
            }

            PMXModel model = exportSettings.Model;

            int index = model.Bones.IndexOf(bn);
            if(index < 0)
            {
                throw new InvalidDataException("Bone not a member of model!");
            }
            return index;
        }        
    }
}
