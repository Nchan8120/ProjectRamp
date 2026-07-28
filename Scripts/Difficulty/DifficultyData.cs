using Godot;
using System;
using System.Collections.Generic;

public enum Difficulty
{
	Easy,
	Medium,
	Hard,
	Expert,
	Master,
	TheChosenOne
}

public class DifficultyData
{
	public Difficulty Level;
	public string Name;
	public string Description;

	public DifficultyData(Difficulty level, string name, string description)
	{
		Level = level;
		Name = name;
		Description = description;
	}
}

public static class DifficultyDatabase
{
	public static List<DifficultyData> AllDifficulties = new List<DifficultyData>
	{
		new DifficultyData(Difficulty.Easy, "Easy", "No modifiers. Pure skee-ball."),
		new DifficultyData(Difficulty.Medium, "Medium", "Round completion bonus money reduced by half."),
		new DifficultyData(Difficulty.Hard, "Hard", "Medium effects + score requirement multiplies by 1.3x each round."),
		new DifficultyData(Difficulty.Expert, "Expert", "Hard effects + start with one less ball in bag."),
		new DifficultyData(Difficulty.Master, "Master", "Expert effects + score requirement multiplies by 1.8x each round."),
		new DifficultyData(Difficulty.TheChosenOne, "The Chosen One", "Master effects + base item slots reduced to 2."),
	};
}
