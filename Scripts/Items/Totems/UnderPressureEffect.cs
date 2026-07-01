using System;
using Godot;

public class UnderPressureEffect : TotemEffect
{
	private int _missCount = 0;

	public override int ProcessScore(int points)
	{
		float multiplier = 1.0f + (_missCount * 0.25f);
		return Mathf.RoundToInt(points * multiplier);
	}

	public override void OnMiss()
	{
		_missCount++;
	}

	public override void OnRoundStart()
	{
		_missCount = 0;
	}
	
	public override string GetDisplayValue()
	{
		float multiplier = 1.0f + (_missCount * 0.25f);
		return $"{multiplier:0.00}x";
	}

}
