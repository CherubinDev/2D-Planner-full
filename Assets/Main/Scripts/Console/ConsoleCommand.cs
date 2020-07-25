using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class Command
{
	private static readonly Command instance = new Command();

	public delegate void LogMessage(string log);
	public event LogMessage logMessage;

	private PathPlanner planner;

	// Explicit static constructor to tell C# compiler
	// not to mark type as beforefieldinit
	static Command()
	{
	}

	private Command()
	{
	}

	public static Command Instance
	{
		get
		{
			return instance;
		}
	}

	internal void initialize(PathPlanner planner)
	{
		this.planner = planner;
	}

	#region Command handlers
	//Implement new commands in this region of the file.

	public void echo(string[] args)
	{
		string message = String.Join<string>(" ", args);
		logMessage?.Invoke("echo " + message);
	}

	public void reload(string[] args)
	{
		string sceneName = SceneManager.GetActiveScene().name;
		logMessage?.Invoke("Reloading scene: " + sceneName);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void resetPrefs(string[] args)
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}

	public void move(string[] args)
	{
		// 0: Name
		// 1: X
		// 2: Y
		// 3: Optional, appendPlan
		if (args.Length < 3 || args.Length > 4)
		{
			logMessage?.Invoke("invalid parameter");
			return;
		}
		Transform transform = GameManager.Instance.lookUpGameObject(args[0]);
		if (transform == null)
		{
			logMessage?.Invoke("game object couldn't be found");
			return;
		}
		int xLoc = Int32.Parse(args[1]);
		int yLoc = Int32.Parse(args[2]);
		bool appendPlan = false;
		if (args.Length == 4)
		{
			appendPlan = Boolean.Parse(args[3]);
		}
		PathPlanner planner = transform.GetComponent<PathPlanner>();
		if (planner == null)
		{
			logMessage?.Invoke("planner script not available on game object");
			return;
		}
		planner.planPath(new Vector3Int(xLoc, yLoc, 1), appendPlan);
	}

	public void create(string[] args)
	{
		// 0: Prefab
		// 1: Name
		// 2: X Location
		// 3: Y Location
		if (args.Length != 4)
		{
			logMessage?.Invoke("invalid parameters");
			return;
		}
		GameObject loadedPrefab = (GameObject)Resources.Load("Prefabs/" + args[0], typeof(GameObject));
		if (loadedPrefab == null)
		{
			logMessage?.Invoke("unable to find prefab");
			return;
		}
		string name = args[1];
		int index = 1;
		while (GameObject.Find("/" + name) != null)
		{
			name = String.Format("{0}-{1}", args[1], index++);
		}
		int xLoc = Int32.Parse(args[2]);
		int yLoc = Int32.Parse(args[3]);
		// Set new location
		GameObject newObj = GameObject.Instantiate<GameObject>(loadedPrefab);
		newObj.name = name;
		BaseMovement movement = newObj.GetComponent<BaseMovement>();
		movement.Start();
		movement.SetWorldPosition(new Vector3Int(xLoc, yLoc, 1));
	}

	public void destroy(string[] args)
	{
		if (args.Length != 1)
		{
			logMessage?.Invoke("invalid parameters");
			return;
		}
		GameObject obj = GameObject.Find("/" + args[0]);
		if (obj != null)
		{
			GameObject.Destroy(obj);
		}
	}

	public void script(string[] args)
	{
		TextAsset obj = Resources.Load<TextAsset>(args[0]);
		string[] lines = obj.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		Debug.Log(obj);
	}

	#endregion
}
