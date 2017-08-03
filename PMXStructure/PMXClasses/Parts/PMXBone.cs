using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public PMXIK IK { get; set; }

        public PMXBone(PMXModel model) : base(model)
        {
        
        }

        public override void FinaliseAfterImport()
        {
            throw new NotImplementedException();
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

            bool isIKBone = ((flags & PMXBone.BONE_IS_IK) != 0);
            if(isIKBone)
            {
                PMXIK ikData = new PMXIK(this.Model, this);
                ikData.LoadFromStream(br, importSettings);
                this.IK = ikData;
            }
        }
    }
}
