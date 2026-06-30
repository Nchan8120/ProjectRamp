using System.Collections.Generic;
using Godot;

public static class TotemDatabase
{
	public static List<TotemData> AllTotems = new List<TotemData>
	{
		// Common
		new TotemData("Juggler", "+1 item slot", 5, TotemRarity.Common, () => new JugglerEffect()),
		new TotemData("Bailout", "Missing gives $1", 4, TotemRarity.Common, () => new BailoutEffect()),
		new TotemData("Free Reroll", "First reroll each shop is free", 5, TotemRarity.Common, () => new FreeRerollEffect()),

		// Rare
		new TotemData("Bonus Ball", "Gives 1 Bonus Ball", 8, TotemRarity.Rare, () => new BonusBallEffect()),
		new TotemData("Clutch Gene", "Last ball of round is worth 2x", 9, TotemRarity.Rare, () => new ClutchGeneEffect()),
		new TotemData("Penny Pincher", "Leftover balls worth double money", 8, TotemRarity.Rare, () => new PennyPincherEffect()),

		// Epic
		new TotemData("Hot Streak", "Consecutive scores increase multiplier by 0.1x, resets on miss", 14, TotemRarity.Epic, () => new HotStreakEffect()),
		new TotemData("Under Pressure", "Each miss increases multiplier by 0.25x, resets each round", 13, TotemRarity.Epic, () => new UnderPressureEffect()),
		new TotemData("Copycat", "Copies the effect of the totem below it", 15, TotemRarity.Epic, () => new CopycatEffect()),
	};

	public static List<TotemData> GetByRarity(TotemRarity rarity)
	{
		return AllTotems.FindAll(t => t.Rarity == rarity);
	}

	public static TotemData GetRandom()
	{
		// weighted rarity roll
		int roll = (int)GD.RandRange(1, 100);
		TotemRarity rarity;

		if (roll <= 61)
			rarity = TotemRarity.Common;
		else if (roll <= 86)
			rarity = TotemRarity.Rare;
		else if (roll <= 99)
			rarity = TotemRarity.Epic;
		else
			rarity = TotemRarity.Legendary;

		var pool = GetByRarity(rarity);

		// fallback to common if rarity pool is empty
		if (pool.Count == 0)
			pool = GetByRarity(TotemRarity.Common);

		return pool[(int)GD.RandRange(0, pool.Count - 1)];
	}
}
