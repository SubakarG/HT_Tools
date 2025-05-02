using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelmertLSA
{
    public abstract class SolverBase
    {
        public abstract TransformationResult Solve(List<Vector<float>> source, List<Vector<float>> target);
    }

    public class TransformationResult
    {
        public float Scale { get; init; }
        public Matrix<float> Rotation { get; init; } = Matrix<float>.Build.DenseIdentity(3); // Default value
        public Vector<float> Translation { get; init; } = Vector<float>.Build.Dense(3); // Default value
    }

    public class HelmertSolver : SolverBase
    {
        public override TransformationResult Solve(List<Vector<float>> source, List<Vector<float>> target)
        {
            var centroidSource = ComputeCentroid(source);
            var centroidTarget = ComputeCentroid(target);

            var centeredSource = source.Select(p => p - centroidSource).ToList();
            var centeredTarget = target.Select(p => p - centroidTarget).ToList();

            var H = Matrix<float>.Build.Dense(3, 3);
            for (int i = 0; i < source.Count; i++)
            {
                H += centeredSource[i].ToColumnMatrix() * centeredTarget[i].ToRowMatrix();
            }

            var svd = H.Svd();
            var rotation = svd.VT.Transpose() * svd.U.Transpose();

            if (rotation.Determinant() < 0)
            {
                var diag = Matrix<float>.Build.DenseIdentity(3);
                diag[2, 2] = -1;
                rotation = svd.VT.Transpose() * diag * svd.U.Transpose();
            }

            float scale = centeredSource.Sum(p => p.DotProduct(rotation * p)) / centeredSource.Sum(p => p.DotProduct(p));
            var translation = centroidTarget - scale * rotation * centroidSource;

            return new TransformationResult { Scale = scale, Rotation = rotation, Translation = translation };
        }

        public List<Point3D> ApplyTransformation(List<Point3D> points, TransformationResult result)
        {
            return points.Select(point =>
            {
                var transformed = result.Scale * result.Rotation * point.Vector + result.Translation;
                return new Point3D(point.Id, transformed[0], transformed[1], transformed[2]);
            }).ToList();
        }

        private Vector<float> ComputeCentroid(List<Vector<float>> points)
        {
            return points.Aggregate(Vector<float>.Build.Dense(points[0].Count), (acc, p) => acc + p) / points.Count;
        }
    }
}