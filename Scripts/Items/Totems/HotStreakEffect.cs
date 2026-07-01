using System;
using Godot;

public class HotStreakEffect : TotemEffect
{
	private float _multiplier = 1.0f;
	private int _streak = 0;

	public override int ProcessScore(int points)
	{
		_streak++;
		_multiplier = 1.0f + (_streak * 0.1f);
		return Mathf.RoundToInt(points * _multiplier);
	}

	public override void OnMiss()
	{
		_streak = 0;
		_multiplier = 1.0f;
	}

	public override string GetDisplayValue()
	{
		return $"{_multiplier:0.0}x";
	}
}
