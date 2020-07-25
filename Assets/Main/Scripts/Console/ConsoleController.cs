using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine.SceneManagement;

public delegate void CommandHandler(string[] args);

public class ConsoleController
{
	#region Event declarations
	// Used to communicate with ConsoleView
	public delegate void LogChangedHandler(string[] log);
	public event LogChangedHandler logChanged;

	public delegate void VisibilityChangedHandler(bool visible);
	public event VisibilityChangedHandler visibilityChanged;
	#endregion

	/// 
	/// Object to hold information about each command
	/// 
	private class CommandRegistration
	{
		public string command { get; private set; }
		public CommandHandler handler { get; private set; }
		public string help { get; private set; }

		public CommandRegistration(
			string command, 
			CommandHandler handler, 
			string help)
		{
			this.command = command;
			this.handler = handler;
			this.help = help;
		}
	}

	/// 
	/// How many log lines should be retained?
	/// Note that strings submitted to appendLogLine with embedded newlines will be counted as a single line.
	/// 
	const int scrollbackSize = 20;

	Queue<string> scrollback = new Queue<string>(scrollbackSize);
	List<string> commandHistory = new List<string>();
	Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();
	
	// Copy of scrollback as an array for easier use by ConsoleView
	public string[] log { get; private set; } 
	
	private Command cmd;

	public ConsoleController(PathPlanner planner)
	{
		cmd = Command.Instance;
		cmd.initialize(
			planner
		);
		cmd.logMessage += appendLogLine;

		// When adding commands, you must add a call below to registerCommand() with its name, implementation method, and help text.
		registerCommand("echo", cmd.echo, "echoes arguments back as array (for testing argument parser)");
		registerCommand("reload", cmd.reload, "Reload game.");
		registerCommand("resetprefs", cmd.resetPrefs, "Reset & saves PlayerPrefs.");
		registerCommand("move", cmd.move, "Move character to the specified X & Y location.");
		registerCommand("create", cmd.create, "Create new prefab at the specified X & Y location.");
		registerCommand("destroy", cmd.destroy, "Destroy the specified game object.");
		registerCommand("script", cmd.script, "Execute script in Resources folder.");
	}

	void registerCommand(string command, CommandHandler handler, string help)
	{
		commands.Add(command, new CommandRegistration(command, handler, help));
	}

	public void appendLogLine(string line)
	{
		Debug.Log(line);

		if (scrollback.Count >= ConsoleController.scrollbackSize)
		{
			scrollback.Dequeue();
		}
		scrollback.Enqueue(line);
		logChanged?.Invoke(scrollback.ToArray());
	}

	public void runCommandString(string commandString)
	{
		appendLogLine("$ " + commandString);

		string command = "";
		string argString = "";
		string[] args = new string[0];
		bool commandFinished = false;
		for (int i = 0; i < commandString.Length; i++)
		{
			char nextChar = commandString[i];
			if (!commandFinished)
			{
				if (nextChar == '(')
				{
					commandFinished = true;
					continue;
				}
				else
				{
					command += nextChar;
				}
			}
			else
			{
				if (nextChar == ')')
				{
					args = argString.Split(',');
					break;
				}
				else
				{
					argString += nextChar;
				}
			}
		}

		//string[] commandSplit = parseArguments(commandString);
		//string[] args = new string[0];

		if (command.Length == 0) {
			return;
		}
		runCommand(command.ToLower(), args);
		commandHistory.Add(commandString);
	}
	
	public void runCommand(string command, string[] args)
	{
		CommandRegistration reg = null;
		if (!commands.TryGetValue(command, out reg))
		{
			appendLogLine(string.Format("Unknown command '{0}', type 'help' for list.", command));
		}
		else
		{
			if (reg.handler == null)
			{
				appendLogLine(string.Format("Unable to process command '{0}', handler was null.", command));
			}
			else
			{
				reg.handler(args);
			}
		}
	}
}