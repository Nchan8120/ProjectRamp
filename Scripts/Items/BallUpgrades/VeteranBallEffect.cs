using Godot;
using System;

public partial class VeteranBallEffect : BallUpgradeEffect
{
	public override int OnScore(int points, OwnedBall ball)
	{
		return points + ball.BonusPoints;
	}
}
