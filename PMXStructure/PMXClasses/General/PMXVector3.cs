﻿using System;
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

        public void Normalize()
        {
            float value = (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            if(value != 0.0f)
            {
                this.X /= value;
                this.Y /= value;
                this.Z /= value;
            }            
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
    }
}
