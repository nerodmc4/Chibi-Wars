using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun01 : MonoBehaviour 
{
	
	public Transform Shooter;
	public bool canShoot;
	public float rocketVelocityMin;
	public float rocketVelocityMax;
	private bool isShooting;
	public float holdTime;
	public float maxHoldTime;
	public float ShootCDTime;
	
	private float rocketVelocity;
	public GameObject TrajectoryPointPrefeb;
	private bool isPressed, wasShooted;
	private List<GameObject> trajectoryPoints;
	public int numOfTrajectoryPoints = 25;
	float timer;
	
	public bool isClickedUI = false;
	public List<GameObject> projecGun;
	public List<Sprite> spritesGun;
	private int currentGun;
	
	public PlayerController player;
	
	void Start ()
	{
		currentGun = 0;
		trajectoryPoints = new List<GameObject>();
		isPressed = wasShooted = false;
		for(int i = 0; i < numOfTrajectoryPoints; i++)
		{
			GameObject dot = (GameObject) Instantiate(TrajectoryPointPrefeb);
			dot.transform.position = this.transform.position;
			dot.renderer.enabled = false;
			trajectoryPoints.Insert(i, dot);
		}
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
		rocketVelocity = Mathf.Lerp(rocketVelocityMin, rocketVelocityMax, holdTime/maxHoldTime);
		projectile.rigidbody2D.velocity = new Vector2(rocketVelocity*mousePos.x, rocketVelocity*mousePos.y);
		
		canShoot = false;
		isShooting = false;
	}
	
	private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
	{
		return(new Vector2 (toPos.x, toPos.y) - new Vector2 (fromPos.x, fromPos.y)) * rocketVelocity;
	}
	
	private void setTrajectoryPoints(Vector3 pStartPosition, Vector3 pVelocity)
	{
		float velocity = Mathf.Sqrt ((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
		float angle = Mathf.Rad2Deg * (Mathf.Atan2 (pVelocity.y, pVelocity.x));
		float fTime = 0;
		
		fTime += 0.1f;
		
		for (int i =0; i < numOfTrajectoryPoints; i++) 
		{
			float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
			float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime/2.0f);
			Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 2);
			trajectoryPoints[i].transform.position = pos;
			trajectoryPoints[i].renderer.enabled = true;
			trajectoryPoints[i].transform.eulerAngles = new Vector3(0,0, Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude)* fTime,pVelocity.x)*Mathf.Rad2Deg);
			fTime+=0.1f;
			trajectoryPoints[i].GetComponent<SpriteRenderer>().color = Color.Lerp(Color.green, Color.red, holdTime/maxHoldTime);
		}
	}
	
	void FixedUpdate () 
	{
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 0;
		
		Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;
		mousePos.Normalize();
		rocketVelocity = Mathf.Lerp(rocketVelocityMin, rocketVelocityMax, holdTime/maxHoldTime);
		
		float angle = Mathf.Atan2 (mousePos.y, mousePos.x) * Mathf.Rad2Deg;
		
		if(angle < 90.0f && angle > -90.0f) 
		{
			player.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		} 
		else
		{ 
			player.gameObject.transform.localScale = new Vector3(-0.6f, 0.6f, 0.6f);
		}
		
		switch (currentGun) 
		{
			//rocket
		case 0:
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
			//rope
		case 1:
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
			//granade
		case 2:
			if(angle < 90.0f && angle > -90.0f) 
			{
				this.gameObject.transform.localScale = new Vector3(0.5f, 0.7f, 0.7f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
			} else
			{
				this.gameObject.transform.localScale = new Vector3(-0.5f, -0.7f, 0.7f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -angle));
			}
			break;
			//antiMatter
		case 3:
			if(angle < 90.0f && angle > -90.0f) 
			{	
				this.gameObject.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
			} else
			{
				this.gameObject.transform.localScale = new Vector3(-0.45f, -0.45f, 0.3f);
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -angle));
			}
			break;
		default:
			
			break;
			
		}
		
		if (wasShooted) 
		{
			return;
		}
		
		if(isShooting){
			holdTime += Time.deltaTime;
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
		
		if (!isShooting && canShoot && isClickedUI && Input.GetKeyDown(KeyCode.Mouse0)){
			isShooting = true;
			isPressed = true;
			
			holdTime = 0;
		}
		else if (Input.GetKeyUp(KeyCode.Mouse0) && canShoot && isShooting) 
		{
			for (int i = 0; i < numOfTrajectoryPoints; i++) 
			{
				trajectoryPoints[i].transform.position = new Vector3(0,0,0);
				trajectoryPoints[i].renderer.enabled = false;
			}
			isPressed = false;
			Shoot();
		}
		
		if (isPressed) 
		{	
			//float angle = Mathf.Atan2(vel.y, vel.x)*Mathf.Rad2Deg;
			//transform.eulerAngles = new Vector3(0,0, angle);
			setTrajectoryPoints(transform.position, new Vector2(rocketVelocity*mousePos.x, rocketVelocity*mousePos.y));
		}
	}
}