using Planner;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(MoveAnimation))]
public class BaseMovement : MonoBehaviour
{
    protected Grid worldGrid;
 
    private Transform model;
    private Vector2Int cellSize;
    private Vector3 spriteCompensation;

    private MoveAnimation moveAnimation;
    private SpriteRenderer spriteRenderer;

    private bool moving = false;
    private Direction direction = Direction.DOWN;
    private float movingTime = 0.5f;
    private float timeSinceMove = 0.0f;

    private Vector3 startLocation;
    private Vector3 endLocation;

    public void Start()
    {
        model = transform;
        worldGrid = FindObjectOfType<Grid>();
        cellSize = Vector2Int.FloorToInt(worldGrid.cellSize);
        spriteCompensation = worldGrid.cellSize / 2.0f;

        moveAnimation = GetComponent<MoveAnimation>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        movePlayer();
    }

    public bool isMoving()
    {
        return moving;
    }

    public bool Move(Direction direction)
    {
        if (!moving)
        {
            this.direction = direction;
            moving = true;
            startLocation = model.localPosition;
            endLocation = getNextPosition(startLocation, direction);
            moveAnimation.walk(this.direction);
            return true;
        }
        return false;
    }

    public void SetWorldPosition(Vector3Int location)
    {
        this.transform.position = worldGrid.CellToLocal(location) + spriteCompensation;
    }

    private bool movePlayer()
    {
        if (moving)
        {
            spriteRenderer.sortingOrder = 1;
            timeSinceMove += Time.deltaTime;
            transform.position = Vector3.Lerp(startLocation, endLocation, timeSinceMove / movingTime);
            if (timeSinceMove >= movingTime)
            {
                moving = false;
                timeSinceMove = 0.0f;
                moveAnimation.idle(direction);
            }
            return true;
        }
        spriteRenderer.sortingOrder = 0;
        return false;
    }

    private Vector3 getNextPosition(Vector3 location, Direction direction)
    {
        Vector3Int newLocation = worldGrid.WorldToCell(location);

        // Move right
        switch (direction)
        {
            case Direction.LEFT:
                newLocation.x -= (int)cellSize.x;
                break;
            case Direction.RIGHT:
                newLocation.x += (int)cellSize.x;
                break;
            case Direction.UP:
                newLocation.y += (int)cellSize.y;
                break;
            case Direction.DOWN:
                newLocation.y -= (int)cellSize.y;
                break;
        }
        return worldGrid.CellToLocal(newLocation) + spriteCompensation;
    }
}
