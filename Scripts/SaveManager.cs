using Godot;
using System;
using System.Collections.Generic;

	public partial class SaveManager : Node
	{
		private const string SaveFolder = "user://Saves/";
		private int _activeProfile = 0;
		public int ActiveProfile => _activeProfile;

		private ConfigFile[] _profiles = new ConfigFile[3];

		public override void _Ready()
		{
			// create save directory if needed
			DirAccess dir = DirAccess.Open("user://");
			if (!dir.DirExists("Saves"))
				dir.MakeDir("Saves");

			// load all 3 profiles
			for (int i = 0; i < 3; i++)
			{
				_profiles[i] = new ConfigFile();
				_profiles[i].Load(GetProfilePath(i));
			}
		}

		private string GetProfilePath(int profileIndex)
		{
			return $"{SaveFolder}profile_{profileIndex}.cfg";
		}

		private void SaveProfile(int profileIndex)
		{
			_profiles[profileIndex].Save(GetProfilePath(profileIndex));
		}

		public void SetActiveProfile(int profileIndex)
		{
			_activeProfile = profileIndex;
			GD.Print($"Active profile set to {profileIndex + 1}");
		}

		// difficulty unlocks
		public int GetHighestDifficultyUnlocked(string machineName)
		{
			return (int)_profiles[_activeProfile].GetValue("difficulty_unlocks", machineName, 0);
		}

		public void SetHighestDifficultyUnlocked(string machineName, int difficultyIndex)
		{
			int current = GetHighestDifficultyUnlocked(machineName);
			if (difficultyIndex > current)
			{
				_profiles[_activeProfile].SetValue("difficulty_unlocks", machineName, difficultyIndex);
				SaveProfile(_activeProfile);
				GD.Print($"Saved difficulty unlock: {machineName} = {difficultyIndex}");
			}
		}

		// runs played
		public int GetRunsPlayed()
		{
			return (int)_profiles[_activeProfile].GetValue("stats", "runs_played", 0);
		}

		public void IncrementRunsPlayed()
		{
			int runs = GetRunsPlayed() + 1;
			_profiles[_activeProfile].SetValue("stats", "runs_played", runs);
			SaveProfile(_activeProfile);
		}

		// profile info for display
		public bool IsProfileEmpty(int profileIndex)
		{
			return (int)_profiles[profileIndex].GetValue("stats", "runs_played", 0) == 0;
		}

		public int GetProfileRunsPlayed(int profileIndex)
		{
			return (int)_profiles[profileIndex].GetValue("stats", "runs_played", 0);
		}

		public int GetProfileHighestDifficulty(int profileIndex, string machineName)
		{
			return (int)_profiles[profileIndex].GetValue("difficulty_unlocks", machineName, 0);
		}

		// delete profile
		public void DeleteProfile(int profileIndex)
		{
			_profiles[profileIndex] = new ConfigFile();
			DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(GetProfilePath(profileIndex)));
			GD.Print($"Deleted profile {profileIndex + 1}");
		}

		// unlock everything for active profile
		public void UnlockEverything()
		{
			int maxDifficulty = DifficultyDatabase.AllDifficulties.Count - 1;
			// unlock all difficulties for all machines
			string[] machines = { "The Original", "The Haunted House", "The Factory", "The Casino", "Space Station" };
			foreach (string machine in machines)
			{
				_profiles[_activeProfile].SetValue("difficulty_unlocks", machine, maxDifficulty);
			}
			SaveProfile(_activeProfile);
			GD.Print("Unlocked everything for active profile");
		}
	}
