using Godot;
using System;

public class RubberBallEffect : BallUpgradeEffect
{
	public const int BounceBonusValue = 25;
	
	public override void Initialize(GameState gameState, RoundManager roundManager)
	{
		base.Initialize(gameState, roundManager);
		ApplyRubberPhysics();
	}

	private void ApplyRubberPhysics()
	{
		BallController ball = RoundManager.GetBallController();
		GD.Print($"RubberBallEffect setting BounceBonus on instance ID: {ball.GetInstanceId()}");
		if (ball != null)
		{
			ball.ApplyPhysicsMaterial("Rubber Ball");
			ball.BounceBonus = BounceBonusValue;
			GD.Print($"BounceBonus after set: {ball.BounceBonus}");
		}
		GD.Print("Rubber Ball physics applied");
	}
}
