using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Poly2Tri;

/// <summary>
/// This thing can be better, but it'll do for now. It takes a list of points, and creates a 2D mesh describing it.
/// </summary>
public static class Ferr2D_Triangulator
{
    #region Public Methods
    /// <summary>
    /// Creates a triangulation of the vertices given, and gives you the indices of it.
    /// </summary>
    /// <param name="aPoints">A list of points to triangulate.</param>
    /// <param name="aTreatAsPath">Should we discard any triangles at all? Use this if you want to get rid of triangles that are outside the path.</param>
    /// <param name="aInvert">if we're treating it as a path, should we instead sicard triangles inside the path?</param>
    /// <returns>A magical list of indices describing the triangulation!</returns>

	public static void GetVerticesAndIndices(List<Vector2> inputPoints, out List<Vector2> outputPoints, out List<int> outputIndices, float chunkWidth, float chunkHeight, float jitterX, float jitterY)
	{
		Polygon poly;
		List<PolygonPoint> points = new List<PolygonPoint>();
		List<Vector2> interpolatedPoints = new List<Vector2>();
		float edgeCutLength = (chunkWidth + chunkHeight) * 0.5f;
		int triangleIndex = 0;
		int numChunksX;
		int numChunksY;
		
		outputPoints = new List<Vector2>();
		outputIndices = new List<int>();
		
		// Cut all the edges into smaller segments (so triangulation won't create long, thin shards along the edges)
		for (int i = 0; i < inputPoints.Count; i++)
		{
			Vector2 a;
			Vector2 b;
			float distance;
			int numCuts;
			float cutLength;
			
			a = i == 0 ? inputPoints[inputPoints.Count - 1] : inputPoints[i - 1];
			b = inputPoints[i];
			
			distance=  (b - a).magnitude;
			numCuts = (int)(distance / edgeCutLength);
			cutLength = distance / (float)numCuts;
			
			for (int j = 0; j < numCuts; j++)
			{
				Vector2 p = Vector2.Lerp(a, b, Mathf.Max(0f, Mathf.Min(1f, (j * cutLength / distance))));
				
				interpolatedPoints.Add(p);
			}
		}
		
		// Convert points to P2T, and create polygon
		foreach (Vector2 p in interpolatedPoints)
		{
			points.Add(new PolygonPoint(p.x, p.y));
		}
		poly = new Polygon(points);
		
		// Calculate number of chunks to split the polygon up into
		numChunksX = (int)(poly.Bounds.Width / chunkWidth);
		numChunksY = (int)(poly.Bounds.Height / chunkHeight);
		
		// Add steiner points (this is the reason for using Poly2Tri)
		UnityEngine.Random.seed = (int)(poly.Bounds.Left * poly.Bounds.Top);
		for (int i = 0; i < numChunksX; i++)
		{
			for (int j = 0; j < numChunksY; j++)
			{
				TriangulationPoint p = new TriangulationPoint(
					i * chunkWidth + poly.Bounds.Left + UnityEngine.Random.Range(-jitterX, jitterX),
					j * -chunkHeight + poly.Bounds.Top + UnityEngine.Random.Range(-jitterY, jitterY));
				
				if (poly.IsPointInside(p))
				{
					poly.AddSteinerPoint(p);
				}
			}
		}
		
		// Triangulate
		P2T.Triangulate(poly);
		
		// Build output from triangulated polygon
		foreach (DelaunayTriangle triangle in poly.Triangles)
		{
			TriangulationPoint p1 = triangle.Points[0];
			TriangulationPoint p2 = triangle.PointCWFrom(p1);
			TriangulationPoint p3 = triangle.PointCWFrom(p2);
			
			outputPoints.Add(new Vector2(p1.Xf, p1.Yf));
			outputPoints.Add(new Vector2(p2.Xf, p2.Yf));
			outputPoints.Add(new Vector2(p3.Xf, p3.Yf));
			outputIndices.Add(triangleIndex++);
			outputIndices.Add(triangleIndex++);
			outputIndices.Add(triangleIndex++);
		}
	}

