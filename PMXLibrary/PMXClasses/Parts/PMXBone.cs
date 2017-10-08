using System;
using System.IO;

namespace PMXStructure.PMXClasses
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

        public const byte PMD_BONE_TYPE_ROTATE = 0;
        public const byte PMD_BONE_TYPE_ROTATE_MOVE = 1;
        public const byte PMD_BONE_TYPE_IK = 2;
        public const byte PMD_BONE_TYPE_IK_CHILD = 4;
        public const byte PMD_BONE_TYPE_EXTERNAL_ROTATOR = 5;
        public const byte PMD_BONE_TYPE_IK_TARGET = 6;
        public const byte PMD_BONE_TYPE_INVISIBLE = 7;
        public const byte PMD_BONE_TYPE_TWIST = 8;
        public const byte PMD_BONE_TYPE_TWIST_INVISIBLE = 9;

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
        private bool _isPMDTwist; //Import of PMD files only

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

        public void LoadFromStream(BinaryReader br, MMDImportSettings importSettings, out int pmdIKIndex)
        {
            pmdIKIndex = -1;

            if (importSettings.Format == MMDImportSettings.ModelFormat.PMX)
            { //PMX
                this.NameJP = PMXParser.ReadString(br, importSettings.TextEncoding);
                this.NameEN = PMXParser.ReadString(br, importSettings.TextEncoding);

                this.Position = PMXVector3.LoadFromStreamStatic(br);

                this.parentIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);

                this.Layer = br.ReadInt32();

                short flags = br.ReadInt16();

                this.HasChildBone = ((flags & PMXBone.BONE_TAILPOS_IS_BONE) != 0);
                if (this.HasChildBone)
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
                if (extRotation)
                {
                    rotFlag |= 1;
                }
                if (extTranslation)
                {
                    rotFlag |= 2;
                }
                this.ExternalModificationType = (BoneExternalModificationType)rotFlag;
                if (this.ExternalModificationType != BoneExternalModificationType.None)
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
                if (this.LocalCoordinates)
                {
                    this.LocalCoordinatesX = PMXVector3.LoadFromStreamStatic(br);
                    this.LocalCoordinatesZ = PMXVector3.LoadFromStreamStatic(br);
                }

                this.HasExternalParent = ((flags & PMXBone.BONE_IS_EXTERNAL_PARENT_DEFORM) != 0);
                if (this.HasExternalParent)
                {
                    this.ExternalParentKey = br.ReadInt32();
                }

                this.TransformPhysicsFirst = ((flags & PMXBone.BONE_IS_AFTER_PHYSICS_DEFORM) != 0);

                bool isIKBone = ((flags & PMXBone.BONE_IS_IK) != 0);
                if (isIKBone)
                {
                    PMXIK ikData = new PMXIK(this.Model, this);
                    ikData.LoadFromStream(br, importSettings);
                    this.IK = ikData;
                }
                else
                {
                    this.IK = null;
                }
            }

            else
            { //PMD
                this.NameJP = PMDParser.ReadString(br, 20, importSettings.TextEncoding);
                this.parentIndex = br.ReadInt16();

                this.HasChildBone = true;
                this.childBoneIndex = br.ReadUInt16();

                byte type = br.ReadByte();

                ushort ikIndex = br.ReadUInt16();

                this.Position = PMXVector3.LoadFromStreamStatic(br);

                switch(type)
                {
                    case PMD_BONE_TYPE_ROTATE:
                        //Default
                        break;
                    case PMD_BONE_TYPE_ROTATE_MOVE:
                        this.Translatable = true;
                        break;
                    case PMD_BONE_TYPE_IK:
                        //IK parameters will be initialised later
                        this.Translatable = true;
                        break;
                    case PMD_BONE_TYPE_IK_CHILD:
                        //PMX doesn't even bother about these                        
                        break;
                    case PMD_BONE_TYPE_EXTERNAL_ROTATOR:
                        this.ExternalModificationType = BoneExternalModificationType.Rotation;
                        this.externalBoneIndex = (int)ikIndex;
                        break;
                    case PMD_BONE_TYPE_IK_TARGET:
                        //PMX doesn't bother either                        
                        this.Visible = false;
                        break;
                    case PMD_BONE_TYPE_INVISIBLE:
                        this.Visible = false;
                        break;
                    case PMD_BONE_TYPE_TWIST:
                        //PMX handles these differently
                        this._isPMDTwist = true;
                        break;
                    case PMD_BONE_TYPE_TWIST_INVISIBLE:
                        //PMX handles these differently
                        this._isPMDTwist = true;
                        this.Visible = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Parses PMD Twist data if available.
        /// </summary>
        public void ParsePMDTwist()
        {
            if(!this._isPMDTwist || this.Parent == null)
            {
                return;
            }
            this._isPMDTwist = false;
            PMXBone p = this.Parent;

            PMXVector3 targetLocation;
            if(p.HasChildBone)
            {
                if(p.ChildBone == this)
                {
                    return;
                }
                targetLocation = p.ChildBone.Position;
            } else
            {
                targetLocation = p.Position + p.ChildVector;
            }

            PMXVector3 direction = targetLocation - this.Position;

            direction = direction.Normalize();

            this.FixedAxis = true;
            this.AxisLimit = direction;
        }

        /// <summary>
        /// Create a local coordinate axis for the current bone.
        /// </summary>
        public void CreateLocalCoodinateAxis()
        {
            PMXVector3 targetLocation;
            if (this.HasChildBone)
            {
                if (this.ChildBone == this)
                {
                    return;
                }
                targetLocation = this.ChildBone.Position;
            }
            else
            {
                targetLocation = this.Position + this.ChildVector;
            }

            PMXVector3 direction = targetLocation - this.Position;

            PMXVector3 zAxis = (new PMXVector3(0, 1, 0)).CrossProduct(direction);
            if(zAxis.Z > 0)
            {
                zAxis *= -1.0f;
            }

            /*float dotProduct = direction.DotProduct(zAxis);
            Console.WriteLine(dotProduct.ToString());*/

            this.LocalCoordinates = true;
            this.LocalCoordinatesX = direction.Normalize();
            this.LocalCoordinatesZ = zAxis.Normalize();
        }

        private static string[] PMDArmBoneNames = new string[]
        {
            "右肩", "右腕", "右ひじ", "右手首",
            "右親指０", "右親指１", "右親指２", "右親指0", "右親指1", "右親指2",
            "右人指１", "右人指２", "右人指３", "右人指1", "右人指2", "右人指3",
            "右中指１", "右中指２", "右中指３", "右中指1", "右中指2", "右中指3",
            "右薬指１", "右薬指２", "右薬指３", "右薬指1", "右薬指2", "右薬指3",
            "右小指１", "右小指２", "右小指３", "右小指1", "右小指2", "右小指3",
            "左肩", "左腕", "左ひじ", "左手首",
            "左親指０", "左親指１", "左親指２", "左親指0", "左親指1", "左親指2",
            "左人指１", "左人指２", "左人指３", "左人指1", "左人指2", "左人指3",
            "左中指１", "左中指２", "左中指３", "左中指1", "左中指2", "左中指3",
            "左薬指１", "左薬指２", "左薬指３", "左薬指1", "左薬指2", "左薬指3",
            "左小指１", "左小指２", "左小指３", "左小指1", "左小指2", "左小指3"
        };

        /// <summary>
        /// Parses PMD Twist data if available.
        /// </summary>
        public void CreateLocalCoodinateAxisForPMD()
        {
            if(Array.IndexOf<string>(PMXBone.PMDArmBoneNames, this.NameJP) >= 0)
            {
                this.CreateLocalCoodinateAxis();
            }
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            int ignoreIndex;
            this.LoadFromStream(br, importSettings, out ignoreIndex);
        }

        public override void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings)
        {
            this.WriteToStream(bw, exportSettings, null);
        }

        public void WriteToStream(BinaryWriter bw, MMDExportSettings exportSettings, PMXBone[] ikBones)
        {
            if(exportSettings.Format == MMDExportSettings.ModelFormat.PMX)
            { //PMX export
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

                if (this.HasChildBone)
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

                if (this.IK != null)
                {
                    this.IK.WriteToStream(bw, exportSettings);
                }
            }
            else
            { //PMD format
                PMDParser.WriteString(bw, 20, exportSettings.TextEncoding, this.NameJP);
                bw.Write((Int16)PMXBone.CheckIndexInModel(this.Parent, exportSettings, true));

                if (this.HasChildBone)
                {
                    bw.Write((Int16)PMXBone.CheckIndexInModel(this.ChildBone, exportSettings, true));
                } else
                {
                    bw.Write((Int16) (-1));
                }

                byte type = PMD_BONE_TYPE_ROTATE;
                short ikIndex = 0;

                if (this.Translatable)
                {
                    type = PMD_BONE_TYPE_ROTATE_MOVE;
                }

                if(this.IK != null)
                {
                    type = PMD_BONE_TYPE_IK;
                }

                if(this.IK != null)
                {
                    bool isIkTarget = false;
                    foreach (PMXBone ikBone in ikBones)
                    {
                        foreach (PMXIKLink link in ikBone.IK.IKLinks)
                        {
                            if (link.Bone == this)
                            {
                                ikIndex = (Int16)PMXBone.CheckIndexInModel(ikBone, exportSettings, true);
                                isIkTarget = true;
                                break;
                            }
                        }
                        if (isIkTarget)
                        {
                            break;
                        }
                    }

                    if (isIkTarget)
                    {
                        type = (this.Visible ? PMD_BONE_TYPE_IK_CHILD:PMD_BONE_TYPE_IK_TARGET);
                    }
                    else if(!this.Visible)
                    {
                        type = PMD_BONE_TYPE_INVISIBLE;
                    }
                    else if(this.FixedAxis)
                    {
                        type = (this.Visible ? PMD_BONE_TYPE_TWIST : PMD_BONE_TYPE_TWIST_INVISIBLE);
                    }
                    else if(this.ExternalModificationType != BoneExternalModificationType.Both)
                    {
                        type = PMD_BONE_TYPE_EXTERNAL_ROTATOR;
                        ikIndex = (Int16)PMXBone.CheckIndexInModel(this.ExternalBone, exportSettings, true);
                    }
                }

                bw.Write(type);
                bw.Write(ikIndex);

                this.Position.WriteToStream(bw);
            }
        }

        /// <summary>
        /// Updates IKs of PMD files for legs.
        /// </summary>
        public void UpdatePMDIKs()
        {
            if(this.IK != null)
            {
                this.IK.UpdatePMDIKs();
            }
        }

        /// <summary>
        /// Checks if the bone is part of a given model.
        /// </summary>
        /// <param name="bn"></param>
        /// <param name="exportSettings"></param>
        /// <param name="nullAcceptable"></param>
        /// <returns></returns>
        public static int CheckIndexInModel(PMXBone bn, MMDExportSettings exportSettings, bool nullAcceptable = true)
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
