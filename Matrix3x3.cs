using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;

namespace HelmertLSA
{
    public class Matrix3x3
    {
        private readonly Matrix<float> matrix;

        public Matrix3x3(float[,] elements)
        {
            if (elements.GetLength(0) != 3 || elements.GetLength(1) != 3)
                throw new ArgumentException("Input must be a 3x3 array.");
            matrix = Matrix<float>.Build.DenseOfArray(elements);
        }

        public Matrix3x3(Matrix<float> matrix)
        {
            if (matrix.RowCount != 3 || matrix.ColumnCount != 3)
                throw new ArgumentException("Matrix must be 3x3.");
            this.matrix = matrix.Clone();
        }

        /// <summary>
        /// Creates an identity 3x3 matrix.
        /// </summary>
        /// <returns>A 3x3 identity matrix.</returns>
        public static Matrix3x3 Identity()
        {
            return new Matrix3x3(Matrix<float>.Build.DenseIdentity(3));
        }

        /// <summary>
        /// Returns the transpose of the matrix.
        /// </summary>
        /// <returns>A new Matrix3x3 object, which is the transpose of the current matrix.</returns>
        public Matrix3x3 Transpose()
        {
            return new Matrix3x3(matrix.Transpose());
        }

        /// <summary>
        /// Returns the inverse of the matrix.
        /// </summary>
        /// <returns>A new Matrix3x3 object, which is the inverse of the current matrix.</returns>
        public Matrix3x3 Inverse()
        {
            if (Math.Abs(matrix.Determinant()) < 1e-6)
                throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
            return new Matrix3x3(matrix.Inverse());
        }

        /// <summary>
        /// Multiplies the matrix by a 3D vector.
        /// </summary>
        /// <param name="vector">A 3D vector.</param>
        /// <returns>The resulting 3D vector after multiplication.</returns>
        public Vector<float> Multiply(Vector<float> vector)
        {
            if (vector.Count != 3)
                throw new ArgumentException("Vector must have 3 elements.");
            return matrix * vector;
        }

        /// <summary>
        /// Multiplies the matrix by another 3x3 matrix.
        /// </summary>
        /// <param name="other">Another 3x3 matrix.</param>
        /// <returns>The resulting Matrix3x3 after multiplication.</returns>
        public Matrix3x3 Multiply(Matrix3x3 other)
        {
            return new Matrix3x3(this.matrix * other.matrix);
        }

        /// <summary>
        /// Accessor for matrix elements.
        /// </summary>
        /// <param name="row">Row index (0-based).</param>
        /// <param name="column">Column index (0-based).</param>
        /// <returns>The value at the specified row and column.</returns>
        public float this[int row, int column]
        {
            get => matrix[row, column];
            set => matrix[row, column] = value;
        }

        /// <summary>
        /// Converts the Matrix3x3 object to a MathNet Numerics matrix.
        /// </summary>
        /// <returns>A clone of the internal MathNet Numerics matrix.</returns>
        public Matrix<float> ToMatrix()
        {
            return matrix.Clone();
        }

        /// <summary>
        /// Computes the determinant of the matrix.
        /// </summary>
        /// <returns>The determinant of the matrix.</returns>
        public float Determinant()
        {
            return matrix.Determinant();
        }

        /// <summary>
        /// Checks if the matrix is approximately equal to another matrix.
        /// </summary>
        /// <param name="other">The other 3x3 matrix to compare.</param>
        /// <param name="tolerance">A tolerance value for approximate equality.</param>
        /// <returns>True if the matrices are approximately equal; otherwise, false.</returns>
        public bool IsApproximatelyEqual(Matrix3x3 other, float tolerance = 1e-6f)
        {
            var diff = this.matrix - other.matrix;
            return diff.Enumerate().All(value => Math.Abs(value) < tolerance);
        }
    }
}