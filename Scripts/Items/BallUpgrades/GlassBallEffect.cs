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
		
		// notify Broken Window totem if active
		foreach (OwnedTotem totem in GameState.OwnedTotems)
		{
			if (totem?.Effect is BrokenWindowEffect brokenWindow)
			{
				brokenWindow.OnGlassBallBroken();
				break;
			}
		}
	
		GD.Print($"Glass Ball shattered! Scored {modifiedPoints} points.");

		return modifiedPoints;
	}
}
