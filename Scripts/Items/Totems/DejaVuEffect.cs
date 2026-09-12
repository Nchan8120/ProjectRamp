using Godot;
using System;
using System.Collections.Generic;

public class DejaVuEffect : TotemEffect
{
	private HashSet<int> _scoredHoles = new HashSet<int>();
	private bool _triggeredThisThrow = false;

	public override void OnRoundStart()
	{
		_scoredHoles.Clear();
		_triggeredThisThrow = false;
	}

	public override void OnHoleScored(int holeIndex)
	{
		_triggeredThisThrow = _scoredHoles.Contains(holeIndex);
		_scoredHoles.Add(holeIndex);
	}

	public override int ProcessScore(int points)
	{
		if (_triggeredThisThrow)
		{
			GD.Print($"Deja Vu triggered! 2x points");
			return points * 2;
		}
		return points;
	}

	public override void OnMiss()
	{
		_triggeredThisThrow = false;
	}

	public override string GetDisplayValue()
	{
		return _triggeredThisThrow ? "x2 Scored!" : null;
	}
}
