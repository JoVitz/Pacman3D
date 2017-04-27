using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AI : MonoBehaviour {

    public Transform target;

    public List<TileManager.Tile> tiles = new List<TileManager.Tile>();
    public TileManager manager;
    public GhostMove ghost;

    public TileManager.Tile nextTile = null;
    public TileManager.Tile targetTile;
    public TileManager.Tile currentTile;

    void Awake()
    {
        if(SceneManager.GetActiveScene().name.Contains("2"))
        {
            tiles = manager.tiles2;

        }
        else if (SceneManager.GetActiveScene().name.Contains("3"))
        {
            tiles = manager.tiles3; 
        }
        else
        {
            tiles = manager.tiles; 
        }


        if (ghost == null) Debug.Log("game object ghost not found");
        if (manager == null) Debug.Log("game object game manager not found");
    }

    public void AILogic()
    {
        // get current tile
        Vector2 currentPos = new Vector2(transform.position.x + 0.499f, transform.position.y + 0.499f);
        currentTile = tiles[manager.Index((int)currentPos.x, (int)currentPos.y)];

        targetTile = GetTargetTilePerGhost();

        // get the next tile according to direction
        if (ghost.direction.x > 0) nextTile = tiles[manager.Index((int)(currentPos.x + 1), (int)currentPos.y)];
        if (ghost.direction.x < 0) nextTile = tiles[manager.Index((int)(currentPos.x - 1), (int)currentPos.y)];
        if (ghost.direction.y > 0) nextTile = tiles[manager.Index((int)currentPos.x, (int)(currentPos.y + 1))];
        if (ghost.direction.y < 0) nextTile = tiles[manager.Index((int)currentPos.x, (int)(currentPos.y - 1))];

        if (nextTile.occupied || currentTile.isIntersection)
        {
            //---------------------
            // IF WE BUMP INTO WALL
            if (nextTile.occupied && !currentTile.isIntersection)
            {
                // if ghost moves to right or left and there is wall next tile
                if (ghost.direction.x != 0)
                {
                    if (currentTile.down == null) ghost.direction = Vector2.up;
                    else ghost.direction = Vector2.down;

                }

                // if ghost moves to up or down and there is wall next tile
                else if (ghost.direction.y != 0)
                {
                    if (currentTile.left == null) ghost.direction = Vector2.right;
                    else ghost.direction = Vector2.left;

                }

            }

            //---------------------------------------------------------------------------------------
            // IF WE ARE AT INTERSECTION
            // calculate the distance to target from each available tile and choose the shortest one
            if (currentTile.isIntersection)
            {

                float dist1, dist2, dist3, dist4;
                dist1 = dist2 = dist3 = dist4 = 999999f;
                if (currentTile.up != null && !currentTile.up.occupied && !(ghost.direction.y < 0)) dist1 = manager.distance(currentTile.up, targetTile);
                if (currentTile.down != null && !currentTile.down.occupied && !(ghost.direction.y > 0)) dist2 = manager.distance(currentTile.down, targetTile);
                if (currentTile.left != null && !currentTile.left.occupied && !(ghost.direction.x > 0)) dist3 = manager.distance(currentTile.left, targetTile);
                if (currentTile.right != null && !currentTile.right.occupied && !(ghost.direction.x < 0)) dist4 = manager.distance(currentTile.right, targetTile);

                float min = Mathf.Min(dist1, dist2, dist3, dist4);
                if (min == dist1) ghost.direction = Vector2.up;
                if (min == dist2) ghost.direction = Vector2.down;
                if (min == dist3) ghost.direction = Vector2.left;
                if (min == dist4) ghost.direction = Vector2.right;

            }

        }

        // if there is no decision to be made, designate next waypoint for the ghost
        else
        {
            ghost.direction = ghost.direction;  // setter updates the waypoint
        }
    }

    public void RunLogic()
    {
        // get current tile //TODO check values
        Vector2 currentPos = new Vector2(transform.position.x + 0.499f, transform.position.y + 0.499f);
        currentTile = tiles[manager.Index((int)currentPos.x, (int)currentPos.y)];

        // get the next tile according to direction
        if (ghost.direction.x > 0) nextTile = tiles[manager.Index((int)(currentPos.x + 1), (int)currentPos.y)];
        if (ghost.direction.x < 0) nextTile = tiles[manager.Index((int)(currentPos.x - 1), (int)currentPos.y)];
        if (ghost.direction.y > 0) nextTile = tiles[manager.Index((int)currentPos.x, (int)(currentPos.y + 1))];
        if (ghost.direction.y < 0) nextTile = tiles[manager.Index((int)currentPos.x, (int)(currentPos.y - 1))];

        //Debug.Log (ghost.direction.x + " " + ghost.direction.y);
        //Debug.Log (ghost.name + ": Next Tile (" + nextTile.x + ", " + nextTile.y + ")" );

        if (nextTile.occupied || currentTile.isIntersection)
        {
            //---------------------
            // IF WE BUMP INTO WALL
            if (nextTile.occupied && !currentTile.isIntersection)
            {
                // if ghost moves to right or left and there is wall next tile
                if (ghost.direction.x != 0)
                {
                    if (currentTile.down == null) ghost.direction = Vector2.up;
                    else ghost.direction = Vector2.down;

                }

                // if ghost moves to up or down and there is wall next tile
                else if (ghost.direction.y != 0)
                {
                    if (currentTile.left == null) ghost.direction = Vector2.right;
                    else ghost.direction = Vector2.left;

                }

            }

            //---------------------------------------------------------------------------------------
            // IF WE ARE AT INTERSECTION
            if (currentTile.isIntersection)
            {
                List<TileManager.Tile> availableTiles = new List<TileManager.Tile>();
                TileManager.Tile chosenTile;
                if (currentTile.up != null && !currentTile.up.occupied && !(ghost.direction.y < 0)) availableTiles.Add(currentTile.up);
                if (currentTile.down != null && !currentTile.down.occupied && !(ghost.direction.y > 0)) availableTiles.Add(currentTile.down);
                if (currentTile.left != null && !currentTile.left.occupied && !(ghost.direction.x > 0)) availableTiles.Add(currentTile.left);
                if (currentTile.right != null && !currentTile.right.occupied && !(ghost.direction.x < 0)) availableTiles.Add(currentTile.right);

                int rand = Random.Range(0, availableTiles.Count);
                chosenTile = availableTiles[rand];
                ghost.direction = new Vector2(chosenTile.x - currentTile.x, chosenTile.y - currentTile.y).normalized;
				//Vector2.Normalize(new Vector2(chosenTile.x - currentTile.x, chosenTile.y - currentTile.y));
                //Debug.Log (ghost.name + ": Chosen Tile (" + chosenTile.x + ", " + chosenTile.y + ")" );
            }

        }

        // else, designate next waypoint for the ghost
        else
        {
            ghost.direction = ghost.direction; 
        }
    }


    TileManager.Tile GetTargetTilePerGhost()
    {
        Vector2 targetPos;
        TileManager.Tile targetTile;
        Vector2 dir;

        // get the target tile position
        switch (name)
        {
            case "blinky":  // target = pacman
                targetPos = new Vector2(target.position.x + 0.499f, target.position.y + 0.499f);
                targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
                break;
            case "pinky":   // target = pacman + 2*pacman direction
                dir = target.GetComponent<PacmanMove>().getDir();
                targetPos = new Vector2(target.position.x + 0.499f, target.position.y + 0.499f) + 2 * dir;
                targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
                break;
            case "inky":    // target = ambushVector(pacman+2 - blinky) added to pacman+2
                dir = target.GetComponent<PacmanMove>().getDir();
                Vector2 blinkyPos = GameObject.Find("blinky").transform.position;
                Vector2 ambushVector = new Vector2();
                ambushVector.x = target.position.x + 1 * dir.x - blinkyPos.x;
                ambushVector.y = target.position.y + 1 * dir.y - blinkyPos.y;
                targetPos = new Vector2(target.position.x + 0.499f, target.position.y + 0.499f) + 2 * dir + ambushVector;
                targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
                break;
			case "clyde":
				targetPos = new Vector2 (target.position.x + 0.499f, target.position.y + 0.499f);
				targetTile = tiles [manager.Index ((int)targetPos.x, (int)targetPos.y)];
				if (manager.distance (targetTile, currentTile) < 4) {
                    targetTile = tiles[manager.Index(2, 2)];

                }
                break;
			case "woody1":
//				if (manager.distance (targetTile, currentTile) < 4) 
//				{
					targetPos = new Vector2 (target.position.x + 0.499f, target.position.y + 0.499f);
					targetTile = tiles [manager.Index ((int)targetPos.x, (int)targetPos.y)];
//				}
				break;
			case "woody2":
//			if (manager.distance (targetTile, currentTile) < 4) 
//			{
				targetPos = new Vector2 (target.position.x + 0.499f, target.position.y + 0.499f);
				targetTile = tiles [manager.Index ((int)targetPos.x, (int)targetPos.y)];
//			}
				break;
            default:
                targetTile = null;
                Debug.Log("No target tile");
                break;

        }
        return targetTile;
    }
}
