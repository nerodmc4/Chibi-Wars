namespace MIConvexHull
{
	using System.Collections.Generic;
	using System.Linq;
	
	public interface ITriangulation<TVertex, TCell>
		where TCell : TriangulationCell<TVertex, TCell>, new()
			where TVertex : IVertex
	{
		IEnumerable<TCell> Cells { get; }
	}
	
	public static class Triangulation
	{
		public static ITriangulation<TVertex, DefaultTriangulationCell<TVertex>> CreateDelaunay<TVertex>(IEnumerable<TVertex> data)
			where TVertex : IVertex
		{
			return DelaunayTriangulation<TVertex, DefaultTriangulationCell<TVertex>>.Create(data);
		}
		
		public static ITriangulation<DefaultVertex, DefaultTriangulationCell<DefaultVertex>> CreateDelaunay(IEnumerable<double[]> data)
		{
			var points = data.Select(p => new DefaultVertex { Position = p.ToArray() });
			return DelaunayTriangulation<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>.Create(points);
		}
		
		public static ITriangulation<TVertex, TFace> CreateDelaunay<TVertex, TFace>(IEnumerable<TVertex> data)
			where TVertex : IVertex
				where TFace : TriangulationCell<TVertex, TFace>, new()
		{
			return DelaunayTriangulation<TVertex, TFace>.Create(data);
		}
	}
}



















