using Godot;
using System;

public class ExtraItemEffect : HouseRuleEffect
{
	public override void OnPurchased()
	{
		GameState.MaxItems += 1;
		GD.Print($"Extra Item slot added - MaxItems now {GameState.MaxItems}");
	}
}
