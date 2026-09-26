using Godot;
using System;

public class BackpackEffect : TotemEffect
{
	private float _multiplier = 1.0f;
	private int _ballsGained = 0;
	private int _lastKnownBallCount = -1;

	public override void Initialize(GameState gameState, RoundManager roundManager)
	{
		base.Initialize(gameState, roundManager);
		// baseline the count only on first initialize, not every scene reload
		if (_lastKnownBallCount == -1)
			_lastKnownBallCount = GameState.OwnedBalls.Count;
	}

	public override void ApplyPassiveEffects()
	{
		// fires whenever ANY totem is added/removed (e.g. buying Bonus Ball), plus round start/shop enter
		CheckForNewBalls();
	}

	public override int ProcessScore(int points)
	{
		CheckForNewBalls();
		if (_multiplier > 1.0f)
			return Mathf.RoundToInt(points * _multiplier);
		return points;
	}

	private void CheckForNewBalls()
	{
		int currentCount = GameState.OwnedBalls.Count;
		if (currentCount > _lastKnownBallCount)
		{
			int newBalls = currentCount - _lastKnownBallCount;
			_ballsGained += newBalls;
			_multiplier = 1.0f + (_ballsGained * 0.2f);
			GD.Print($"Backpack: +{newBalls} ball(s) added, multiplier now {_multiplier}x");
		}
		_lastKnownBallCount = currentCount;
	}

	public override string GetDisplayValue()
	{
		return _multiplier > 1.0f ? $"{_multiplier:0.0}x" : null;
	}
}
