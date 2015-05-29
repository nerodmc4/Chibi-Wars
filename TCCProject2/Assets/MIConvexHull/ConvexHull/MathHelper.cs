#pragma warning disable 414
namespace MIConvexHull
{
    using System;

    class MathHelper
    {
        readonly int Dimension;

        double[] ntX, ntY, ntZ;
        double[] nDNormalSolveVector;
        double[,] nDMatrix;
        double[][] jaggedNDMatrix;

        static void GaussElimination(int nDim, double[][] pfMatr, double[] pfVect, double[] pfSolution)
        {
            double fMaxElem;
            double fAcc;

            int i, j, k, m;

            for (k = 0; k < (nDim - 1); k++) 
            {
                var rowK = pfMatr[k];

                fMaxElem = Math.Abs(rowK[k]);
                m = k;
                for (i = k + 1; i < nDim; i++)
                {
                    if (fMaxElem < Math.Abs(pfMatr[i][k]))
                    {
                        fMaxElem = pfMatr[i][k];
                        m = i;
                    }
                }               
                if (m != k)
                {
                    var rowM = pfMatr[m];
                    for (i = k; i < nDim; i++)
                    {
                        fAcc = rowK[i];
                        rowK[i] = rowM[i];
                        rowM[i] = fAcc;
                    }
                    fAcc = pfVect[k];
                    pfVect[k] = pfVect[m];
                    pfVect[m] = fAcc;
                }

                for (j = (k + 1); j < nDim; j++)
                {
                    var rowJ = pfMatr[j];
                    fAcc = -rowJ[k] / rowK[k];
                    for (i = k; i < nDim; i++)
                    {
                        rowJ[i] = rowJ[i] + fAcc * rowK[i];
                    }
                    pfVect[j] = pfVect[j] + fAcc * pfVect[k];
                }
            }

            for (k = (nDim - 1); k >= 0; k--)
            {
                var rowK = pfMatr[k];
                pfSolution[k] = pfVect[k];
                for (i = (k + 1); i < nDim; i++)
                {
                    pfSolution[k] -= (rowK[i] * pfSolution[i]);
                }
                pfSolution[k] = pfSolution[k] / rowK[k];
            }
        }

        public static double LengthSquared(double[] x)
        {
            double norm = 0;
            for (int i = 0; i < x.Length; i++)
            {
                var t = x[i];
                norm += t * t;
            }
            return norm;
        }

        void Normalize(double[] x)
        {
            double norm = 0;
            for (int i = 0; i < Dimension; i++)
            {
                var t = x[i];
                norm += t * t;
            }
            double f = 1.0 / Math.Sqrt(norm);
            for (int i = 0; i < Dimension; i++) x[i] *= f;
        }

        public void SubtractFast(double[] x, double[] y, double[] target)
        {
            for (int i = 0; i < Dimension; i++)
            {
                target[i] = x[i] - y[i];
            }
        }

        void FindNormalVector4D(VertexWrap[] vertices, double[] normal)
        {
            SubtractFast(vertices[1].PositionData, vertices[0].PositionData, ntX);
            SubtractFast(vertices[2].PositionData, vertices[1].PositionData, ntY);
            SubtractFast(vertices[3].PositionData, vertices[2].PositionData, ntZ);

            var x = ntX;
            var y = ntY;
            var z = ntZ;

            var nx = x[3] * (y[2] * z[1] - y[1] * z[2])
                   + x[2] * (y[1] * z[3] - y[3] * z[1])
                   + x[1] * (y[3] * z[2] - y[2] * z[3]);
            var ny = x[3] * (y[0] * z[2] - y[2] * z[0])
                   + x[2] * (y[3] * z[0] - y[0] * z[3])
                   + x[0] * (y[2] * z[3] - y[3] * z[2]);
            var nz = x[3] * (y[1] * z[0] - y[0] * z[1])
                   + x[1] * (y[0] * z[3] - y[3] * z[0])
                   + x[0] * (y[3] * z[1] - y[1] * z[3]);
            var nw = x[2] * (y[0] * z[1] - y[1] * z[0])
                   + x[1] * (y[2] * z[0] - y[0] * z[2])
                   + x[0] * (y[1] * z[2] - y[2] * z[1]);

            double norm = System.Math.Sqrt(nx * nx + ny * ny + nz * nz + nw * nw);

            double f = 1.0 / norm;
            normal[0] = f * nx;
            normal[1] = f * ny;
            normal[2] = f * nz;
            normal[3] = f * nw;
        }

        void FindNormalVector3D(VertexWrap[] vertices, double[] normal)
        {
            SubtractFast(vertices[1].PositionData, vertices[0].PositionData, ntX);
            SubtractFast(vertices[2].PositionData, vertices[1].PositionData, ntY);

            var x = ntX;
            var y = ntY;

            var nx = x[1] * y[2] - x[2] * y[1];
            var ny = x[2] * y[0] - x[0] * y[2];
            var nz = x[0] * y[1] - x[1] * y[0];

            double norm = System.Math.Sqrt(nx * nx + ny * ny + nz * nz);

            double f = 1.0 / norm;
            normal[0] = f * nx;
            normal[1] = f * ny;
            normal[2] = f * nz;
        }

        void FindNormalVector2D(VertexWrap[] vertices, double[] normal)
        {
            SubtractFast(vertices[1].PositionData, vertices[0].PositionData, ntX);

            var x = ntX;

            var nx = -x[1];
            var ny = x[0];

            double norm = System.Math.Sqrt(nx * nx + ny * ny);

            double f = 1.0 / norm;
            normal[0] = f * nx;
            normal[1] = f * ny;
        }

        public void FindNormalVector(VertexWrap[] vertices, double[] normalData)
        {
            switch (Dimension)
            {
                case 2: FindNormalVector2D(vertices, normalData); break;
                case 3: FindNormalVector3D(vertices, normalData); break;
                case 4: FindNormalVector4D(vertices, normalData); break;
                default:
                    {
                        for (var i = 0; i < Dimension; i++) nDNormalSolveVector[i] = 1.0;
                        for (var i = 0; i < Dimension; i++)
                        {
                            var row = jaggedNDMatrix[i];
                            var pos = vertices[i].Vertex.Position;
                            for (int j = 0; j < Dimension; j++) row[j] = pos[j];
                        }
                        GaussElimination(Dimension, jaggedNDMatrix, nDNormalSolveVector, normalData);
                        Normalize(normalData);
                        break;
                    }
            }
        }

		public double GetVertexDistance(VertexWrap v, ConvexFaceInternal f)
        {
            double[] normal = f.Normal;
            double[] p = v.PositionData;
            double distance = f.Offset;
            for (int i = 0; i < Dimension; i++) distance += normal[i] * p[i];
            return distance;
        }

        public MathHelper(int dimension)
        {
            this.Dimension = dimension;

            ntX = new double[Dimension];
            ntY = new double[Dimension];
            ntZ = new double[Dimension];
            
            nDNormalSolveVector = new double[Dimension];
            jaggedNDMatrix = new double[Dimension][];
            for (var i = 0; i < Dimension; i++)
            {
                nDNormalSolveVector[i] = 1.0;
                jaggedNDMatrix[i] = new double[Dimension];
            }
            nDMatrix = new double[Dimension, Dimension];
        }
    }
}
#pragma warning restore 414





