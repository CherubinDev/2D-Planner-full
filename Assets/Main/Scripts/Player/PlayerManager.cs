using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : MonoBehaviour
{
    private Grid worldGrid;
    private Vector2Int cellSize;
    private Vector3 spriteCompensation;

    void Start()
    {
        worldGrid = FindObjectOfType<Grid>();
        cellSize = Vector2Int.FloorToInt(worldGrid.cellSize);
        spriteCompensation = worldGrid.cellSize / 2.0f;

        StartCoroutine(GetPlayerInformation());
    }

    [Serializable]
    public class Player
    {
        public string username;
        public Attribute[] attributes;
    }

    [Serializable]
    public class Attribute
    {
        public string name;
        public string value;
    }

    private IEnumerator GetPlayerInformation()
    {
        string username = PlayerPrefs.GetString("username", GameManager.DEFAULT_USERNAME);
        string url = string.Format("http://localhost:8080/player/{0}", username);
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            // Show results as text
            string json = request.downloadHandler.text;
            Debug.Log(json);
            Player player = JsonUtility.FromJson<Player>(json);

            foreach (Attribute attr in player.attributes)
            {
                PlayerPrefs.SetString(attr.name, attr.value);
                // Look for the location attribute
                if (attr.name.Equals("location"))
                {
                    // Pull the location
                    string[] locs = attr.value.Split(',');
                    if (locs.Length == 2)
                    {
                        int xLoc = Int32.Parse(locs[0]);
                        int yLoc = Int32.Parse(locs[1]);
                        PlayerPrefs.SetInt(attr.name + ".x", xLoc);
                        PlayerPrefs.SetInt(attr.name + ".y", yLoc);

                        transform.localPosition = 
                            worldGrid.CellToLocal(new Vector3Int(xLoc, yLoc, 0)) + spriteCompensation;
                    }
                    break;
                }
            }
            PlayerPrefs.Save();
            Debug.Log(player);
        }
    }
}
