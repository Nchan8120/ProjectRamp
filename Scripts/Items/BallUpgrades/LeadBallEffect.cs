using Godot;
using System;

public class LeadBallEffect : BallUpgradeEffect
{
	public override void Initialize(GameState gameState, RoundManager roundManager)
	{
		base.Initialize(gameState, roundManager);

		// make the active ball big and heavy
		BallController ball = RoundManager.GetBallController();
		ball?.ApplyPhysicsMaterial("Lead Ball");
	}
}