	public  static List<int> GetIndices           (ref List<Vector2> aPoints, bool aTreatAsPath, bool aInvert) {
        
		Vector4            bounds = GetBounds(aPoints);
        LibTessDotNet.Tess tess   = new LibTessDotNet.Tess();

        LibTessDotNet.ContourVertex[] verts = new LibTessDotNet.ContourVertex[aPoints.Count];
        for (int i = 0; i < aPoints.Count; i++) {
            verts[i] = new LibTessDotNet.ContourVertex();
            verts[i].Position = new LibTessDotNet.Vec3();
            verts[i].Position.X = aPoints[i].x;
            verts[i].Position.Y = aPoints[i].y;
            verts[i].Position.Z = 0;
        }
        tess.AddContour(verts, LibTessDotNet.ContourOrientation.Original);
        if (aInvert) {
            aPoints.Add(new Vector2(bounds.x - (bounds.z - bounds.x) * 1, bounds.w - (bounds.y - bounds.w) * 1)); // 4
            aPoints.Add(new Vector2(bounds.z + (bounds.z - bounds.x) * 1, bounds.w - (bounds.y - bounds.w) * 1)); // 3
            aPoints.Add(new Vector2(bounds.z + (bounds.z - bounds.x) * 1, bounds.y + (bounds.y - bounds.w) * 1)); // 2
            aPoints.Add(new Vector2(bounds.x - (bounds.z - bounds.x) * 1, bounds.y + (bounds.y - bounds.w) * 1)); // 1
            LibTessDotNet.ContourVertex[] outer = new LibTessDotNet.ContourVertex[4];
            for (int i = 0; i < 4; i++) {
                outer[i] = new LibTessDotNet.ContourVertex();
                outer[i].Position = new LibTessDotNet.Vec3();
                outer[i].Position.X = aPoints[(aPoints.Count - 4) + i].x;
                outer[i].Position.Y = aPoints[(aPoints.Count - 4) + i].y;
                outer[i].Position.Z = 0;
            }
            tess.AddContour(outer, aInvert ? LibTessDotNet.ContourOrientation.CounterClockwise : LibTessDotNet.ContourOrientation.Clockwise);
        }
        tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3);
        List<int> result = new List<int>();
        for (int i = 0; i < tess.Elements.Length; i += 3) {
            for (int t = 0; t < 3; t++) {
                if (tess.Elements[i + t] == -1) {
                    continue;
                }
                for (int v = 0; v < aPoints.Count; v++) {
                    if (aPoints[v].x == tess.Vertices[tess.Elements[i + t]].Position.X &&
                        aPoints[v].y == tess.Vertices[tess.Elements[i + t]].Position.Y) {
                        result.Add(v);
                        break;
                    }
                }
            }
        }
        return result;
	}
	
    /// <summary>
    /// Gets a list of line segments that are under the given point. Two indices per segment.
    /// </summary>
    /// <param name="aPath">A list of path vertices.</param>
    /// <param name="aX">The point.</param>
    /// <param name="aY">The point</param>
    /// <param name="aIgnoreLast">Ignore the last 4 vertices, often useful since triangulation involves adding 4 verts to the end of the path list.</param>
    /// <returns></returns>
	public  static List<int> GetSegmentsUnder     (List<Vector2> aPath, float aX, float aY, bool aIgnoreLast) {
		List<int> result = new List<int>();
        int off = aIgnoreLast ? 4 : 0;
		for (int i=0;i<aPath.Count-off;i+=1) {
			int next = i+1 >= aPath.Count-off ? 0 : i+1;
			int min = aPath[i].x < aPath[next].x ? i : next;
			int max = aPath[i].x > aPath[next].x ? i : next;
			
			if (aPath[min].x <= aX && aPath[max].x > aX) {
				float height = Mathf.Lerp(aPath[min].y, aPath[max].y, (aX - aPath[min].x) / (aPath[max].x - aPath[min].x));
				if (aY > height) {
					result.Add(min);
					result.Add(max);
				}
			}
		}

		return result;
	}
    /// <summary>
    /// Gets a bounding rectangle based on the given points
    /// </summary>
    /// <param name="aPoints">List of points.</param>
    /// <returns>x = left, y = top, z = right, w = bottom</returns>
	public  static Vector4   GetBounds            (List<Vector2> aPoints) {
		if (aPoints.Count <=0)return new Vector4(0,0,1,1);
        float left   = aPoints[0].x;
        float right  = aPoints[0].x;
        float top    = aPoints[0].y;
        float bottom = aPoints[0].y;

		for (int i=0;i<aPoints.Count;i+=1) {
			if (aPoints[i].x < left  ) left   = aPoints[i].x;
			if (aPoints[i].x > right ) right  = aPoints[i].x;
			if (aPoints[i].y > top   ) top    = aPoints[i].y;
			if (aPoints[i].y < bottom) bottom = aPoints[i].y;
		}
		return new Vector4(left, top, right, bottom);
	}
    /// <summary>
    /// Is the given point inside a 2D triangle?
    /// </summary>
    /// <param name="aTri1">Triangle point 1</param>
    /// <param name="aTri2">Triangle point 2</param>
    /// <param name="aTri3">Triangle point 9001</param>
    /// <param name="aPt">The point to test!</param>
    /// <returns>IS IT INSIDE YET?</returns>
	public  static bool      PtInTri              (Vector2 aTri1,   Vector2 aTri2, Vector2 aTri3,   Vector2 aPt) {
        float as_x = aPt.x - aTri1.x;
        float as_y = aPt.y - aTri1.y;
        bool  s_ab = (aTri2.x - aTri1.x) * as_y - (aTri2.y - aTri1.y) * as_x > 0;

        if ((aTri3.x - aTri1.x) * as_y - (aTri3.y - aTri1.y) * as_x > 0 == s_ab) return false;
        if ((aTri3.x - aTri2.x) * (aPt.y - aTri2.y) - (aTri3.y - aTri2.y) * (aPt.x - aTri2.x) > 0 != s_ab) return false;

        return true;
	}
    /// <summary>
    /// Gets the point where two lines intersect, really useful for determining the circumcenter.
    /// </summary>
    /// <param name="aStart1">Line 1 start</param>
    /// <param name="aEnd1">Line 1 llamma</param>
    /// <param name="aStart2">Line 2 start</param>
    /// <param name="aEnd2">Line 2 end</param>
    /// <returns>WHERE THEY INTERSECT</returns>
	public  static Vector2   LineIntersectionPoint(Vector2 aStart1, Vector2 aEnd1, Vector2 aStart2, Vector2 aEnd2)
	{
		float A1 = aEnd1  .y - aStart1.y;
		float B1 = aStart1.x - aEnd1  .x;
		float C1 = A1 * aStart1.x + B1 * aStart1.y;
		
		float A2 = aEnd2  .y - aStart2.y;
		float B2 = aStart2.x - aEnd2  .x;
		float C2 = A2 * aStart2.x + B2 * aStart2.y;
		
		float delta = A1*B2 - A2*B1;
		
		return new Vector2(
		  (B2*C1 - B1*C2)/delta,
		  (A1*C2 - A2*C1)/delta
		);
	}
    /// <summary>
    /// Determines if these points are in clockwise order.
    /// </summary>
	public  static bool      IsClockwise          (Vector2 aPt1,    Vector2 aPt2,  Vector2 aPt3) {
		return (aPt2.x - aPt1.x)*(aPt3.y - aPt1.y) - (aPt3.x - aPt1.x)*(aPt2.y - aPt1.y) > 0;
	}
    #endregion

    #region Private Methods
    private static Vector2   GetCircumcenter      (List<Vector2> aPoints, List<int> aTris, int     aTri) {
		// find midpoints on two sides
		Vector2 midA = (aPoints[aTris[aTri  ]] + aPoints[aTris[aTri+1]]) / 2;
		Vector2 midB = (aPoints[aTris[aTri+1]] + aPoints[aTris[aTri+2]]) / 2;
		// get a perpendicular line for each midpoint
		Vector2 dirA = (aPoints[aTris[aTri  ]] - aPoints[aTris[aTri+1]]); dirA = new Vector2(dirA.y, -dirA.x);
		Vector2 dirB = (aPoints[aTris[aTri+1]] - aPoints[aTris[aTri+2]]); dirB = new Vector2(dirB.y, -dirB.x);
		// the intersection should give us the circumcenter
		return LineIntersectionPoint(midA, midA + dirA, midB, midB + dirB);
	}
	private static bool      EdgeFlip             (List<Vector2> aPoints, List<int> aTris, int     aTri) {
		List<int> xyz      = new List<int>(3);
		List<int> abc      = new List<int>(3);
		List<int> shared   = new List<int>(3);
		List<int> opposing = new List<int>(3);
		
		xyz.Clear ();
		xyz.Add ( aTris[aTri]   );
		xyz.Add ( aTris[aTri+1] );
		xyz.Add ( aTris[aTri+2] );
		Vector2 center = GetCircumcenter(aPoints, aTris, aTri);
		float   distSq = Vector2.SqrMagnitude(aPoints[xyz[0]] - center);
		
		for (int i = 0; i < aTris.Count; i+=3) {
			if (i == aTri) continue;
			
			shared  .Clear ();
			opposing.Clear ();
			abc     .Clear ();
			abc.Add (aTris[i]);
			abc.Add (aTris[i+1]);
			abc.Add (aTris[i+2]);
			
			for (int triID1 = 0; triID1 < 3; triID1++) {
				int count = 0;
				for (int triID2 = 0; triID2 < 3; triID2++) {
					if (xyz[triID1] == abc[triID2]) {
						shared.Add(xyz[triID1]);
						count += 1;
					}
				}
				if (count == 0) {
					opposing.Add (xyz[triID1]);
				}
			}
			if (opposing.Count == 1 && shared.Count == 2) {
				for (int triID1 = 0; triID1 < 3; triID1++) {
					if (abc[triID1] != shared[0] &&
						abc[triID1] != shared[1] &&
						abc[triID1] != opposing[0]) {
						opposing.Add (abc[triID1]);
						break;
					}
				}
			}
			
			if (opposing.Count == 2 && shared.Count == 2) {
				if(Vector2.SqrMagnitude(aPoints[opposing[1]] - center) < distSq) {
					
					aTris[aTri  ] = opposing[0];
					aTris[aTri+1] = shared  [0];
					aTris[aTri+2] = opposing[1];
					
					aTris[i  ] = opposing[1];
					aTris[i+1] = shared  [1];
					aTris[i+2] = opposing[0];
					
					//EdgeFlip(aPoints, aTris, aTri);
					//EdgeFlip(aPoints, aTris, i);
					return true;
				}
			}
		}
		return false;
	}
	private static int       GetSurroundingTri    (List<Vector2> aPoints, List<int> aTris, Vector2 aPt ) {
		for (int i=0;i<aTris.Count;i+=3) {
			if (PtInTri(aPoints[aTris[i]],
						aPoints[aTris[i+1]],
						aPoints[aTris[i+2]],
						aPt )) {
				return i;
			}
		}
		return -1;
    }
    #endregion
}