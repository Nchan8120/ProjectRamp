using Godot;
using System;
using System.Collections.Generic;

public partial class SaveManager : Node
{
	private const string SavePath = "user://save.cfg";
	private ConfigFile _config = new ConfigFile();

	public override void _Ready()
	{
		Load();
	}

	private void Load()
	{
		Error err = _config.Load(SavePath);
		if (err != Error.Ok)
			GD.Print("No save file found, starting fresh");
	}

	private void Save()
	{
		_config.Save(SavePath);
	}

	// difficulty unlocks stored per machine
	public int GetHighestDifficultyUnlocked(string machineName)
	{
		return (int)_config.GetValue("difficulty_unlocks", machineName, 0);
	}

	public void SetHighestDifficultyUnlocked(string machineName, int difficultyIndex)
	{
		int current = GetHighestDifficultyUnlocked(machineName);
		if (difficultyIndex > current)
		{
			_config.SetValue("difficulty_unlocks", machineName, difficultyIndex);
			Save();
			GD.Print($"Saved difficulty unlock: {machineName} = {difficultyIndex}");
		}
	}
}
