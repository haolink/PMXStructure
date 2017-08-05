using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.General;
using PMXStructure.PMXClasses.Parts.RigidBodies;

namespace PMXStructure.PMXClasses.Parts
{
    public class PMXRigidBody : PMXBasePart
    {
        public const byte SHAPE_IDENTIFY_SPHERE = 0;
        public const byte SHAPE_IDENTIFY_BOX = 1;
        public const byte SHAPE_IDENTIFY_CAPSULE = 2;        

        public enum BodyShape
        {
            Sphere = 0,
            Box = 1,
            Capsule = 2
        }

        public enum BodyType
        {
            FollowUp = 0,
            Physics = 1,
            PhysicsBoneAlignment = 2
        }

        public string NameJP { get; set; }
        public string NameEN { get; set; }

        public PMXBone Bone { get; set; }

        public byte CollissionGroup { get; set; }

        private PMXNoCollissionGroup NoCollissionGroups { get; }

        public BodyShape Shape { get; set; }
        public BodyType Type { get; set; }

        private PMXVector3 _shapeSize;

        public float Radius { get { return this._shapeSize.X; } set { this._shapeSize.X = value; } }

        public float Height { get { return this._shapeSize.Y; } set { this._shapeSize.Y = value; } }

        public float Width { get { return this._shapeSize.X; } set { this._shapeSize.X = value; } }
        public float Depth { get { return this._shapeSize.Z; } set { this._shapeSize.Z = value; } }

        public PMXVector3 Position { get; set; }
        public PMXVector3 Rotation { get; set; }

        public float Mass { get; set; }
        public float LinearDamping { get; set; }
        public float AngularDamping { get; set; }
        public float Repulsion { get; set; }
        public float Friction { get; set; }

        public PMXRigidBody(PMXModel model) : base(model)
        {
            this._shapeSize = new PMXVector3();
            this.Position = new PMXVector3();
            this.Rotation = new PMXVector3();
            this.NoCollissionGroups = new PMXNoCollissionGroup(this.Model, this);
        }

        public override void FinaliseAfterImport()
        {
            this.NoCollissionGroups.FinaliseAfterImport();
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.NameJP = PMXParser.ReadString(br, importSettings.TextEncoding);
            this.NameEN = PMXParser.ReadString(br, importSettings.TextEncoding);

            int boneIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.BoneIndexLength);
            this.Bone = this.Model.Bones[boneIndex];

            this.CollissionGroup = br.ReadByte();
            this.NoCollissionGroups.LoadFromStream(br, importSettings);            

            this.Shape = (BodyShape)(int)br.ReadByte();
            this._shapeSize = PMXVector3.LoadFromStreamStatic(br);

            this.Position = PMXVector3.LoadFromStreamStatic(br);
            this.Rotation = PMXVector3.LoadFromStreamStatic(br);

            this.Mass = br.ReadSingle();
            this.LinearDamping = br.ReadSingle();
            this.AngularDamping = br.ReadSingle();
            this.Repulsion = br.ReadSingle();
            this.Friction = br.ReadSingle();

            this.Type = (BodyType)(int)br.ReadByte();
        }
    }
}
