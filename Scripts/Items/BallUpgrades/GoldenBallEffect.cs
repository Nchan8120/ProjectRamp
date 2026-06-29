using Godot;
using System;

public class GoldenBallEffect : BallUpgradeEffect
{
	public override int OnScore(int points, OwnedBall ball)
	{
		GameState.AddMoney(3);
		GD.Print("Golden Ball scored - +$3");
		return points; // score unchanged
	}
}
