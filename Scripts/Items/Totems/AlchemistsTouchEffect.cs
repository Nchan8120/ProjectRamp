using Godot;
using System;

public class AlchemistsTouchEffect : TotemEffect
{
	private bool _hasTriggeredThisRound = false;

	public override void OnRoundStart()
	{
		_hasTriggeredThisRound = false;
	}

	public override void OnScore(int points)
	{
		// handled in ProcessScore to access ball data
	}

	public void TryConvertBall(OwnedBall ball)
	{
		if (_hasTriggeredThisRound) return;
		if (ball.IsLocked) return;
		if (ball.UpgradeType == "Golden Ball") return;

		ball.UpgradeType = "Golden Ball";
		_hasTriggeredThisRound = true;
		GD.Print($"Alchemist's Touch: Ball {ball.BallNumber} converted to Golden Ball");
	}

	public override string GetDisplayValue()
	{
		return _hasTriggeredThisRound ? "Used" : "Ready";
	}
}
