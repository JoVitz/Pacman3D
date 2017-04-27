﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    //--------------------------------------------------------
    // Game variables


    public GameObject pacman;
    public GameObject blinky;
    public GameObject pinky;
    public GameObject inky;
    public GameObject clyde;
	public GameObject woody1;

    public GameObject wall1;
    public GameObject wall2;

    public static bool scared;

    public float scareLength;
    private float _timeToCalm;

    public int score;
    public int scoreWin;


    //-------------------------------------------------------------------
    // function definitions


    void Start()
    {
        score = 0;
        if (SceneManager.GetActiveScene().name.Contains("2"))
        {
            scoreWin = 4;

        }
        else if (SceneManager.GetActiveScene().name.Contains("3"))
        {
            scoreWin = 3;
        }
        else
        {
            scoreWin = 5;
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (scared && _timeToCalm <= Time.time)
            CalmGhosts();
        if(score == scoreWin)
        {
            Destroy(wall1);
            Destroy(wall2);
        }
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
        Debug.Log("blinky");
        pinky.GetComponent<GhostMove>().Frighten();
        Debug.Log("pinky");
        inky.GetComponent<GhostMove>().Frighten();
        Debug.Log("inky");
        clyde.GetComponent<GhostMove>().Frighten();
        Debug.Log("clyde");
        woody1.GetComponent<GhostMove>().Frighten();
        Debug.Log("woody");
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
		woody1.GetComponent<GhostMove>().Calm();
    }



}
