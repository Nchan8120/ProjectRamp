using Godot;
using System;

public class PrizeCounterEffect : TotemEffect
{
	private const float Chance = 0.25f;

	public override void OnScore(int points)
	{
		if (GD.Randf() >= Chance) return;

		// find an open item slot (first null hole, otherwise append if there's room)
		int emptyIndex = GameState.OwnedItems.IndexOf(null);
		bool hasRoom = emptyIndex >= 0 || GameState.OwnedItems.Count < GameState.MaxItems;
		if (!hasRoom)
		{
			GD.Print("Prize Counter: item slots full, nothing created");
			return;
		}

		// equal chance of any ball upgrade
		var upgrades = ItemDatabase.BallUpgrades;
		ItemData prize = upgrades[(int)GD.RandRange(0, upgrades.Count - 1)];
		OwnedItem newItem = new OwnedItem(prize);

		if (emptyIndex >= 0)
			GameState.OwnedItems[emptyIndex] = newItem;
		else
			GameState.OwnedItems.Add(newItem);

		GD.Print($"Prize Counter: created {prize.Name}");

		// refresh the item panel so the new item shows up
		if (RoundManager != null)
		{
			var panels = RoundManager.GetTree().GetNodesInGroup("ItemPanel");
			foreach (Node node in panels)
				if (node is ItemPanel ip) ip.RefreshUI();
		}
	}
}
