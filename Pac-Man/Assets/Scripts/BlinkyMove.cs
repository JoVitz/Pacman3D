using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	public float speed = 0.3f;
		
	void FixedUpdate () {

/*		// Animation
		Vector2 dir = waypoints[cur].position - transform.position;
		GetComponent<Animator>().SetFloat("DirX", dir.x);
		GetComponent<Animator>().SetFloat("DirY", dir.y);
		*/
	}

	void OnTriggerEnter2D(Collider2D co) {
		if (co.name == "pacman")
			Destroy(co.gameObject);
	}
}
