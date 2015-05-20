using UnityEngine;
using System.Collections;

public class DestroyGrappler : MonoBehaviour {
		
		public float ropeHitTime = 1.0f;
		public bool  isRopeHitted = false;
		
		// Use this for initialization
		void Start () {
			
		}
		
		void Awake()
		{
			ropeHitTime = 0.5f;
		}

		void OnCollisionEnter2D(Collision2D col)
		{
			isRopeHitted = true;
		}

		// Update is called once per frame
		void Update () {
			if (!isRopeHitted) 
			{
				ropeHitTime -=Time.deltaTime;
			}

			if(ropeHitTime <= 0 )
			{
				Destroy(this.gameObject);
			}
		}
	}
