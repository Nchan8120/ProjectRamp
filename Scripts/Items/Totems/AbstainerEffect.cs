using Godot;
using System;

public class AbstainerEffect : TotemEffect
{
	private int _skipCount = 0;

	public override int ProcessScore(int points)
	{
		if (_skipCount > 0)
		{
			int bonus = _skipCount * 30;
			GD.Print($"Abstainer: {_skipCount} skips = +{bonus} points");
			return points + bonus;
		}
		return points;
	}

	public override void OnCapsuleSkipped()
	{
		_skipCount++;
		GD.Print($"Abstainer: skip count now {_skipCount}");
	}

	public override string GetDisplayValue()
	{
		return _skipCount > 0 ? $"+{_skipCount * 30}pts" : null;
	}
}
