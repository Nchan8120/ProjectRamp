using Godot;
using System;

public class GlassBallEffect : BallUpgradeEffect
{
	public override int OnScore(int points, OwnedBall ball)
	{
		// double the points
		int modifiedPoints = points * 2;

		// remove ball from bag permanently
		GameState.OwnedBalls.Remove(ball);
		BallWasRemoved = true;
		GD.Print($"Glass Ball shattered! Scored {modifiedPoints} points.");

		return modifiedPoints;
	}
}
