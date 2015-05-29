using UnityEngine;

namespace MIConvexHull
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class VoronoiMesh
    {
        public static VoronoiMesh<TVertex, TCell, TEdge> Create<TVertex, TCell, TEdge>(IEnumerable<TVertex> data)
            where TCell : TriangulationCell<TVertex, TCell>, new()
            where TVertex : IVertex
            where TEdge : VoronoiEdge<TVertex, TCell>, new()
        {
            return VoronoiMesh<TVertex, TCell, TEdge>.Create(data);
        }

        public static VoronoiMesh<TVertex, DefaultTriangulationCell<TVertex>, VoronoiEdge<TVertex, DefaultTriangulationCell<TVertex>>> Create<TVertex>(IEnumerable<TVertex> data)
            where TVertex : IVertex
        {
            return VoronoiMesh<TVertex, DefaultTriangulationCell<TVertex>, VoronoiEdge<TVertex, DefaultTriangulationCell<TVertex>>>.Create(data);
        }

        public static VoronoiMesh<DefaultVertex, DefaultTriangulationCell<DefaultVertex>, VoronoiEdge<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>> 
            Create(IEnumerable<double[]> data)
        {
            var points = data.Select(p => new DefaultVertex { Position = p.ToArray() });
            return VoronoiMesh<DefaultVertex, DefaultTriangulationCell<DefaultVertex>, VoronoiEdge<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>>.Create(points);
        }

        public static VoronoiMesh<TVertex, TCell, VoronoiEdge<TVertex, TCell>> Create<TVertex, TCell>(IEnumerable<TVertex> data)
            where TVertex : IVertex
            where TCell : TriangulationCell<TVertex, TCell>, new()
        {
            return VoronoiMesh<TVertex, TCell, VoronoiEdge<TVertex, TCell>>.Create(data);
        }
    }

    public class VoronoiMesh<TVertex, TCell, TEdge>
        where TCell : TriangulationCell<TVertex, TCell>, new()
        where TVertex : IVertex
        where TEdge : VoronoiEdge<TVertex, TCell>, new()
    {
        class EdgeComparer : IEqualityComparer<TEdge>
        {
            public bool Equals(TEdge x, TEdge y)
            {
                return (x.Source == y.Source && x.Target == y.Target) || (x.Source == y.Target && x.Target == y.Source);
            }

            public int GetHashCode(TEdge obj)
            {
                return obj.Source.GetHashCode() ^ obj.Target.GetHashCode();
            }
        }

        public IEnumerable<TCell> Vertices { get; private set; }

        public IEnumerable<TEdge> Edges { get; private set; }

        public static VoronoiMesh<TVertex, TCell, TEdge> Create(IEnumerable<TVertex> data)
        {
            if (data == null) throw new ArgumentNullException("data can't be null");

            if (!(data is IList<TVertex>)) data = data.ToArray();

            var t = DelaunayTriangulation<TVertex, TCell>.Create(data);
            var vertices = t.Cells;
            var edges = new HashSet<TEdge>(new EdgeComparer());

            foreach (var f in vertices)
            {
                for (int i = 0; i < f.Adjacency.Length; i++)
                {
                    var af = f.Adjacency[i];
                    if (af != null) edges.Add(new TEdge { Source = f, Target = af });
                }
            }

            return new VoronoiMesh<TVertex, TCell, TEdge>
            {
                Vertices = vertices,
                Edges = edges.ToList()
            };
        }
		private VoronoiMesh()
        {

        }
    }
}












