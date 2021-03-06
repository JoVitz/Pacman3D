﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanMove : MonoBehaviour {
	public float speed = 0.4f;
	Vector2 dest = Vector2.zero;
    Vector2 radius = new Vector2(0.5f, 0.5f);
    float tileSize = 1f;
    public Vector2 dir;

    // Use this for initialization
    void Start () {
		dest = transform.position;		
	}
    /*void Update()
    {
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + move);
        //transform.position += move * speed * Time.deltaTime;
    }*/
    // Update is called once per frame
    void FixedUpdate () {
		// Move closer to Destination
		Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
		GetComponent<Rigidbody2D>().MovePosition(p);

        // Check for Input if not moving
        if ((Vector2)transform.position == dest) {
			if (Input.GetKey(KeyCode.UpArrow) && valid(Vector2.up))
				dest = (Vector2)transform.position + Vector2.up*tileSize;
			if (Input.GetKey(KeyCode.RightArrow) && valid(Vector2.right))
				dest = (Vector2)transform.position + Vector2.right*tileSize;
			if (Input.GetKey(KeyCode.DownArrow) && valid(-Vector2.up))
				dest = (Vector2)transform.position - Vector2.up*tileSize;
			if (Input.GetKey(KeyCode.LeftArrow) && valid(-Vector2.right))
				dest = (Vector2)transform.position - Vector2.right*tileSize;
		}

		// Animation Parameters
		dir = dest - (Vector2)transform.position;
		GetComponent<Animator>().SetFloat("DirX", dir.x);
		GetComponent<Animator>().SetFloat("DirY", dir.y);
	}

	bool valid(Vector2 dir) {
		// Cast Line from 'next to Pac-Man' to 'Pac-Man'
		Vector2 pos = transform.position;
		RaycastHit2D hit = Physics2D.Linecast(pos + dir , pos, 1 << LayerMask.NameToLayer("Default"));
		return (hit.collider == GetComponent<Collider2D>());
	}
    public Vector2 getDir()
    {
        return dir;
    }
}
