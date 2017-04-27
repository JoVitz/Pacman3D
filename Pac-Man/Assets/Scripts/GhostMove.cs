using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GhostMove : MonoBehaviour
{

    // ----------------------------
    // Navigation variables
    private Vector2 waypoint;           // AI-determined waypoint
    private Queue<Vector2> waypoints;   // waypoints used on Init and Scatter states

    // direction is set from the AI component
    public Vector2 _direction;
    public Vector2 direction
    {
        get
        {
            return _direction;
        }

        set
        {
            _direction = value;
            Vector2 pos = new Vector2((int)transform.position.x, (int)transform.position.y);
            waypoint = pos + _direction;
            //Debug.Log ("waypoint (" + waypoint.position.x + ", " + waypoint.position.y + ") set! _direction: " + _direction.x + ", " + _direction.y);

        }
    }

    public float speed = 0.3f;

    // ----------------------------
    // Ghost mode variables
    public float scatterLength = 5f;
    public float waitLength = 3.0f;

    private float timeToEndScatter;
    private float timeToEndWait;

    enum State { Wait, Init, Scatter, Chase, Run };
    State state;

    private Vector2 _startPos;
    private float _timeToWhite;
    private float _timeToToggleWhite;
    private float _toggleInterval;
    private bool isWhite = false;

    // handles
    public PacmanMove pacman;
    public GameManager _gm;

    //-----------------------------------------------------------------------------------------
    // variables end, functions begin
    void Start()
    {
        _toggleInterval = _gm.scareLength * 0.33f * 0.20f;
        InitializeGhost();
    }

    public float DISTANCE;

    void FixedUpdate()
    {
        DISTANCE = Vector3.Distance(transform.position, waypoint);
            animate();

            switch (state)
            {
                case State.Wait:
                    Wait();
                    break;

                case State.Init:
                    Init();
                    break;

                case State.Scatter:
                    Scatter();
                    break;

                case State.Chase:
                    ChaseAI();
                    break;

                case State.Run:
                    RunAway();
                    break;
            }
        
    }

    //-----------------------------------------------------------------------------------
    // Start() functions

    public void InitializeGhost()
    {
        _startPos = getStartPosAccordingToName();
        waypoint = transform.position;  // to avoid flickering animation
        state = State.Wait;
        timeToEndWait = Time.time + waitLength;
        InitializeWaypoints(state);
    }

    public void InitializeGhost(Vector3 pos)
    {
        transform.position = pos;
        waypoint = transform.position;	// to avoid flickering animation
        state = State.Wait;
        timeToEndWait = Time.time + waitLength;
        InitializeWaypoints(state);
    }

    //TODO
    private void InitializeWaypoints(State st)
    {
        //--------------------------------------------------
        // File Format: Init and Scatter coordinates separated by empty line
        // Init X,Y 
        // Init X,Y
        // 
        // Scatter X,Y
        // Scatter X,Y

        //--------------------------------------------------
        // hardcode waypoints according to name.
        string data = "";
        switch (name)
        {
            case "blinky":
                data = @"11 14
11 16
13 16
13 18
15 18

15 18
15 20
18 20
18 18";
			
//				@"23.75 28.75
//				22 26
//27 26
//27 30
//22 30
//22 26";
                break;
            case "pinky":
                data = @"10 12
10 14
9 14
9 16
7 16
7 18

5 18
5 20
2 20
2 18";
			
//				@"14.5 17
//14 17
//14 20
//7 20

//7 26
//7 30
//2 30
//2 26";
                break;
            case "inky":
                data = @"11 12
10 12
10 14
13 14
13 8
15 8

15 6
15 4
18 4
18 2
11 2
11 4
13 4
13 6";
			
//			@"16.5 17
//15 17
//15 20
//22 20

//22 8
//19 8
//19 5
//16 5
//16 2
//27 2
//27 5
//22 5";
                break;
            case "clyde":
                data = @"9 12
10 12
10 14
7 14
7 8
5 8

5 6
5 4
2 4
2 2
9 2
9 4
7 4
7 6";
                break;

		case "woody1":
			data = @"2 2
2 4
2 2

18 2
18 4
17 4
17 6
2 6
2 8
5 8
5 10
9 10
9 14
11 14
11 16
18 16
18 20
11 20
11 14
13 14
13 6
10 6
10 4
8 4
8 2"; 
			break;

		case "woody2":
			data = @"2 20
2 16
2 20

9 20
9 16
15 16
15 8
18 8
18 6
17 6
17 4
18 4
18 2
12 2
12 4
10 4
10 6
8 6
8 8 
7 8
7 10
5 10
5 16
2 16
2 20"; 
			break;

        }

        //-------------------------------------------------
        // read from the hardcoded waypoints
        string line;

        waypoints = new Queue<Vector2>();
        Vector3 wp;

        if (st == State.Init)
        {
            using (StringReader reader = new StringReader(data))
            {
                // stop reading if empty line is reached
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0) break; 

                    string[] values = line.Split(' ');
                    float x = float.Parse(values[0]);
                    float y = float.Parse(values[1]);

                    wp = new Vector2(x, y);
                    waypoints.Enqueue(wp);
                    Debug.Log("waypoint init : " + wp);
                }
            }
        }

        if (st == State.Scatter)
        {
            // skip until empty line is reached, read coordinates afterwards
            bool scatterWps = false;	// Scatter waypoints

            using (StringReader reader = new StringReader(data))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0)
                    {
                        scatterWps = true; // we reached the scatter waypoints
                        continue; // do not read empty line, go to next line
                    }

                    if (scatterWps)
                    {
                        string[] values = line.Split(' ');
                        int x = Int32.Parse(values[0]);
                        int y = Int32.Parse(values[1]);

                        wp = new Vector3(x, y, 0);
                        waypoints.Enqueue(wp);
                        Debug.Log("waypoint scatter: " + wp);
                    }
                }
            }
        }

        // if in wait state, patrol vertically
        if (st == State.Wait)
        {
            Vector3 pos = transform.position;

            // inky and clyde start going down and then up
            if (transform.name == "pinky" || transform.name == "clyde")
            {
                waypoints.Enqueue(new Vector2(pos.x, pos.y - 0.1f));
                waypoints.Enqueue(new Vector2(pos.x, pos.y + 0.1f));
            }
            // while pinky start going up and then down
            else
            {
                waypoints.Enqueue(new Vector2(pos.x, pos.y + 0.1f));
                waypoints.Enqueue(new Vector2(pos.x, pos.y - 0.1f));
            }
        }

    }
 
    private Vector2 getStartPosAccordingToName()
    {
        switch (gameObject.name)
        {
            case "blinky":
                return new Vector2(10f, 14f);

            case "pinky":
                return new Vector2(10f, 12f);

            case "inky":
                return new Vector2(11f, 12f);

            case "clyde":
                return new Vector2(9f, 12f);

			case "woody1":
				return new Vector2(2f, 2f);

			case "woody2":
				return new Vector2(2f, 20f);
        }

        return new Vector2();
    }

    //------------------------------------------------------------------------------------
    // Update functions
    void animate()
    {
        Vector2 dir = new Vector2();
        dir.x = waypoint.x - transform.position.x;
        dir.y = waypoint.y - transform.position.y;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
        GetComponent<Animator>().SetBool("Run", false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "pacman")
        {
            //Destroy(other.gameObject);
            if (state == State.Run)
            {
                Calm();
                InitializeGhost(_startPos);
            }

            else
            {
//TODO gameover
            }

        }
    }

    //-----------------------------------------------------------------------------------
    // State functions
    void Wait()
    {
        if (Time.time >= timeToEndWait + 3f)
        {
            state = State.Init;
            waypoints.Clear();
            InitializeWaypoints(state);
        }

        // get the next waypoint and move towards it
        MoveToWaypoint(true);
    }

    void Init()
    {
        _timeToWhite = 0;

        // if the Queue is cleared, do some clean up and change the state
        if (waypoints.Count == 0)
        {
            state = State.Scatter;

            //get direction according to sprite name
            string name = GetComponent<SpriteRenderer>().sprite.name;
            if (name[name.Length - 1] == '0' || name[name.Length - 1] == '1') direction = Vector3.right;
            if (name[name.Length - 1] == '2' || name[name.Length - 1] == '3') direction = Vector3.left;
            if (name[name.Length - 1] == '4' || name[name.Length - 1] == '5') direction = Vector3.up;
            if (name[name.Length - 1] == '6' || name[name.Length - 1] == '7') direction = Vector3.down;

            InitializeWaypoints(state);
            timeToEndScatter = Time.time + scatterLength;

            return;
        }

        // get the next waypoint and move towards it
        MoveToWaypoint();
    }

    void Scatter()
    {
		if (Time.time >= timeToEndScatter && name != "woody1" && name !="woody2")
        {
            waypoints.Clear();
            state = State.Chase;
            return;
        }


        // get the next waypoint and move towards it
        MoveToWaypoint(true);

//		if (name == "woody1" && name =="woody2")
//			GetComponent<AI>().AILogic();

    }

    void ChaseAI()
    {

        // if not at waypoint, move towards it
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
        {
            Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }

        // if at waypoint, run AI module
       else GetComponent<AI>().AILogic();

    }

    void RunAway()
    {
        GetComponent<Animator>().SetBool("Run", true);

        if (Time.time >= _timeToWhite && Time.time >= _timeToToggleWhite) ToggleBlueWhite();

        // if not at waypoint, move towards it
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)
        {
            Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }

        // if at waypoint, run AI run away logic
        else GetComponent<AI>().RunLogic();

    }

    //------------------------------------------------------------------------------
    // Utility functions
    void MoveToWaypoint(bool loop = false)
    {
        waypoint = waypoints.Peek();        // get the waypoint (CHECK NULL?)
        Debug.Log("waypoint peek: " + waypoint);
        if (Vector3.Distance(transform.position, waypoint) > 0.000000000001)    // if its not reached
        {                                                           // move towards it
            Vector2 temp = new Vector2();
            temp.x = waypoint.x - transform.position.x;
            temp.y = waypoint.y - transform.position.y;

            _direction = temp.normalized;// Vector2.Normalize(temp);  // dont screw up waypoint by calling public setter
            Vector2 p = Vector2.MoveTowards(transform.position, waypoint, speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }
        else    // if waypoint is reached, remove it from the queue
        {
            if (loop) waypoints.Enqueue(waypoints.Dequeue());
            else waypoints.Dequeue();
        }
    }

    public void Frighten()
    {
        state = State.Run;
        _direction *= -1;
        _timeToWhite = Time.time + _gm.scareLength * 0.66f;
        _timeToToggleWhite = _timeToWhite;
        GetComponent<Animator>().SetBool("Run_White", false);

    }

    public void Calm()
    {
        // if the ghost is not running, do nothing
        if (state != State.Run) return;

        waypoints.Clear();
        state = State.Chase;
        _timeToToggleWhite = 0;
        _timeToWhite = 0;
        GetComponent<Animator>().SetBool("Run_White", false);
        GetComponent<Animator>().SetBool("Run", false);
    }

    public void ToggleBlueWhite()
    {
        isWhite = !isWhite;
        GetComponent<Animator>().SetBool("Run_White", isWhite);
        _timeToToggleWhite = Time.time + _toggleInterval;
    }

}
