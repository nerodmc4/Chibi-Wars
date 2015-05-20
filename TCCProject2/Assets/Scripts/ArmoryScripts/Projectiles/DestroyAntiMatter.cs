using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DestroyAntiMatter : MonoBehaviour 
{
	public int kickCount = 3;
	public float destroyTime;
	public GameObject Explosion;
	Vector2 ExplosionPoint;
	public float explosionRadius;


	void Start () 
	{
		Destroy (this.gameObject, destroyTime);
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere (ExplosionPoint, explosionRadius);
	}
	
	void OnCollisionEnter2D(Collision2D col)
	{
		kickCount -= 1;

		if (kickCount <= 0) 
		{
			int layermask = 1 << 8;
			Collider2D[] colls = Physics2D.OverlapCircleAll (ExplosionPoint, explosionRadius, layermask);
			
			foreach (Collider2D coll in colls) {
				//Debug.Log (coll.gameObject.name);
				coll.gameObject.GetComponent<ShrinkAndDestroy> ().Shrink ();
				Destroy (this.gameObject);
			}
			if (col.gameObject.tag == "Player") {
				
			} 
			else {
				OnExplode ();
				Destroy (this.gameObject);
			}
		}
		
	}
	void OnDestroy()
	{
		OnExplode ();
	}
	
	void OnExplode()
	{
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		Instantiate(Explosion, gameObject.transform.position, randomRotation);
		
	}

	void Update () 
	{ 	
		if (kickCount <= 0)
		{
			kickCount = 3;
		}
		ExplosionPoint = gameObject.transform.position;	
	}
}