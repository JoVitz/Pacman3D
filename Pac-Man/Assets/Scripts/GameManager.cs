using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	public GameObject woody1;
	public GameObject woody2;
	public GameObject slimy1;
	public GameObject slimy2;

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
        if (score == scoreWin)
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
        pinky.GetComponent<GhostMove>().Frighten();
        inky.GetComponent<GhostMove>().Frighten();
        clyde.GetComponent<GhostMove>().Frighten();
		woody1.GetComponent<GhostMove>().Frighten();
		woody2.GetComponent<GhostMove>().Frighten();
		slimy1.GetComponent<GhostMove>().Frighten();
		slimy2.GetComponent<GhostMove>().Frighten();
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
		woody2.GetComponent<GhostMove>().Calm();
		slimy1.GetComponent<GhostMove>().Calm();
		slimy2.GetComponent<GhostMove>().Calm();
    }



}
