using UnityEngine;
using System.Collections;

public class CamController : MonoBehaviour {
	
	public Transform target;
	private Vector3 velocity = Vector3.zero;
	
	public float smoothTime = 0.15f;
	
	public bool verticalMaxEnabled = false;
	public float verticalMax = 0f;
	public bool verticalMinEnabled = false;
	public float verticalMin = 0f;
	
	public bool horizontalMaxEnabled = false;
	public float horizontalMax = 0f;
	public bool horizontalMinEnabled = false;
	public float horizontalMin = 0f;
	
	void LateUpdate () {
			
		if (target) {
			Vector3 targetPosition = target.position;
			
			if (verticalMinEnabled && verticalMaxEnabled) {
				targetPosition.y = Mathf.Clamp(target.position.y, verticalMin, verticalMax);
			} else if (verticalMinEnabled) {
				targetPosition.y = Mathf.Clamp(target.position.y, verticalMin, target.position.y);
			} else if (verticalMaxEnabled) {
				targetPosition.y = Mathf.Clamp(target.position.y, target.position.y, verticalMax);
			}
			
			if (horizontalMinEnabled && horizontalMaxEnabled) {
				targetPosition.x = Mathf.Clamp(target.position.x, horizontalMin, horizontalMax);
			} else if (horizontalMinEnabled) {
				targetPosition.x = Mathf.Clamp(target.position.x, horizontalMin, target.position.x);
			} else if (horizontalMaxEnabled) {
				targetPosition.x = Mathf.Clamp(target.position.x, target.position.x, horizontalMax);
			}
			
			targetPosition.z = transform.position.z;
			
			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

		}
	}
}