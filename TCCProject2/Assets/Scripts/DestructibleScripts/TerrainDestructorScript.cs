using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TerrainDestructorScript : MonoBehaviour 
{
	private int _mask;
	private Collider2D[] _results;
	
	public float radius = 1f;
	
	void Start()
	{
		_results = new Collider2D[255];
		_mask = LayerMask.GetMask("DestructibleTerrain");
	}
	
	void FixedUpdate()
	{
		int numResults = Physics2D.OverlapCircleNonAlloc(transform.position, radius, _results, _mask);
		
		for (int i = 0; i < numResults; i++)
		{
			BaseDestructibleScript destructibleScript = _results[i].gameObject.GetComponent<BaseDestructibleScript>();
			
			destructibleScript.onCollisionEnter(gameObject);
		}
	}
	
	#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, radius);
	}
	#endif
}