namespace MIConvexHull
{
    public abstract class TriangulationCell<TVertex, TCell> : ConvexFace<TVertex, TCell>
        where TVertex : IVertex
        where TCell : ConvexFace<TVertex, TCell>
    {

    }
	public class DefaultTriangulationCell<TVertex> : TriangulationCell<TVertex, DefaultTriangulationCell<TVertex>>
        where TVertex : IVertex
    {
		public DefaultTriangulationCell() {}
    }
}