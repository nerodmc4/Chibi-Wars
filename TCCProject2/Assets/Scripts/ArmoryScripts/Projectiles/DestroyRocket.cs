using UnityEngine;
using System.Collections;

public class DestroyRocket : MonoBehaviour {

	public float destroyTime;
	public GameObject Explosion;
	Vector2 ExplosionPoint;
	public float explosionRadius;


	// Use this for initialization
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

		int layermask = 1 << 8;
		Collider2D[] colls = Physics2D.OverlapCircleAll (ExplosionPoint, explosionRadius, layermask);
		
		foreach (Collider2D coll in colls) 
		{
			//Debug.Log (coll.gameObject.name);
			coll.gameObject.GetComponent<ShrinkAndDestroy>().Shrink();
			Destroy (this.gameObject);
		}
		if (col.gameObject.tag == "Player") {
			
		} else 
		{
			OnExplode ();
			Destroy (this.gameObject);
		}

		
	}

	void OnExplode()
	{

		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		Instantiate(Explosion, gameObject.transform.position, randomRotation);

	}
	
	void OnDestroy()
	{
		//this.transform.FindChild("Trail").transform.FindChild("Smoke").parent = null;
	}
	
	void Update () 
	{
		ExplosionPoint = gameObject.transform.position;	
	}
}
