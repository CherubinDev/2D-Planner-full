using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenInfo : MonoBehaviour
{
    private const int spriteSheetSize = 32;

    private Vector3 stageDimensions;

    private float leftBound, rightBound, upperBound, lowerBound;

    // Start is called before the first frame update
    void Start()
    {
        stageDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        leftBound = -stageDimensions.x;
        rightBound = stageDimensions.x;
        upperBound = stageDimensions.y;
        lowerBound = -stageDimensions.y;
    }

    void Update() 
    {
        drawOutOfBounds();
    }

    private void drawOutOfBounds()
    {
        Debug.DrawLine(new Vector3(leftBound, upperBound), new Vector3(rightBound, upperBound), Color.red);
        Debug.DrawLine(new Vector3(leftBound, lowerBound), new Vector3(rightBound, lowerBound), Color.red);
        Debug.DrawLine(new Vector3(leftBound, upperBound), new Vector3(leftBound, lowerBound), Color.red);
        Debug.DrawLine(new Vector3(rightBound, upperBound), new Vector3(rightBound, lowerBound), Color.red);
    }

    public Vector2 GetStageDimensions()
    {
        return stageDimensions;
    }

    public float GetRandomBoundedY()
    {
        return UnityEngine.Random.Range(-stageDimensions.y, stageDimensions.y);
    }

    public float GetMinX()
    {
        return -stageDimensions.x;
    }

    public float GetMinY()
    {
        return -stageDimensions.y;
    }

    public float GetMaxX()
    {
        return stageDimensions.x;
    }

    public float GetMaxY()
    {
        return stageDimensions.y;
    }

    public float GetOutOfBoundMinX()
    {
        return leftBound;
    }

    public float GetDistributionForX(int n)
    {
        if (n == 0)
        {
            return 0.0f;
        }
        return (stageDimensions.x * 2.0f) / (n * 1.0f);
    }

    public bool IsOutOfBound(Vector2 location)
    {
        return IsOutOfBoundX(location) || IsOutOfBoundY(location);
    }

    public bool IsOutOfBoundX(Vector2 location)
    {
        if (location.x <= rightBound &&
            location.x >= leftBound)
        {
            return false;
        }
        return true;
    }

    public bool IsOutOfBoundY(Vector2 location)
    {
        if (location.y <= upperBound &&
            location.y >= lowerBound)
        {
            return false;
        }
        return true;
    }
}
