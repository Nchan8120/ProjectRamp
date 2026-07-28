using Godot;
using System;

public class MachineData
{
	public string Name;
	public string Description;
	public bool IsUnlocked;
	public string UnlockCondition;

	public MachineData(string name, string description, bool isUnlocked, string unlockCondition = "Coming Soon")
	{
		Name = name;
		Description = description;
		IsUnlocked = isUnlocked;
		UnlockCondition = unlockCondition;
	}
}
