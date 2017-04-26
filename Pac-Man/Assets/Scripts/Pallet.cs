using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pallet : MonoBehaviour {

    public GameManager gm;

    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.name == "pacman")
        {
            gm.ScareGhosts();
            Destroy(gameObject);
        }
    }
}



