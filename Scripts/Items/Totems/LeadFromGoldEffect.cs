using Godot;
using System;

public class LeadFromGoldEffect : TotemEffect
{
	public override int ProcessScore(int points)
	{
		int goldenBallCount = CountGoldenBalls();
		if (goldenBallCount > 0)
		{
			int bonus = goldenBallCount * 50;
			GD.Print($"Lead from Gold: {goldenBallCount} Golden Balls = +{bonus} points");
			return points + bonus;
		}
		return points;
	}

	private int CountGoldenBalls()
	{
		int count = 0;
		foreach (OwnedBall ball in GameState.OwnedBalls)
		{
			if (ball.UpgradeType == "Golden Ball")
				count++;
		}
		return count;
	}

	public override string GetDisplayValue()
	{
		int count = CountGoldenBalls();
		return count > 0 ? $"+{count * 50}pts" : null;
	}
}
