namespace MIConvexHull
{
    public class VoronoiEdge<TVertex, TCell>
        where TVertex : IVertex
        where TCell : TriangulationCell<TVertex, TCell>
    {
        public TCell Source
        {
            get;
            internal set;
        }

        public TCell Target
        {
            get;
            internal set;
        }

        public override bool Equals(object obj)
        {
            var other = obj as VoronoiEdge<TVertex, TCell>;
            if (other == null) return false;
            if (object.ReferenceEquals(this, other)) return true;
            return (Source == other.Source && Target == other.Target)
                || (Source == other.Target && Target == other.Source);
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + Source.GetHashCode();
            return hash * 31 + Target.GetHashCode();
        }

        public VoronoiEdge()
        {

        }

        public VoronoiEdge(TCell source, TCell target)
        {
            this.Source = source;
            this.Target = target;
        }
    }
}
