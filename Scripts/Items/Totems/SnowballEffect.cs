using Godot;
using System;

public partial class SnowballEffect : TotemEffect
{
	public override int ProcessScore(int points)
	{
		int ballsThrown = RoundManager.CurrentBallIndex;
		if (ballsThrown > 0)
		{
			int bonus = ballsThrown * 30;
			GD.Print($"Last Stand: {ballsThrown} balls thrown = +{bonus} points");
			return points + bonus;
		}
		return points;
	}

	public override string GetDisplayValue()
	{
		if (RoundManager == null) return null;
		int ballsThrown = RoundManager.CurrentBallIndex;
		return ballsThrown > 0 ? $"+{ballsThrown * 30}pts" : null;
	}
}
