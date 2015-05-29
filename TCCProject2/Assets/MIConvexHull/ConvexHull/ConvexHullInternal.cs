using UnityEngine;

namespace MIConvexHull
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ConvexHullInternal
    {
		const double PlaneDistanceTolerance = 0.0000001;

        bool Computed;
        readonly int Dimension;

        List<VertexWrap> InputVertices;
        List<VertexWrap> ConvexHull;
        FaceList UnprocessedFaces;
        List<ConvexFaceInternal> ConvexFaces;

        VertexWrap CurrentVertex;
        double MaxDistance;
        VertexWrap FurthestVertex;

        double[] Center;
        
        ConvexFaceInternal[] UpdateBuffer;
        int[] UpdateIndices;

        Stack<ConvexFaceInternal> TraverseStack;

        VertexBuffer EmptyBuffer; 
        VertexBuffer BeyondBuffer;
        List<ConvexFaceInternal> AffectedFaceBuffer;
        List<DeferredFace> ConeFaceBuffer;        
        HashSet<VertexWrap> SingularVertices;

        const int ConnectorTableSize = 2017;
        ConnectorList[] ConnectorTable;

        ObjectManager ObjectManager;
        MathHelper MathHelper;

        void Initialize()
        {
            ConvexHull = new List<VertexWrap>();
            UnprocessedFaces = new FaceList(); 
            ConvexFaces = new List<ConvexFaceInternal>();

            ObjectManager = new MIConvexHull.ObjectManager(Dimension);
            MathHelper = new MIConvexHull.MathHelper(Dimension);

            Center = new double[Dimension];            
            TraverseStack = new Stack<ConvexFaceInternal>();
            UpdateBuffer = new ConvexFaceInternal[Dimension];
            UpdateIndices = new int[Dimension];
            EmptyBuffer = new VertexBuffer();
            AffectedFaceBuffer = new List<ConvexFaceInternal>();
            ConeFaceBuffer = new List<DeferredFace>();
            SingularVertices = new HashSet<VertexWrap>();
            BeyondBuffer = new VertexBuffer();

            ConnectorTable = Enumerable.Range(0, ConnectorTableSize).Select(_ => new ConnectorList()).ToArray();           
        }

        int DetermineDimension()
        {
            var r = new Random();
            var VCount = InputVertices.Count;
            var dimensions = new List<int>();
            for (var i = 0; i < 10; i++)
                dimensions.Add(InputVertices[r.Next(VCount)].Vertex.Position.Length);
            var dimension = dimensions.Min();
            if (dimension != dimensions.Max()) throw new ArgumentException("Invalid input data (non-uniform dimension).");
            return dimension;
        }

        ConvexFaceInternal[] InitiateFaceDatabase()
        {
            var faces = new ConvexFaceInternal[Dimension + 1];

            for (var i = 0; i < Dimension + 1; i++)
            {
                var vertices = ConvexHull.Where((_, j) => i != j).ToArray(); 
                var newFace = new ConvexFaceInternal(Dimension, new VertexBuffer());
                newFace.Vertices = vertices;
                Array.Sort(vertices, VertexWrapComparer.Instance);
                CalculateFacePlane(newFace);
                faces[i] = newFace;
            }

            for (var i = 0; i < Dimension; i++)
            {
                for (var j = i + 1; j < Dimension + 1; j++) UpdateAdjacency(faces[i], faces[j]);
            }

            return faces;
        }
        
        private bool CalculateFacePlane(ConvexFaceInternal face)
        {
            var vertices = face.Vertices;
            var normal = face.Normal;
            MathHelper.FindNormalVector(vertices, normal);

            if (double.IsNaN(normal[0]))
            {
                return false;
            }

            double offset = 0.0;
            double centerDistance = 0.0;
            var fi = vertices[0].PositionData;
            for (int i = 0; i < Dimension; i++)
            {
                double n = normal[i];
                offset += n * fi[i];
                centerDistance += n * Center[i];
            }
            face.Offset = -offset;
            centerDistance -= offset;

            if (centerDistance > 0)
            {
                for (int i = 0; i < Dimension; i++) normal[i] = -normal[i];
                face.Offset = offset;
                face.IsNormalFlipped = true;
            }
            else face.IsNormalFlipped = false;

            return true;
        }
                
        void TagAffectedFaces(ConvexFaceInternal currentFace)
        {
            AffectedFaceBuffer.Clear();
            AffectedFaceBuffer.Add(currentFace);
            TraverseAffectedFaces(currentFace);
        }
        
        void TraverseAffectedFaces(ConvexFaceInternal currentFace)
        {
            TraverseStack.Clear();
            TraverseStack.Push(currentFace);
            currentFace.Tag = 1;

            while (TraverseStack.Count > 0)
            {
                var top = TraverseStack.Pop();
                for (int i = 0; i < Dimension; i++)
                {
                    var adjFace = top.AdjacentFaces[i];

                    if (adjFace.Tag == 0 && MathHelper.GetVertexDistance(CurrentVertex, adjFace) >= PlaneDistanceTolerance)
                    {
                        AffectedFaceBuffer.Add(adjFace);
                        adjFace.Tag = 1;
                        TraverseStack.Push(adjFace);
                    }
                }
            }
        }

        void UpdateAdjacency(ConvexFaceInternal l, ConvexFaceInternal r)
        {
            var lv = l.Vertices;
            var rv = r.Vertices;
            int i;

            for (i = 0; i < Dimension; i++) lv[i].Marked = false;

            for (i = 0; i < Dimension; i++) rv[i].Marked = true;

            for (i = 0; i < Dimension; i++) if (!lv[i].Marked) break;

            if (i == Dimension) return;

            for (int j = i + 1; j < Dimension; j++) if (!lv[j].Marked) return;

            l.AdjacentFaces[i] = r;

            for (i = 0; i < Dimension; i++) lv[i].Marked = false;
            for (i = 0; i < Dimension; i++)
            {
                if (rv[i].Marked) break;
            }
            r.AdjacentFaces[i] = l;
        }
        
        DeferredFace MakeDeferredFace(ConvexFaceInternal face, int faceIndex, ConvexFaceInternal pivot, int pivotIndex, ConvexFaceInternal oldFace)
        {
            var ret = ObjectManager.GetDeferredFace();
            
            ret.Face = face;
            ret.FaceIndex = faceIndex;
            ret.Pivot = pivot;
            ret.PivotIndex = pivotIndex;
            ret.OldFace = oldFace;

            return ret;
        }

        void ConnectFace(FaceConnector connector)
        {
            var index = connector.HashCode % ConnectorTableSize;
            var list = ConnectorTable[index];

            for (var current = list.First; current != null; current = current.Next)
            {
                if (FaceConnector.AreConnectable(connector, current, Dimension))
                {
                    list.Remove(current);
                    FaceConnector.Connect(current, connector);
                    current.Face = null;
                    connector.Face = null;
                    ObjectManager.DepositConnector(current);
                    ObjectManager.DepositConnector(connector);
                    return;
                }
            }

            list.Add(connector);
        }

        private bool CreateCone()
        {
            var currentVertexIndex = CurrentVertex.Index;
            ConeFaceBuffer.Clear();

            for (int fIndex = 0; fIndex < AffectedFaceBuffer.Count; fIndex++)
            {
                var oldFace = AffectedFaceBuffer[fIndex];

                int updateCount = 0;
                for (int i = 0; i < Dimension; i++)
                {
                    var af = oldFace.AdjacentFaces[i];
                    if (af.Tag == 0) 
                    {
                        UpdateBuffer[updateCount] = af;
                        UpdateIndices[updateCount] = i;
                        ++updateCount;
                    }
                }

                for (int i = 0; i < updateCount; i++)
                {
                    var adjacentFace = UpdateBuffer[i];

                    int oldFaceAdjacentIndex = 0;
                    var adjFaceAdjacency = adjacentFace.AdjacentFaces;
                    for (int j = 0; j < Dimension; j++)
                    {
                        if (object.ReferenceEquals(oldFace, adjFaceAdjacency[j]))
                        {
                            oldFaceAdjacentIndex = j;
                            break;
                        }
                    }

                    var forbidden = UpdateIndices[i];

                    ConvexFaceInternal newFace;

                    int oldVertexIndex;
                    VertexWrap[] vertices;

                    newFace = ObjectManager.GetFace();
                    vertices = newFace.Vertices;
                    for (int j = 0; j < Dimension; j++) vertices[j] = oldFace.Vertices[j];
                    oldVertexIndex = vertices[forbidden].Index;

                    int orderedPivotIndex;

                    // correct the ordering
                    if (currentVertexIndex < oldVertexIndex)
                    {
                        orderedPivotIndex = 0;
                        for (int j = forbidden - 1; j >= 0; j--)
                        {
                            if (vertices[j].Index > currentVertexIndex) vertices[j + 1] = vertices[j];
                            else
                            {
                                orderedPivotIndex = j + 1;
                                break;
                            }
                        }
                    }
                    else
                    {
                        orderedPivotIndex = Dimension - 1;
                        for (int j = forbidden + 1; j < Dimension; j++)
                        {
                            if (vertices[j].Index < currentVertexIndex) vertices[j - 1] = vertices[j];
                            else
                            {
                                orderedPivotIndex = j - 1;
                                break;
                            }
                        }
                    }
                    
                    vertices[orderedPivotIndex] = CurrentVertex;

                    if (!CalculateFacePlane(newFace))
                    {
                        return false;
                    }

                    ConeFaceBuffer.Add(MakeDeferredFace(newFace, orderedPivotIndex, adjacentFace, oldFaceAdjacentIndex, oldFace));
                }
            }
            
            return true;
        }

        void CommitCone()
        {
            
            ConvexHull.Add(CurrentVertex);
            
            for (int i = 0; i < ConeFaceBuffer.Count; i++)
            {
                var face = ConeFaceBuffer[i];

                var newFace = face.Face;
                var adjacentFace = face.Pivot;
                var oldFace = face.OldFace;
                var orderedPivotIndex = face.FaceIndex;

                newFace.AdjacentFaces[orderedPivotIndex] = adjacentFace;
                adjacentFace.AdjacentFaces[face.PivotIndex] = newFace;

                for (int j = 0; j < Dimension; j++)
                {
                    if (j == orderedPivotIndex) continue;
                    var connector = ObjectManager.GetConnector();
                    connector.Update(newFace, j, Dimension);
                    ConnectFace(connector);
                }

                if (adjacentFace.VerticesBeyond.Count < oldFace.VerticesBeyond.Count)
                {
                    FindBeyondVertices(newFace, adjacentFace.VerticesBeyond, oldFace.VerticesBeyond);
                }
                else
                {
                    FindBeyondVertices(newFace, oldFace.VerticesBeyond, adjacentFace.VerticesBeyond);
                }

                if (newFace.VerticesBeyond.Count == 0)
                {
                    ConvexFaces.Add(newFace);
                    UnprocessedFaces.Remove(newFace);
                    ObjectManager.DepositVertexBuffer(newFace.VerticesBeyond);
                    newFace.VerticesBeyond = EmptyBuffer;
                }
                else
                {
                    UnprocessedFaces.Add(newFace);
                }

                ObjectManager.DepositDeferredFace(face);
            }

            for (int fIndex = 0; fIndex < AffectedFaceBuffer.Count; fIndex++)
            {
                var face = AffectedFaceBuffer[fIndex];
                UnprocessedFaces.Remove(face);
                ObjectManager.DepositFace(face);                
            }
        }
        
        void IsBeyond(ConvexFaceInternal face, VertexBuffer beyondVertices, VertexWrap v)
        {
            double distance = MathHelper.GetVertexDistance(v, face);
            if (distance >= PlaneDistanceTolerance)
            {
                if (distance > MaxDistance)
                {
                    MaxDistance = distance;
                    FurthestVertex = v;
                }
                beyondVertices.Add(v);
            }
        }

        void FindBeyondVertices(ConvexFaceInternal face)
        {
            var beyondVertices = face.VerticesBeyond;

            MaxDistance = double.NegativeInfinity;
            FurthestVertex = null;

            int count = InputVertices.Count;
            for (int i = 0; i < count; i++) IsBeyond(face, beyondVertices, InputVertices[i]);

            face.FurthestVertex = FurthestVertex;
        }

        void FindBeyondVertices(ConvexFaceInternal face, VertexBuffer beyond, VertexBuffer beyond1)
        {
            var beyondVertices = BeyondBuffer;

            MaxDistance = double.NegativeInfinity;
            FurthestVertex = null;
            VertexWrap v;

            int count = beyond1.Count;
            for (int i = 0; i < count; i++) beyond1[i].Marked = true;
            CurrentVertex.Marked = false;
            count = beyond.Count;
            for (int i = 0; i < count; i++)
            {
                v = beyond[i];
                if (object.ReferenceEquals(v, CurrentVertex)) continue;
                v.Marked = false;
                IsBeyond(face, beyondVertices, v);
            }

            count = beyond1.Count;
            for (int i = 0; i < count; i++)
            {
                v = beyond1[i];
                if (v.Marked) IsBeyond(face, beyondVertices, v);
            }

            face.FurthestVertex = FurthestVertex;
           
            var temp = face.VerticesBeyond;
            face.VerticesBeyond = beyondVertices;
            if (temp.Count > 0) temp.Clear();
            BeyondBuffer = temp;
        }
                
        void UpdateCenter()
        {
            var count = ConvexHull.Count + 1;
            for (int i = 0; i < Dimension; i++) Center[i] *= (count - 1);
            double f = 1.0 / count;
            for (int i = 0; i < Dimension; i++) Center[i] = f * (Center[i] + CurrentVertex.PositionData[i]);
        }

        void RollbackCenter()
        {
            var count = ConvexHull.Count + 1;
            for (int i = 0; i < Dimension; i++) Center[i] *= count;
            double f = 1.0 / (count - 1);
            for (int i = 0; i < Dimension; i++) Center[i] = f * (Center[i] - CurrentVertex.PositionData[i]);
        }

        void InitConvexHull()
        {
            var extremes = FindExtremes();
            var initialPoints = FindInitialPoints(extremes);

            foreach (var vertex in initialPoints)
            {
                CurrentVertex = vertex;
                UpdateCenter();
                ConvexHull.Add(CurrentVertex);
                InputVertices.Remove(vertex);

                extremes.Remove(vertex);
            }

            var faces = InitiateFaceDatabase();

            foreach (var face in faces)
            {
                FindBeyondVertices(face);
                if (face.VerticesBeyond.Count == 0) ConvexFaces.Add(face); 
                else UnprocessedFaces.Add(face);
            }
        }

        private List<VertexWrap> FindInitialPoints(List<VertexWrap> extremes)
        {
            List<VertexWrap> initialPoints = new List<VertexWrap>();

            VertexWrap first = null, second = null;
            double maxDist = 0;
            double[] temp = new double[Dimension];
            for (int i = 0; i < extremes.Count - 1; i++)
            {
                var a = extremes[i];
                for (int j = i + 1; j < extremes.Count; j++)
                {
                    var b = extremes[j];
                    MathHelper.SubtractFast(a.PositionData, b.PositionData, temp);
                    var dist = MathHelper.LengthSquared(temp);
                    if (dist > maxDist)
                    {
                        first = a;
                        second = b;
                        maxDist = dist;
                    }
                }
            }

            initialPoints.Add(first);
            initialPoints.Add(second);

            for (int i = 2; i <= Dimension; i++)
            {
                double maximum = 0.000001;
                VertexWrap maxPoint = null;
                for (int j = 0; j < extremes.Count; j++)
                {
                    var extreme = extremes[j];
                    if (initialPoints.Contains(extreme)) continue;

                    var val = GetSquaredDistanceSum(extreme, initialPoints);

                    if (val > maximum)
                    {
                        maximum = val;
                        maxPoint = extreme;
                    }
                }
                if (maxPoint != null) initialPoints.Add(maxPoint);
                else
                {
                    int vCount = InputVertices.Count;
                    for (int j = 0; j < vCount; j++)
                    {
                        var point = InputVertices[j];
                        if (initialPoints.Contains(point)) continue;

                        var val = GetSquaredDistanceSum(point, initialPoints);

                        if (val > maximum)
                        {
                            maximum = val;
                            maxPoint = point;
                        }
                    }

                    if (maxPoint != null) initialPoints.Add(maxPoint);
                    else ThrowSingular();
                }
            }
            return initialPoints;
        }

        double GetSquaredDistanceSum(VertexWrap pivot, List<VertexWrap> initialPoints)
        {
            var initPtsNum = initialPoints.Count;
            var sum = 0.0;
        
            for (int i = 0; i < initPtsNum; i++)
            {
                var initPt = initialPoints[i];
                for (int j = 0; j < Dimension; j++)
                {
                    double t = (initPt.PositionData[j] - pivot.PositionData[j]);
                    sum += t * t;
                }
            }

            return sum;
        }

        private List<VertexWrap> FindExtremes()
        {
            var extremes = new List<VertexWrap>(2 * Dimension);

            int vCount = InputVertices.Count;
            for (int i = 0; i < Dimension; i++)
            {
                double min = double.MaxValue, max = double.MinValue;
                int minInd = 0, maxInd = 0;
                for (int j = 0; j < vCount; j++)
                {
                    var v = InputVertices[j].PositionData[i];
                    if (v < min)
                    {
                        min = v;
                        minInd = j;
                    }
                    if (v > max)
                    {
                        max = v;
                        maxInd = j;
                    }
                }

                if (minInd != maxInd)
                {
                    extremes.Add(InputVertices[minInd]);
                    extremes.Add(InputVertices[maxInd]);
                }
                else extremes.Add(InputVertices[minInd]);
            }
            return extremes;
        }

        void ThrowSingular()
        {
            throw new InvalidOperationException(
                    "ConvexHull: Singular input data (i.e. trying to triangulate a data that contain a regular lattice of points).\n"
                    + "Introducing some noise to the data might resolve the issue.");
        }

        void HandleSingular()
        {
            RollbackCenter();
            SingularVertices.Add(CurrentVertex);

            for (int fIndex = 0; fIndex < AffectedFaceBuffer.Count; fIndex++)
            {
                var face = AffectedFaceBuffer[fIndex];
                var vb = face.VerticesBeyond;
                for (int i = 0; i < vb.Count; i++)
                {
                    SingularVertices.Add(vb[i]);
                }

                ConvexFaces.Add(face);
                UnprocessedFaces.Remove(face);
                ObjectManager.DepositVertexBuffer(face.VerticesBeyond);
                face.VerticesBeyond = EmptyBuffer;
            }
        }

        void FindConvexHull()
        {
            InitConvexHull();

            while (UnprocessedFaces.First != null)
            {
                var currentFace = UnprocessedFaces.First;
                CurrentVertex = currentFace.FurthestVertex;
                                                
                UpdateCenter();

                TagAffectedFaces(currentFace);

                if (!SingularVertices.Contains(CurrentVertex) && CreateCone()) CommitCone();
                else HandleSingular();

                int count = AffectedFaceBuffer.Count;
                for (int i = 0; i < count; i++) AffectedFaceBuffer[i].Tag = 0;
            }
        }

        private ConvexHullInternal(IEnumerable<IVertex> vertices)
        {
            InputVertices = new List<VertexWrap>(vertices.Select((v, i) => new VertexWrap { Vertex = v, PositionData = v.Position, Index = i }));
            Dimension = DetermineDimension();
            Initialize();
        }

        private IEnumerable<TVertex> GetConvexHullInternal<TVertex>(bool onlyCompute = false) where TVertex : IVertex
        {
            if (Computed) return onlyCompute ? null : ConvexHull.Select(v => (TVertex)v.Vertex).ToArray();

            if (Dimension < 2) throw new ArgumentException("Dimension of the input must be 2 or greater.");

            FindConvexHull();
            Computed = true;
            return onlyCompute ? null : ConvexHull.Select(v => (TVertex)v.Vertex).ToArray();
        }

        private IEnumerable<TFace> GetConvexFacesInternal<TVertex, TFace>()
            where TFace : ConvexFace<TVertex, TFace>, new()
            where TVertex : IVertex
        {
            if (!Computed) GetConvexHullInternal<TVertex>(true);

            var faces = ConvexFaces;
            int cellCount = faces.Count;
            var cells = new TFace[cellCount];

            for (int i = 0; i < cellCount; i++)
            {
                var face = faces[i];
                var vertices = new TVertex[Dimension];
                for (int j = 0; j < Dimension; j++) vertices[j] = (TVertex)face.Vertices[j].Vertex;
                cells[i] = new TFace
                {
                    Vertices = vertices,
                    Adjacency = new TFace[Dimension],
                    Normal = face.Normal
                };
                face.Tag = i;
            }

            for (int i = 0; i < cellCount; i++)
            {
                var face = faces[i];
                var cell = cells[i];
                for (int j = 0; j < Dimension; j++)
                {
                    if (face.AdjacentFaces[j] == null) continue;
                    cell.Adjacency[j] = cells[face.AdjacentFaces[j].Tag];
                }

                if (face.IsNormalFlipped)
                {
                    var tempVert = cell.Vertices[0];
                    cell.Vertices[0] = cell.Vertices[Dimension - 1];
                    cell.Vertices[Dimension - 1] = tempVert;

                    var tempAdj = cell.Adjacency[0];
                    cell.Adjacency[0] = cell.Adjacency[Dimension - 1];
                    cell.Adjacency[Dimension - 1] = tempAdj;
                }
            }
            
            return cells;
        }

        internal static List<ConvexFaceInternal> GetConvexFacesInternal<TVertex, TFace>(IEnumerable<TVertex> data)
            where TFace : ConvexFace<TVertex, TFace>, new()
            where TVertex : IVertex
        {
            ConvexHullInternal ch = new ConvexHullInternal(data.Cast<IVertex>());
            ch.GetConvexHullInternal<TVertex>(true);
            return ch.ConvexFaces;
        }

        internal static void GetConvexHullAndFaces<TVertex, TFace>(IEnumerable<IVertex> data, out IEnumerable<TVertex> points, out IEnumerable<TFace> faces)
            where TFace : ConvexFace<TVertex, TFace>, new()
            where TVertex : IVertex
        {
            ConvexHullInternal ch = new ConvexHullInternal(data);
			points = ch.GetConvexHullInternal<TVertex>();
			faces = ch.GetConvexFacesInternal<TVertex, TFace>();
        }
    }
}














