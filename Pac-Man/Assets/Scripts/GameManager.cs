using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //--------------------------------------------------------
    // Game variables

    public static int Level = 0;

    public GameObject pacman;
    public GameObject blinky;
    public GameObject pinky;
    public GameObject inky;
    public GameObject clyde;

    public static bool scared;

    public float scareLength;
    private float _timeToCalm;


    //-------------------------------------------------------------------
    // function definitions


    void Start()
    {
    }



    // Update is called once per frame
    void Update()
    {
        if (scared && _timeToCalm <= Time.time)
            CalmGhosts();

    }

    public void ToggleScare()
    {
        if (!scared) ScareGhosts();
        else CalmGhosts();
    }

    public void ScareGhosts()
    {
        Debug.Log("start scare");
        scared = true;
        blinky.GetComponent<GhostMove>().Frighten();
        pinky.GetComponent<GhostMove>().Frighten();
        inky.GetComponent<GhostMove>().Frighten();
        clyde.GetComponent<GhostMove>().Frighten();
        _timeToCalm = Time.time + scareLength;

        Debug.Log("Ghosts Scared");
    }

    public void CalmGhosts()
    {
        scared = false;
        blinky.GetComponent<GhostMove>().Calm();
        pinky.GetComponent<GhostMove>().Calm();
        inky.GetComponent<GhostMove>().Calm();
        clyde.GetComponent<GhostMove>().Calm();
    }



}
