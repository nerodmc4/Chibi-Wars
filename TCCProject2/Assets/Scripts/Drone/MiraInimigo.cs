using UnityEngine;
using System.Collections;

public class MiraInimigo : MonoBehaviour {

	public Transform Shooter;
	public PlayerController playerVoronoi;
	
	void Start () {
		
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
