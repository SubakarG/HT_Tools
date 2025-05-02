using MathNet.Numerics.LinearAlgebra;
using System;

namespace HelmertLSA
{
    public class Point3D
    {
        // Properties
        public string Id { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        // Computed property to get the vector representation of the point
        public Vector<float> Vector => Vector<float>.Build.Dense(new float[] { X, Y, Z });

        // Constructor to initialize all properties
        public Point3D(string id, float x, float y, float z)
        {
            this.Id = id; // Use 'this' to disambiguate
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        // Constructor to initialize from a vector
        public Point3D(string id, Vector<float> v)
        {
            if (v.Count != 3) // Ensure the vector has exactly 3 elements
            {
                throw new ArgumentException("Vector must have exactly 3 elements.", nameof(v));
            }

            this.Id = id; // Use 'this' to disambiguate
            this.X = v[0];
            this.Y = v[1];
            this.Z = v[2];
        }

        // Override ToString for better debugging and logging
        public override string ToString()
        {
            return $"{Id}: ({X}, {Y}, {Z})";
        }
    }
}