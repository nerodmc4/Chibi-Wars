using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MIConvexHull;

public class Voronoi : MonoBehaviour 
{
	
	public int NumberOfVertices = 1000;
	public float size = 150.0f;
	
	Material lineMaterial;
	Mesh mesh;
	
	List<Vertex2> vertices;
	VoronoiMesh<Vertex2, Cell2, VoronoiEdge<Vertex2, Cell2>> voronoiMesh;
	
	bool drawVoronoi = true;
	
	void CreateLineMaterial() 
	{
		if( !lineMaterial ) 
		{
			lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
			                            "SubShader { Pass { " +
			                            "    Blend SrcAlpha OneMinusSrcAlpha " +
			                            "    ZWrite Off Cull Off Fog { Mode Off } " +
			                            "    BindChannels {" +
			                            "      Bind \"vertex\", vertex Bind \"color\", color }" +
			                            "} } }" );
			
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	void Start () 
	{
		CreateLineMaterial();
		
		mesh = new Mesh();
		Vertex2[] vertices = new Vertex2[NumberOfVertices];
		Vector3[] meshVerts = new Vector3[NumberOfVertices];
		int[] indices = new int[NumberOfVertices];
		
		Random.seed = 0;
		for (var i = 0; i < NumberOfVertices; i++)
		{
			vertices[i] = new Vertex2(size * Random.Range(-1.0f, 1.0f), size * Random.Range(-1.0f, 1.0f));
			meshVerts[i] = vertices[i].ToVector3();
			indices[i] = i;
		}
		
		mesh.vertices = meshVerts;
		mesh.SetIndices(indices, MeshTopology.Points, 0);
		Transform parent = new GameObject("asafdsa").transform;
		foreach(Vertex2 v in vertices){
			GameObject g = new GameObject(v.ToString());
			g.transform.parent = parent;
			g.transform.position = new Vector3((float)v.x, (float)v.y, -2f);
			g.AddComponent<CircleCollider2D>().isTrigger = true;
			g.tag = "voronoi";
		}
		float now = Time.realtimeSinceStartup;
		voronoiMesh = VoronoiMesh.Create<Vertex2, Cell2>(vertices);
		float interval = Time.realtimeSinceStartup - now;
		Debug.Log("time = " + interval * 1000.0f);
		
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F1)) drawVoronoi = !drawVoronoi;
		
		Graphics.DrawMesh(mesh, Matrix4x4.identity, lineMaterial, 0, Camera.main);
	}
	
	void OnPostRender() 
	{
		GL.PushMatrix();
		
		GL.LoadIdentity();
		GL.MultMatrix(camera.worldToCameraMatrix);
		GL.LoadProjectionMatrix(camera.projectionMatrix);
		
		lineMaterial.SetPass( 0 );
		GL.Begin( GL.LINES );
		GL.Color( Color.red );
		
		if(drawVoronoi)
		{
			foreach(var edge in voronoiMesh.Edges)
			{
				bool draw = true;
				
				GL.Vertex3( edge.Source.Circumcenter.x, edge.Source.Circumcenter.y, 0.0f);
				GL.Vertex3( edge.Target.Circumcenter.x, edge.Target.Circumcenter.y, 0.0f);
			}
		}

		GL.End();
		GL.PopMatrix();
	}
}