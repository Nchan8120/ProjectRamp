using Godot;
using System;

public class IceCubeBallEffect : BallUpgradeEffect
{
	public const int InitialBonus = 300;
	public const int DecayAmount = 75;

	public override int OnScore(int points, OwnedBall ball)
	{
		int modifiedPoints = points + ball.BonusPoints;
		GD.Print($"Ice Cube Ball scored with +{ball.BonusPoints} bonus points");

		Decay(ball);

		return modifiedPoints;
	}

	public override void OnMiss(OwnedBall ball)
	{
		Decay(ball);
	}

	private void Decay(OwnedBall ball)
	{
		ball.BonusPoints -= DecayAmount;

		if (ball.BonusPoints <= 0)
		{
			ball.BonusPoints = 0;
			GameState.OwnedBalls.Remove(ball);
			BallWasRemoved = true;
			GD.Print("Ice Cube Ball fully melted and was removed from the bag.");
		}
	}
}
