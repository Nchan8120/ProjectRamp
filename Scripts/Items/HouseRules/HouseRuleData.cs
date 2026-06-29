using Godot;
using System;

public class HouseRuleData
{
	public string Name;
	public string Description;
	public int Cost;
	private Func<HouseRuleEffect> _effectFactory;

	public HouseRuleData(string name, string description, int cost, Func<HouseRuleEffect> effectFactory)
	{
		Name = name;
		Description = description;
		Cost = cost;
		_effectFactory = effectFactory;
	}

	public HouseRuleEffect CreateEffect()
	{
		return _effectFactory();
	}
}
