using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


	public float speed = 10.0f;
	private float realSpeed;
	public float balanceForce;
	public Vector2 jumpVector;
	public bool isOnGround;
	public bool isOnRapel = false;
	public Transform Grounder;
	public float radiuss;
	public LayerMask ground;
	public GameObject rope;

	public Animator anim;

	void Start () 
	{
		anim = GetComponent<Animator> ();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere (Grounder.transform.position, radiuss);
	}

	void OndTriggerEnter2D(Collider2D other)
	{

	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "ground") 
		{
			isOnGround = true;
		} 
	}

	// Update is called once per frame
	void FixedUpdate () {

		realSpeed = isOnGround ? speed : speed * 0.8f;
		isOnGround = false;
		if (Input.GetKey(KeyCode.A) && isOnRapel == false) 
		{
			rigidbody2D.velocity = new Vector2 (-realSpeed, rigidbody2D.velocity.y);
			//transform.localScale = new Vector3(-0.4f, 0.4f, 0.4f);
		} 
		else if (Input.GetKey(KeyCode.D) && isOnRapel == false) 
		{
			rigidbody2D.velocity = new Vector2 (realSpeed, rigidbody2D.velocity.y);
			//transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

		}
		else if(isOnGround)
		{
			rigidbody2D.velocity = new Vector2 (0, rigidbody2D.velocity.y);
		}

		isOnGround = Physics2D.OverlapCircle(Grounder.transform.position, radiuss, ground) ;
		
		if ((Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.W)) && isOnGround == true && isOnRapel == false) 
		{
			rigidbody2D.AddForce(jumpVector);
		}

		if (isOnRapel == true && Input.GetKey(KeyCode.D)) 
		{
			this.rigidbody2D.AddForce(new Vector2(balanceForce, 0.0f));

		}
		else if (isOnRapel == true  && Input.GetKey(KeyCode.A)) 
		{
			this.rigidbody2D.AddForce(new Vector2(-balanceForce, 0.0f));
			
		}

	}
}
