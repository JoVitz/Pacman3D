using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScript : MonoBehaviour {
    public string scene;
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("load");
        if (other.name == "pacman")
        {
            Application.LoadLevel(scene);
        }
    }
}
