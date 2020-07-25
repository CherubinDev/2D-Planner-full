using Planner;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PathPlanner))]
public class PlayerMovement : BaseMovement
{
    private GameManager gameManager;

    private PathPlanner pathPlanner;

    new void Start()
    {
        base.Start();
        gameManager = GameManager.Instance;

        pathPlanner = GetComponent<PathPlanner>();
    }

    new void Update()
    {
        base.Update();
        checkInput();
    }

    private void checkInput()
    {
        if (!gameManager.takePlayerInput())
        {
            return;
        }
        if (!isMoving())
        {
            checkKeyboardInput();
        }
        checkMouseInput();
    }

    private void checkMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            planNewPath(getCurrentMouseWorldCellLocation(), true);
        }
        checkForReplan();
    }

    private void checkKeyboardInput()
    {
        Direction? direction = null;
        if ((Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow)) ||
            (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow)))
        {
            return;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            direction = Direction.RIGHT;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction = Direction.LEFT;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            direction = Direction.UP;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            direction = Direction.DOWN;
        }

        // Check for non-null
        if (direction != null)
        {
            Move(direction.Value);
        }
    }

    private Vector3Int? replanDestination = null;
    private void planNewPath(Vector3Int worldCellDestination, bool appendToPlan)
    {
        if (replanDestination != null)
        {
            return;
        }
        if (!appendToPlan && isMoving())
        {
            replanDestination = worldCellDestination;
        } else
        {
            pathPlanner.planPath(worldCellDestination, appendToPlan);
        }
    }

    private void checkForReplan()
    {
        if (replanDestination != null && !isMoving())
        {
            // Replan
            pathPlanner.planPath(replanDestination.Value, false);
            replanDestination = null;
        }
    }

    private Vector3Int getCurrentMouseWorldCellLocation()
    {
        // Get the mouse position
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 0.5f;
        Vector3 mousePointWorld = Camera.main.ScreenToWorldPoint(mousePoint);
        // Calculate mouse position in the world grid
        return worldGrid.WorldToCell(mousePointWorld);
    }

    /*private IEnumerator SaveLocation()
    {
        string username = PlayerPrefs.GetString("username", GameManager.DEFAULT_USERNAME);
        Vector3Int location = worldGrid.LocalToCell(endLocation);
        string url = String.Format("http://localhost:8080/player/{0}/location/{1},{2}", username, location.x, location.y);
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("SaveLocation failure: " + request.error);
        }
        Debug.Log("SaveLocation success: " + request.responseCode + "\nResponse: " + request.downloadHandler.text);
    }*/
}
