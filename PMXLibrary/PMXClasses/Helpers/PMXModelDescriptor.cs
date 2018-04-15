using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMXStructure.PMXClasses
{
    public class PMXModelDescriptor
    { 
        public InheritableType<PMXVertex> VertexType { get; set; }
        public InheritableType<PMXTriangle> TriangleType { get; set; }
        public InheritableType<PMXMaterial> MaterialType { get; set; }
        public InheritableType<PMXBone> BoneType { get; set; }
        public InheritableType<PMXMorph> MorphType { get; set; }
        public InheritableType<PMXRigidBody> RigidBodyType { get; set; }
        public InheritableType<PMXJoint> JointType { get; set; }

        public PMXModelDescriptor()
        {
            this.VertexType = new InheritableType<PMXVertex>(typeof(PMXVertex));
            this.TriangleType = new InheritableType<PMXTriangle>(typeof(PMXTriangle));
            this.MaterialType = new InheritableType<PMXMaterial>(typeof(PMXMaterial));
            this.BoneType = new InheritableType<PMXBone>(typeof(PMXBone));
            this.MorphType = new InheritableType<PMXMorph>(typeof(PMXMorph));
            this.RigidBodyType = new InheritableType<PMXRigidBody>(typeof(PMXRigidBody));
            this.JointType = new InheritableType<PMXJoint>(typeof(PMXJoint));
        }
    }
}
