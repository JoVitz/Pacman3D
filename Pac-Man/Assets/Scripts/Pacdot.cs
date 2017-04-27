using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacdot : MonoBehaviour {
    public GameManager gm;
	
	void OnTriggerEnter2D(Collider2D co) {
        if (co.name == "pacman")
        {
            gm.score++;
            Debug.Log("score " + gm.score);
            Destroy(gameObject);
        }
        
	}
}
