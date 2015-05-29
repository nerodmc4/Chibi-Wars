using UnityEngine;
using System.Collections;

public class DestroyBullet : MonoBehaviour {

	public float BulletDestroyTime = 1.5f;
	public GameObject Explosion;
	public int dano = 10;
	Vector2 ExplosionPoint;

	// Use this for initialization
	void Start () {
		//.tag = "BulletProjectile";
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Inimigo") {
			col.gameObject.GetComponent<PlayerManager>().TakeDamage(dano, col.gameObject.transform);
			Destroy (this.gameObject);
		}
		else 
		{
			Destroy (this.gameObject);
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
	// Update is called once per frame
	void Update () {
		BulletDestroyTime -= Time.deltaTime;
		if (BulletDestroyTime <= 0) 
		{
			Destroy(this.gameObject);
		}
	}
}
