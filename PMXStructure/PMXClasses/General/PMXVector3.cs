using System;
using System.IO;

namespace PMXStructure.PMXClasses.General
{
    public class PMXVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public PMXVector3()
        {
            this.X = 0.0f;
            this.Y = 0.0f;
            this.Z = 0.0f;
        }

        public PMXVector3(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public static PMXVector3 LoadFromStreamStatic(BinaryReader br)
        {
            PMXVector3 res = new PMXVector3();
            res.LoadFromStream(br);
            return res;
        }

        public void LoadFromStream(BinaryReader br)
        {
            this.X = br.ReadSingle();
            this.Y = br.ReadSingle();
            this.Z = br.ReadSingle();
        }

        public void WriteToStream(BinaryWriter bw)
        {
            bw.Write(this.X);
            bw.Write(this.Y);
            bw.Write(this.Z);
        }

        public PMXVector3 Normalize()
        {
            PMXVector3 n = new PMXVector3(this.X, this.Y, this.Z);
            float v = this.Value;

            if (v != 0.0f)
            {
                n.X /= v;
                n.Y /= v;
                n.Z /= v;
            }
            return n;
        }

        public float Value
        {
            get
            {
                return (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            }            
        }

        public static PMXVector3 operator +(PMXVector3 a, PMXVector3 b)
        {
            return new PMXVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static PMXVector3 operator -(PMXVector3 a, PMXVector3 b)
        {
            return new PMXVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static PMXVector3 operator *(PMXVector3 a, float b)
        {
            return new PMXVector3(a.X * b, a.Y * b, a.Z * b);
        }

        public static PMXVector3 operator *(float a, PMXVector3 b)
        {
            return (b * a);
        }

        public static PMXVector3 operator /(PMXVector3 a, float b)
        {
            return a * (1.0f / b);
        }

        public static PMXVector3 operator /(float a, PMXVector3 b)
        {
            return b * (1.0f / a);
        }

        public float DotProduct(PMXVector3 b)
        {
            return (this.X * b.X + this.Y * b.Y + this.Z * b.Z);
        }

        public PMXVector3 CrossProduct(PMXVector3 b)
        {
            return new PMXVector3(
                this.Y * b.Z - this.Z * b.Y,
                this.Z * b.X - this.X * b.Z,
                this.X * b.Y - this.Y * b.X
            );
        }
    }
}
