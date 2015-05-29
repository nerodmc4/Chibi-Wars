using UnityEngine;
using System.Collections;

public class Gun03 : MonoBehaviour {
	
	public CannonScript cannonType;
	public InstantFireScript instantType;
	
	// Use this for initialization
	void Start () {
	}
	
	public void setCannonType()
	{
		
		instantType.enabled = false;
		cannonType.enabled = true;
		
	}
	
	public void setInstantType()
	{
		instantType.enabled = true;
		cannonType.enabled = false;
	}
	
}
