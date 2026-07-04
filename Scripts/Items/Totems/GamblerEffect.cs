using Godot;
using System;

public class GamblerEffect : TotemEffect
{
	private int _rerollCount = 0;

	public override int ProcessScore(int points)
	{
		if (_rerollCount > 0)
		{
			int bonus = _rerollCount * 25;
			GD.Print($"Gambler: {_rerollCount} rerolls = +{bonus} points");
			return points + bonus;
		}
		return points;
	}

	public override void OnReroll()
	{
		_rerollCount++;
		GD.Print($"Gambler: reroll count now {_rerollCount}");
	}

	public override string GetDisplayValue()
	{
		return _rerollCount > 0 ? $"+{_rerollCount * 25}pts" : null;
	}
}
