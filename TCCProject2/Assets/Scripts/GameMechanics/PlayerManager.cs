using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	public int health = 100;
	public float repeatDamagePeriod = 1f;	
	public float hurtForce = 10f;				
	public int damageMultiplier = 1;	
	private float lastHitTime;					// The time at which the player was last hit.
	private Vector3 healthScale;			

	private bool dead;
	private bool isWithFlag;

	private SpriteRenderer healthBar;			// Reference to the sprite renderer of the health bar.

	// Use this for initialization
	void Start () 
	{
		healthBar = GameObject.Find("HealthBar").GetComponent<SpriteRenderer>();
		healthScale = healthBar.transform.localScale;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Projectile") 
		{
			TakeDamage(10, col.gameObject.transform);
		}
	}
	public void UpdateHealthBar ()
	{
		// Set the health bar's colour to proportion of the way between green and red based on the player's health.
		healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - health * 0.01f);
		
		// Set the scale of the health bar to be proportional to the player's health.
		healthBar.transform.localScale = new Vector3(healthScale.x * health * 0.01f, 1, 1);
	}

	void TakeDamage (int damage, Transform posTiro)
	{
		// Make sure the player can't jump.
		// Create a vector that's from the enemy to the player with an upwards boost.
		Vector3 hurtVector = transform.position - posTiro.position + Vector3.up * 5f;
		
		// Add a force to the player in the direction of the vector and multiply by the hurtForce.
		rigidbody2D.AddForce(hurtVector * hurtForce);
		
		// Reduce the player's health by 10.
		health -= damage*damageMultiplier;
		
		// Update what the health bar looks like.
		UpdateHealthBar();
		
	}

	void SendDeathEvent ()
	{
	}
	
	void death ()
	{
	}	
	void respawn()
	{
	}
	// Update is called once per frame
	void Update () {
	
	}
}
