using Godot;
using System;

public class TinkererEffect : TotemEffect
{
	public override void ApplyPassiveEffects()
	{
		GameState.BallUpgradesFree = true;
	}
}
