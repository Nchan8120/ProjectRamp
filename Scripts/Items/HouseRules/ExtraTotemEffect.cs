using Godot;
using System;

public class ExtraTotemEffect : HouseRuleEffect
{
	public override void OnPurchased()
	{
		GameState.MaxTotems += 1;
		GD.Print($"Extra Totem slot added - MaxTotems now {GameState.MaxTotems}");
	}
}
