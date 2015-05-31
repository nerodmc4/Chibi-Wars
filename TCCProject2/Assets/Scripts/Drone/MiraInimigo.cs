using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiraInimigo : MonoBehaviour {

	float timer;
	public PlayerController player;
	
	public Transform Shooter;
	
	public bool isClickedUI = false;
	public List<GameObject> projecGun;
	public List<Sprite> spritesGun;
	private int currentGun;
	
	public bool canShoot;
	public float shootVelocity;
	public float ShootCDTime = 0.5f;
	
	public bool HoldToFire = false;
	public PlayerController playerVoronoi;
	
	void Start ()
	{
		currentGun = 3;
	}

	public void clickUI(bool shoot)
	{
		isClickedUI = shoot;
	}

	void OnDrawGizmos()
	{	
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (Shooter.transform.position, 0.3f);
	}

	void Update () {
		Shooter.position = playerVoronoi.posicao - Shooter.position;
	}
}
