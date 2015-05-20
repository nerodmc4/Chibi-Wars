using UnityEngine;
using System.Collections;

public class RotatetoMouse : MonoBehaviour {

	public PlayerController player;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 0;
		
		Vector3 objectPos = Camera.main.WorldToScreenPoint (player.transform.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;
		
		float angle = Mathf.Atan2 (mousePos.y, mousePos.x) * Mathf.Rad2Deg;


		transform.localScale = transform.localScale;

		if(angle < 90.0f && angle > -90.0f) 
		{
			//Debug.Log("direita");
			player.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			this.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));


		} 
		else
		{ 
			//Debug.Log("esquerda");
			player.gameObject.transform.localScale = new Vector3(-0.2f, 0.2f, 0.2f);
			this.gameObject.transform.localScale = new Vector3(-1.5f, -1.5f, 1.5f);
			transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -angle));


		}

	}
}
