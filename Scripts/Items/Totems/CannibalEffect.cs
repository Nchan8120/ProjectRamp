using Godot;
using System;

public class CannibalEffect : TotemEffect
{
	private float _multiplier = 1.0f;
	private int _consumeCount = 0;

	public override int ProcessScore(int points)
	{
		if (_multiplier > 1.0f)
			return Mathf.RoundToInt(points * _multiplier);
		return points;
	}

	public void TryConsumeBall(OwnedBall ball)
	{
		if (ball == null) return;
		if (ball.IsLocked) return;
		if (ball.UpgradeType == "Standard") return;

		GD.Print($"Cannibal consumed: {ball.UpgradeType} on Ball {ball.BallNumber}");
		ball.UpgradeType = "Standard";
		_consumeCount++;
		_multiplier = 1.0f + (_consumeCount * 0.1f);
	}

	public override string GetDisplayValue()
	{
		return _multiplier > 1.0f ? $"{_multiplier:0.0}x" : null;
	}
}
