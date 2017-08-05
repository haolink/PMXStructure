using System;
using System.IO;

using PMXStructure.PMXClasses.Helpers;
using PMXStructure.PMXClasses.General;

namespace PMXStructure.PMXClasses.Parts
{
    public class PMXJoint : PMXBasePart
    {
        public enum JointType
        {
            SpringSixDOF = 0,
            SixDOF = 1,
            P2P = 2,
            ConeTwist = 3,
            Slider = 4,
            Hinge = 5
        }

        public string NameJP { get; set; }
        public string NameEN { get; set; }

        public JointType Type { get; set; }

        public PMXRigidBody RigidBodyA { get; set; }
        public PMXRigidBody RigidBodyB { get; set; }

        public PMXVector3 Position { get; set; }
        public PMXVector3 Rotation { get; set; }

        public PMXVector3 TranslationLimitMin { get; set; }
        public PMXVector3 TranslationLimitMax { get; set; }

        public PMXVector3 RotationLimitMin { get; set; }
        public PMXVector3 RotationLimitMax { get; set; }

        public PMXVector3 SpringConstantTranslation { get; set; }
        public PMXVector3 SpringConstantRotation { get; set; }

        public PMXJoint(PMXModel model) : base(model)
        {
            this.Position = new PMXVector3();
            this.Rotation = new PMXVector3();
            this.TranslationLimitMin = new PMXVector3();
            this.TranslationLimitMax = new PMXVector3();
            this.RotationLimitMin = new PMXVector3();
            this.RotationLimitMax = new PMXVector3();
            this.SpringConstantTranslation = new PMXVector3();
            this.SpringConstantRotation = new PMXVector3();
        }

        public override void LoadFromStream(BinaryReader br, MMDImportSettings importSettings)
        {
            this.NameJP = PMXParser.ReadString(br, importSettings.TextEncoding);
            this.NameEN = PMXParser.ReadString(br, importSettings.TextEncoding);

            this.Type = (JointType)(int)br.ReadByte();

            int rigidBodyIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.RigidBodyIndexLength);
            this.RigidBodyA = this.Model.RigidBodies[rigidBodyIndex];

            rigidBodyIndex = PMXParser.ReadIndex(br, importSettings.BitSettings.RigidBodyIndexLength);
            this.RigidBodyB = this.Model.RigidBodies[rigidBodyIndex];

            this.Position = PMXVector3.LoadFromStreamStatic(br);
            this.Rotation = PMXVector3.LoadFromStreamStatic(br);
            this.TranslationLimitMin = PMXVector3.LoadFromStreamStatic(br);
            this.TranslationLimitMax = PMXVector3.LoadFromStreamStatic(br);
            this.RotationLimitMin = PMXVector3.LoadFromStreamStatic(br);
            this.RotationLimitMax = PMXVector3.LoadFromStreamStatic(br);
            this.SpringConstantTranslation = PMXVector3.LoadFromStreamStatic(br);
            this.SpringConstantRotation = PMXVector3.LoadFromStreamStatic(br);
        }

        public override void FinaliseAfterImport()
        {
            //Not required
        }

        public override void WriteToStream(BinaryWriter bw, PMXExportSettings exportSettings)
        {
            throw new NotImplementedException();
        }
    }
}
