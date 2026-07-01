using System;
using Godot;

public class ClutchGeneEffect : TotemEffect
{
	public override int ProcessScore(int points)
	{
		// only applies on the last ball
		if (RoundManager != null && RoundManager.BallsRemaining == 1)
		{
			GD.Print("Clutch Gene triggered - 2x points!");
			return points * 2;
		}
		return points;
	}
}
