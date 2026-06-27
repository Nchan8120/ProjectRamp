using Godot;
using System.Collections.Generic;

public static class ItemDatabase
{
	public static List<ItemData> BallUpgrades = new List<ItemData>
	{
		new ItemData("Gold Plated", "Ball worth +50 points every throw", ItemType.BallUpgrade, 5),
		new ItemData("Rubber Coat", "Ball bounces higher off ramp", ItemType.BallUpgrade, 4),
		new ItemData("Hollow Core", "Ball gets more air off the ramp", ItemType.BallUpgrade, 5),
		new ItemData("Coin Filled", "Scoring zero still generates $2", ItemType.BallUpgrade, 6),
		new ItemData("Veteran", "Gains +10 points every round survived", ItemType.BallUpgrade, 7),
		new ItemData("Sticky Ball", "Ball sticks to the wall", ItemType.BallUpgrade, 7),
		new ItemData("Glass Ball", "Ball  Scores 2x points, then breaks permanently", ItemType.BallUpgrade, 5),
	};

	public static List<ItemData> Engineering = new List<ItemData>
	{
		new ItemData("Wax Job", "Approach is frictionless", ItemType.Engineering, 4),
		new ItemData("Bumper Rails", "Side bumpers redirect balls toward center", ItemType.Engineering, 4),
		new ItemData("Expanded Bullseye", "Bullseye is 20% larger", ItemType.Engineering, 5),
		new ItemData("Scoring Walls", "Hitting a wall generates +25 points", ItemType.Engineering, 5),
		new ItemData("Turbo Pad", "Launch pad gives ball extra lift", ItemType.Engineering, 6),
		new ItemData("Demolition", "Destroy obstacles in a small area", ItemType.Engineering, 6),
	};

	public static List<ItemData> Stunts = new List<ItemData>
	{
		new ItemData("Scope", "Shows trajectory preview for next throw", ItemType.Stunt, 4),
		new ItemData("Magnet", "Pulls ball toward bullseye for one second", ItemType.Stunt, 5),
		new ItemData("Double Down", "Costs 2 balls, next throw scores double", ItemType.Stunt, 6),
		new ItemData("Nudge", "Give ball a small directional push mid air", ItemType.Stunt, 4),
		new ItemData("Clutch", "Only on last ball, triples its score", ItemType.Stunt, 7),
		new ItemData("Ghost", "Ball passes through one wall before becomeing solid again", ItemType.Stunt, 7),
	};

	public static List<ItemData> HouseRules = new List<ItemData>
	{
		new ItemData("Extra Ball", "Start every round with +1 ball", ItemType.HouseRule, 15),
		new ItemData("Safety Net", "First game over attempt in run, survive with 1 point instead", ItemType.HouseRule, 15),
		new ItemData("High Interest Account", "Interest rate permanently doubled", ItemType.HouseRule, 15),
		new ItemData("Extra Totem Slot", "Can hold +1 totem permanently", ItemType.HouseRule, 15),
		new ItemData("Bulk Discount", "Capsules cost $1 less for rest of run", ItemType.HouseRule, 15),
	};

	public static List<ItemData> GetListByType(ItemType type)
	{
		return type switch
		{
			ItemType.BallUpgrade => BallUpgrades,
			ItemType.Engineering => Engineering,
			ItemType.Stunt => Stunts,
			ItemType.HouseRule => HouseRules,
			_ => BallUpgrades
		};
	}

	public static ItemData GetRandom(ItemType type)
	{
		if (type == ItemType.Totem)
		{
			TotemData totemData = TotemDatabase.GetRandom();
			return new ItemData(totemData.Name, totemData.Description, ItemType.Totem, totemData.Cost);
		}

		var list = GetListByType(type);
		return list[(int)GD.RandRange(0, list.Count - 1)];
	}

	public static ItemType GetRandomType()
	{
		// excludes HouseRule since those are handled separately
		return (ItemType)GD.RandRange(0, 3);
	}

	public static CapsuleSize GetRandomSize()
	{
		int roll = GD.RandRange(1, 100);
		if (roll <= 50) return CapsuleSize.Small;
		if (roll <= 80) return CapsuleSize.Medium;
		return CapsuleSize.Large;
	}
}
