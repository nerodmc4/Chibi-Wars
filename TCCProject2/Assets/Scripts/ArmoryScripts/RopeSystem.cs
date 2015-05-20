using UnityEngine;
using System.Collections;

public class RopeSystem : MonoBehaviour {
	DistanceJoint2D dist;
	LineRenderer lineRend;
	public float distanceBalance;
	PlayerController playerControl;

	// Use this for initialization
	void Start () 
	{	
		playerControl = GameObject.FindWithTag("Player").gameObject.GetComponent<PlayerController> ();

		if(playerControl.rope != null )
		{
			Destroy(playerControl.rope);
		}
		playerControl.rope = this.gameObject;
		lineRend = this.GetComponent<LineRenderer> ();
		dist = this.GetComponent<DistanceJoint2D> ();
		dist.connectedBody = GameObject.FindWithTag ("Player").gameObject.rigidbody2D;
		dist.distance = distanceBalance;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player") 
		{

		} 
		else 
		{
			playerControl.isOnRapel = true;
			this.gameObject.rigidbody2D.isKinematic = true;
			distanceBalance = Vector3.Distance(playerControl.gameObject.transform.position, col.contacts[0].point);
		} 

	}

	// Update is called once per frame
	void Update ()
	{
		lineRend.SetPosition (0, GameObject.FindWithTag ("Player").transform.position);
		lineRend.SetPosition (1, this.transform.position);
		
		if (Input.GetKey (KeyCode.W)) 
		{
			distanceBalance -= Time.deltaTime*10;
			dist.distance = distanceBalance;
		}
		if (Input.GetKey (KeyCode.S)&& playerControl.isOnGround == false) 
		{
			distanceBalance += Time.deltaTime*10;
			dist.distance = distanceBalance;
		}

		if(Input.GetKey (KeyCode.Space))
		{
			Destroy(playerControl.rope);
		}
	}

	void OnDestroy()
	{
		playerControl.isOnRapel = false;
	}
}
