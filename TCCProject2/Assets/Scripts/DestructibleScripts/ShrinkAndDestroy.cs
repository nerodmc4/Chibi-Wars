using UnityEngine;
using System.Collections;

public class ShrinkAndDestroy : MonoBehaviour {


	public Vector3 targetScale = new Vector3(0.3f, 0.3f, 0.3f);
	public float shrinkSpeed = 0.1f;
	public bool shrinking;
	public float Dtime = 0.05f;
	public void Shrink() {
		shrinking = true;
	}
	
	void Update() {
		if (shrinking) {
			Vector2 velocity = new Vector2(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
			this.gameObject.transform.localScale -= Vector3.one*Time.deltaTime*shrinkSpeed;
			rigidbody2D.isKinematic = false;
			transform.parent = null;
			rigidbody2D.AddForce(velocity * 0.01f + Random.insideUnitCircle * 10f);
			rigidbody2D.AddTorque(Random.Range(-1f, 1f) * 10f);
			this.gameObject.collider2D.enabled = false;
			if (this.gameObject.transform.localScale.x < targetScale.x)
			{
				shrinking = false;
				Destroy(this.gameObject, Dtime);

			}

		}
	}

	// Use this for initialization
	void Start () {
		
	}

}
