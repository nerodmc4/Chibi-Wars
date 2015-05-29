namespace MIConvexHull{
    using System.Collections.Generic;

    sealed class VertexWrap{
        public IVertex Vertex;

        public double[] PositionData;

        public int Index;

       public bool Marked;
    }

    class VertexWrapComparer : IComparer<VertexWrap>{
        public static readonly VertexWrapComparer Instance = new VertexWrapComparer();

        public int Compare(VertexWrap x, VertexWrap y){
            return x.Index.CompareTo(y.Index);
        }
    }

    sealed class DeferredFace
    {
        public ConvexFaceInternal Face, Pivot, OldFace;
        public int FaceIndex, PivotIndex;
    }

    sealed class FaceConnector
    {
        public ConvexFaceInternal Face;
        public int EdgeIndex;
        public int[] Vertices;
        public uint HashCode;
        public FaceConnector Previous;
        public FaceConnector Next;
        public FaceConnector(int dimension){
            Vertices = new int[dimension - 1];
        }

        public void Update(ConvexFaceInternal face, int edgeIndex, int dim){
            this.Face = face;
            this.EdgeIndex = edgeIndex;

            uint hashCode = 31;

            var vs = face.Vertices;
            for (int i = 0, c = 0; i < dim; i++){
                if (i != edgeIndex){
                    var v = vs[i].Index;
                    this.Vertices[c++] = v;
                    hashCode += unchecked(23 * hashCode + (uint)v);
                }
            }

            this.HashCode = hashCode;
        }

        public static bool AreConnectable(FaceConnector a, FaceConnector b, int dim){
            if (a.HashCode != b.HashCode) return false;

            var n = dim - 1;
            var av = a.Vertices;
            var bv = b.Vertices;
            for (int i = 0; i < n; i++){
                if (av[i] != bv[i]) return false;
            }

            return true;
        }

        public static void Connect(FaceConnector a, FaceConnector b){
            a.Face.AdjacentFaces[a.EdgeIndex] = b.Face;
            b.Face.AdjacentFaces[b.EdgeIndex] = a.Face;
        }
    }

    sealed class ConvexFaceInternal{
        public ConvexFaceInternal(int dimension, VertexBuffer beyondList){
            AdjacentFaces = new ConvexFaceInternal[dimension];
            VerticesBeyond = beyondList;
            Normal = new double[dimension];
            Vertices = new VertexWrap[dimension];
        }

        public ConvexFaceInternal[] AdjacentFaces;
        public VertexBuffer VerticesBeyond;
        public VertexWrap FurthestVertex;
        public VertexWrap[] Vertices;        
        public double[] Normal;
        public bool IsNormalFlipped;
        public double Offset;
        public int Tag;
        public ConvexFaceInternal Previous;
        public ConvexFaceInternal Next;
        public bool InList;
    }
}
