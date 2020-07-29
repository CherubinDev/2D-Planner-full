﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    private const int PIXELS_PER_CELL = 32;

    public Tile baseTile;
    public Tile flower;

    private ScreenInfo screenInfo;

    private List<float> worldSpaceX = new List<float>();
    private List<float> worldSpaceY = new List<float>();

    private Grid worldGrid;
    private Tilemap[] grids;

    internal class Cell
    {
        internal GameObject obj;
        internal float xLoc;
        internal float yLoc;
    }

    // Start is called before the first frame update
    void Start()
    {
        screenInfo = FindObjectOfType<ScreenInfo>();
        worldGrid = FindObjectOfType<Grid>();
        grids = worldGrid.GetComponentsInChildren<Tilemap>();

        StartCoroutine(GetLevel());
    }

    private IEnumerator GetLevel()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:8080/level");
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("GetLevel failure: " + request.error);
        }
        // Show results as text
        string json = request.downloadHandler.text;
        Debug.Log(json);

        Level level = JsonConvert.DeserializeObject<Level>(json);

        Tilemap baseTileMap = grids[0];
        int xDist = level.base_x / 2;
        int yDist = level.base_y / 2;
        for (int x = -xDist; x < xDist; x++)
        {
            for (int y = -yDist; y < yDist; y++)
            {
                baseTileMap.SetTile(new Vector3Int(x, y, 1), baseTile);
            }
        }

        Tilemap currentTileMap;
        for (int i = 0; i < level.layers.Length; i++)
        {
            LevelLayer levelLayer = level.layers[i];
            currentTileMap = grids[i+1];

            currentTileMap.tag = levelLayer.walkable.GetValueOrDefault(true) 
                ? "Walkable" : "Obstacle";
            
            foreach (TextureToCell textureToCell in levelLayer.cells)
            {
                switch(textureToCell.texture)
                {
                    case "flower":
                        foreach (CellInfo cellInfo in textureToCell.cells)
                        {
                            currentTileMap.SetTile(cellInfo.GetVector3Int(), flower);
                        }
                        break;
                }
            }
            currentTileMap.RefreshAllTiles();
        }
    }

    // Update is called once per frame
    void Update()
    {
        drawDebugLines();
    }

    private void drawDebugLines()
    {
        foreach (float yLoc in worldSpaceY)
        {
            Debug.DrawLine(new Vector3(screenInfo.GetMinX(), yLoc), new Vector3(screenInfo.GetMaxX(), yLoc), Color.white);
        }
        foreach (float xLoc in worldSpaceX)
        {
            Debug.DrawLine(new Vector3(xLoc, screenInfo.GetMinY()), new Vector3(xLoc, screenInfo.GetMaxY()), Color.white);
        }
    }
}