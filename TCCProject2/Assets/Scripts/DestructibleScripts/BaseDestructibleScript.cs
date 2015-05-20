using UnityEngine;
using System.Collections.Generic;

public class BaseDestructibleScript : MonoBehaviour 
{
	public List<string> tags;
	public PhysicsMaterial2D chunkPhysicsMaterial;
	
	public void onCollisionEnter(GameObject collisionObject)
	{
		Debug.Log("QUEBRA A PORRA TODA!!!!!");
		//ShrinkAndDestroyScript shrinkAndDestroy = gameObject.AddComponent<ShrinkAndDestroyScript>();
		Vector2 velocity = collisionObject.rigidbody2D.velocity;
		gameObject.layer = LayerMask.NameToLayer("Default");
		transform.parent = null;
		rigidbody2D.isKinematic = false;
		rigidbody2D.AddForce(velocity * 100f + Random.insideUnitCircle * 50f);
		rigidbody2D.AddTorque(Random.Range(-1f, 1f) * 50f);
		//shrinkAndDestroy.time = 0.5f;
	}
	
	virtual public void copyFrom(BaseDestructibleScript source)
	{
		tags = source.tags;
		chunkPhysicsMaterial = source.chunkPhysicsMaterial;
		
		collider2D.sharedMaterial = chunkPhysicsMaterial;
	}
}