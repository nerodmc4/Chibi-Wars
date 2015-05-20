using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Destroy : MonoBehaviour 
{

	public bool byContact;
	public float destroyTime;
	public GameObject Explosion;
	Vector2 ExplosionPoint;
	public float explosionRadius;
	public Text countText;

	Text cText;

	// Use this for initialization
	void Start () {
		Destroy (this.gameObject, destroyTime);
		cText = (Text)Text.Instantiate (countText);
		cText.transform.parent = GameObject.Find ("Canvas").transform;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere (ExplosionPoint, explosionRadius);
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (byContact) {
			if (col.gameObject.layer == 8) {
				int layermask = 1 << 8;
				Collider2D[] colls = Physics2D.OverlapCircleAll (ExplosionPoint, explosionRadius, layermask);


				foreach (Collider2D coll in colls) {
					//Debug.Log (coll.gameObject.name);
					coll.gameObject.GetComponent<ShrinkAndDestroy> ().Shrink ();
					Destroy (this.gameObject);
				}
			}
			if (col.gameObject.tag == "Player") {

			} else {
				OnExplode ();
				Destroy (this.gameObject);
			}
		}

	}

	void OnExplode()
	{
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		Instantiate(Explosion, gameObject.transform.position, randomRotation);
	}

	void OnDestroy()
	{
		OnExplode ();
		Destroy (cText.gameObject);
		
	}

	void Update () 
	{
		cText.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y +1, this.transform.position.z);
		cText.transform.localScale = new Vector3 (1, 1, 1);
		destroyTime -= Time.deltaTime;
		cText.text = Mathf.Ceil (destroyTime) + "";
		ExplosionPoint = gameObject.transform.position;
		
	}
}
