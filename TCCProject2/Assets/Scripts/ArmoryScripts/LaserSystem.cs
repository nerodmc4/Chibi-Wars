using UnityEngine;
using System.Collections;

public class LaserSystem : MonoBehaviour {

	LineRenderer line;
	Transform transPlayer;
	// Use this for initialization
	void Start () {

		line = this.GetComponent<LineRenderer> ();
		transPlayer = GameObject.Find ("shooterPos").transform;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 0;
		
		Vector3 objectPos = Camera.main.WorldToScreenPoint (transPlayer.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;
		mousePos.Normalize ();

		RaycastHit2D hit = Physics2D.Raycast(transPlayer.position, mousePos);
		if (hit.collider != null) 
		{
			line.SetPosition (1, hit.point);
		} 
		else
		{
			line.SetPosition (1, transPlayer.position + mousePos * 100);
		}

		Debug.DrawRay(transPlayer.position, mousePos, Color.red);
		
		line.SetPosition (0, transPlayer.position);
	}
}
