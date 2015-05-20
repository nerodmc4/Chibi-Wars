using UnityEngine;
using System.Collections;

public class CarryEgg : MonoBehaviour {

	public bool isCarrying = false;
	public Vector3 offset = new Vector3(0, 4f, 0);
	GameObject egg;
	// Use this for initialization
	void Start () 
	{
		
	}

	public void Grab()
	{
		isCarrying = true;

	}

	public void Drop()
	{
		isCarrying = false;
	}

	public void DropBasket()
	{
		isCarrying = false;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "egg" && isCarrying == false) 
		{
			egg = col.gameObject;
			egg.gameObject.collider2D.enabled = false;
			Grab();
		}
		if (col.gameObject.tag == "basket" && isCarrying == true) 
		{
			egg.transform.position = col.gameObject.transform.position + new Vector3(Random.Range(-1.0F, 1.0F), Random.Range(-0.0F, 0.6F), -0.5f);
			DropBasket();
		}
	}


	// Update is called once per frame
	void Update ()
	{
	
		if (isCarrying) 
		{
			egg.gameObject.transform.position = this.transform.position + offset;
		}
	}
}
