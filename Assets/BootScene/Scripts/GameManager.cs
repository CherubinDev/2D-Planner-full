using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour {

	public static string DEFAULT_USERNAME = "cherubin";

	private enum GameState
	{
		START,
		SCRIPT_CONSOLE
	}

	private GameState state = GameState.START;
	private GameState previousState;

	public static GameManager Instance { get; private set; }

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.Log("Warning: multiple " + this + " in scene!");
		}
	}

	internal void quitGame()
	{
#if UNITY_EDITOR
		// Application.Quit() does not work in the editor so
		// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	internal void toggleConsole()
	{
		if (state != GameState.SCRIPT_CONSOLE)
		{
			previousState = state;
			state = GameState.SCRIPT_CONSOLE;
		} else
		{
			state = previousState;
		}
	}

	internal bool takePlayerInput()
	{
		return state != GameState.SCRIPT_CONSOLE;
	}

	internal Transform lookUpGameObject(string name)
	{
		GameObject obj = GameObject.Find("/" + name);
		if (obj == null)
		{
			return null;
		}
		return obj.transform;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state)
		{
			default:
				// Do Nothing
				break;
		}
	}
}
