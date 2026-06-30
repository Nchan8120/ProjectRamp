using Godot;
using System;
using System.Collections.Generic;

public static class HouseRuleDatabase
{
	public static List<HouseRuleData> AllHouseRules = new List<HouseRuleData>
	{
		new HouseRuleData("Extra Ball", "Adds 1 Standard Ball to Ball Bag", 15, () => new ExtraBallEffect()),
		new HouseRuleData("Extra Totem", "+1 totem slot", 15, () => new ExtraTotemEffect()),
	};

	public static HouseRuleData GetByName(string name)
	{
		return AllHouseRules.Find(r => r.Name == name);
	}
}
