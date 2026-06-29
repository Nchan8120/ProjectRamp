using Godot;
using System;

public class ExtraBallEffect : HouseRuleEffect
{
	public override void OnPurchased()
	{
		int ballNumber = GameState.OwnedBalls.Count + 1;
		GameState.OwnedBalls.Add(new OwnedBall(ballNumber));
		GD.Print($"Extra Ball added - Ball {ballNumber}");
	}
}
