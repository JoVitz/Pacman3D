using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pallet : MonoBehaviour {

    private GameManager gm;

    // Use this for initialization
    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (gm == null) Debug.Log("Energizer did not find Game Manager!");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col);
        if (col.name == "pacman")
        {
            gm.ScareGhosts();
            Destroy(gameObject);
        }
    }
}



