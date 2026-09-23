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
		new TotemData("Abstainer", "Each skipped capsule permanently adds +30 points per throw", 6, TotemRarity.Common, () => new AbstainerEffect()),
		new TotemData("Jackpot", "Randomly selects a scoring hole. Any ball scored in that hole gives $4. Hole changes every throw", 8, TotemRarity.Common, () => new JackpotEffect()),
		new TotemData("Prize Counter", "1 in 4 chance for each scored ball to create a random ball upgrade", 5, TotemRarity.Common, () => new PrizeCounterEffect()),
		
		// Rare
		new TotemData("Bonus Ball", "Gives 1 Bonus Ball", 6, TotemRarity.Rare, () => new BonusBallEffect()),
		new TotemData("Clutch Gene", "Last ball of round is worth 2x", 6, TotemRarity.Rare, () => new ClutchGeneEffect()),
		new TotemData("Penny Pincher", "Leftover balls worth double money", 8, TotemRarity.Rare, () => new PennyPincherEffect()),
		new TotemData("Lead from Gold", "For each Golden Ball in bag, +50 on each throw", 9, TotemRarity.Rare, () => new LeadFromGoldEffect()),
		new TotemData("Gambler", "Each shop reroll permanently adds +25 points per throw", 8, TotemRarity.Rare, () => new GamblerEffect()),
		new TotemData("Snowball", "Scoring gives +30 points for each ball already thrown this round", 9, TotemRarity.Rare, () => new SnowballEffect()),
		new TotemData("Deja Vu", "2x multiplier if a ball has already scored in this hole this round", 9, TotemRarity.Rare, () => new DejaVuEffect()),
		new TotemData("Tinkerer", "All ball upgrades and ball upgrade capsules are free", 10, TotemRarity.Rare, () => new TinkererEffect()),

		// Epic
		new TotemData("Hot Streak", "Consecutive scores increase multiplier by 0.1x, resets on miss", 14, TotemRarity.Epic, () => new HotStreakEffect()),
		new TotemData("Under Pressure", "Each miss increases multiplier by 0.25x, resets each round", 13, TotemRarity.Epic, () => new UnderPressureEffect()),
		new TotemData("Copycat", "Copies the effect of the totem below it", 15, TotemRarity.Epic, () => new CopycatEffect()),
		new TotemData("Alchemist's Touch", "The first non-locked scoring ball each round is converted to a Golden Ball", 13, TotemRarity.Epic, () => new AlchemistsTouchEffect()),
		new TotemData("Broken Window", "Each broken glass ball increases multiplier by 0.5x ", 13, TotemRarity.Epic, () => new BrokenWindowEffect()),
		new TotemData("Cannibal", "Gains 0.1x multiplier per upgraded ball scored, removes ball upgrade", 13, TotemRarity.Epic, () => new CannibalEffect()),
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
