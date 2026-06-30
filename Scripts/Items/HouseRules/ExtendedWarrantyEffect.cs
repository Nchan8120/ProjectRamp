using Godot;
using System;

public class ExtendedWarrantyEffect : HouseRuleEffect
{
	public override void OnPurchased()
	{
		GameState.SellValueMultiplier = 0.75f; // bump from 50% to 75%
		GD.Print($"Extended Warranty applied - sell multiplier now {GameState.SellValueMultiplier}");
	}
}
