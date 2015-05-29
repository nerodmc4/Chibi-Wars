namespace MIConvexHull
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ConvexHull
    {
        public static ConvexHull<TVertex, TFace> Create<TVertex, TFace>(IEnumerable<TVertex> data)
            where TVertex : IVertex
            where TFace : ConvexFace<TVertex, TFace>, new()
        {
            return ConvexHull<TVertex, TFace>.Create(data);
        }

        public static ConvexHull<TVertex, DefaultConvexFace<TVertex>> Create<TVertex>(IEnumerable<TVertex> data)
            where TVertex : IVertex
        {
            return ConvexHull<TVertex, DefaultConvexFace<TVertex>>.Create(data);
        }

        public static ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>> Create(IEnumerable<double[]> data)
        {
            var points = data.Select(p => new DefaultVertex { Position = p.ToArray() });
            return ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>>.Create(points);
        }
    }

    public class ConvexHull<TVertex, TFace>
        where TVertex : IVertex
        where TFace : ConvexFace<TVertex, TFace>, new(){
        public IEnumerable<TVertex> Points { get; private set; }
		public IEnumerable<TFace> Faces { get; private set; }
		public static ConvexHull<TVertex, TFace> Create(IEnumerable<TVertex> data){
            if (!(data is IList<TVertex>)) data = data.ToArray();

			IEnumerable<TVertex> points;
			IEnumerable<TFace> faces;

            ConvexHullInternal.GetConvexHullAndFaces<TVertex, TFace>(data.Cast<IVertex>(), out points, out faces);
            return new ConvexHull<TVertex, TFace> { Points = points, Faces = faces };
        }

        private ConvexHull(){

        }
    }
}






















