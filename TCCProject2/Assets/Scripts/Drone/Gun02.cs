using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun02 : MonoBehaviour {
	
	float timer;
	public PlayerController player;
	
	public Transform Shooter;
	public PlayerController playerVoronoi;
	
	public bool isClickedUI = false;
	public List<GameObject> projecGun;
	public List<Sprite> spritesGun;
	private int currentGun;
	
	public bool canShoot;
	public float shootVelocity;
	public float ShootCDTime = 0.5f;
	
	public bool HoldToFire = false;
	
	void Start ()
	{
		currentGun = 0;
	}
	
	
	public void clickUI(bool shoot)
	{
		isClickedUI = shoot;
	}
	public void setGun(int gunNum)
	{
		currentGun = gunNum;
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = spritesGun [currentGun];
	}
	
	void OnDrawGizmos()
	{	
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (Shooter.transform.position, 0.3f);
	}
	
	public void Shoot()
	{
		GameObject projectile = (GameObject)Instantiate(projecGun[currentGun], Shooter.transform.position, Quaternion.identity);
		
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 0;
		
		Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;
		mousePos.Normalize();
		if (currentGun == 1 || currentGun == 2) {
			projectile.rigidbody2D.velocity = new Vector2 (shootVelocity * mousePos.x, shootVelocity * mousePos.y + Random.Range (-10.5f, 10.5f));
		} else if (currentGun == 0 || currentGun == 3 || currentGun == 4) {
			projectile.rigidbody2D.velocity = new Vector2 (shootVelocity * mousePos.x, shootVelocity * mousePos.y + Random.Range (-3.5f, 3.5f));
		} else 
		{
			projectile.rigidbody2D.velocity = new Vector2 (shootVelocity * mousePos.x, shootVelocity * mousePos.y);
		}
		canShoot = false;
		
	}
	
	void Update () 
	{
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 0;
		
		Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;
		mousePos.Normalize();
		
		float angle = Mathf.Atan2 (mousePos.y, mousePos.x) * Mathf.Rad2Deg;
		
		if(angle < 90.0f && angle > -90.0f) 
		{
			//Debug.Log("direita");
			player.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		} 
		else
		{ 
			//Debug.Log("esquerda");
			player.gameObject.transform.localScale = new Vector3(-0.6f, 0.6f, 0.6f);
		} 
		
		switch (currentGun) 
		{
			//pistol
		case 0:
			shootVelocity = 80;
			ShootCDTime = 0.5f;
			if(angle < 90.0f && angle > -90.0f) 
			{
				this.gameObject.transform.localScale = new Vector3(0.3f, 0.3f ,0.3f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
			} else
			{
				this.gameObject.transform.localScale = new Vector3(-0.3f, -0.3f, 0.3f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -angle));
			}
			break;
			//UZI
		case 1:
			shootVelocity = 80;
			ShootCDTime = 0.25f;
			
			if(angle < 90.0f && angle > -90.0f) 
			{
				this.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.45f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
			} else
			{
				this.gameObject.transform.localScale = new Vector3(-0.5f, -0.5f, 0.45f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -angle));
			}
			break;
			//Giratoria
		case 2:
			shootVelocity = 100;
			ShootCDTime = 0.07f;
			
			if(angle < 90.0f && angle > -90.0f) 
			{
				this.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.45f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
			} else
			{
				this.gameObject.transform.localScale = new Vector3(-0.5f, -0.5f, 0.45f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -angle));
			}
			break;
			//Grappler
		case 3:
			ShootCDTime = 0.5f;
			shootVelocity = 100;
			if(angle < 90.0f && angle > -90.0f) 
			{
				this.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.45f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
			} else
			{
				this.gameObject.transform.localScale = new Vector3(-0.5f, -0.5f, 0.45f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -angle));
			}
			break;
		case 4:
			ShootCDTime = 0.01f;
			shootVelocity = 100;
			if(angle < 90.0f && angle > -90.0f) 
			{
				this.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.45f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
			} else
			{
				this.gameObject.transform.localScale = new Vector3(-0.5f, -0.5f, 0.45f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -angle));
			}
			break;
		case 5:
			ShootCDTime = 0.1f;
			shootVelocity = 20;
			if(angle < 90.0f && angle > -90.0f) 
			{
				this.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.45f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
			} else
			{
				this.gameObject.transform.localScale = new Vector3(-0.5f, -0.5f, 0.45f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -angle));
			}
			break;
		default:
			
			break;
			
		}  
		
		if (!canShoot) 
		{
			timer -= Time.deltaTime;
		}
		
		if (timer <= 0) 
		{
			canShoot = true;
			timer = ShootCDTime;
		}
		
		if (currentGun == 1 || currentGun == 2 || currentGun == 5) {
			HoldToFire = true;
		} else 
		{
			HoldToFire = false;
		}
		
		if (canShoot && isClickedUI && Input.GetKeyDown (KeyCode.Mouse0)) {
			
		} else if (Input.GetKeyDown (KeyCode.Mouse0) && canShoot && !HoldToFire) {
			Shoot ();
		} 
		else if (Input.GetKey (KeyCode.Mouse0) && canShoot && HoldToFire) 
		{
			Shoot ();
		}
	}
}
