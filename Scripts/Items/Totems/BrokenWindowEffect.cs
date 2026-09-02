using Godot;
using System;

public class BrokenWindowEffect : TotemEffect
{
	private float _multiplier = 1.0f;
	private int _brokenCount = 0;

	public override int ProcessScore(int points)
	{
		if (_multiplier > 1.0f)
			return Mathf.RoundToInt(points * _multiplier);
		return points;
	}

	public void OnGlassBallBroken()
	{
		_brokenCount++;
		_multiplier = 1.0f + (_brokenCount * 0.5f);
		GD.Print($"Broken Window: {_brokenCount} broken = {_multiplier}x multiplier");
	}

	public override string GetDisplayValue()
	{
		return _brokenCount > 0 ? $"{_multiplier:0.0}x" : null;
	}
}
