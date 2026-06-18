using System;

public class TotemData
{
	public string Name;
	public string Description;
	public int Cost;
	public TotemRarity Rarity;
	private Func<TotemEffect> _effectFactory;

	public TotemData(string name, string description, int cost, TotemRarity rarity, Func<TotemEffect> effectFactory)
	{
		Name = name;
		Description = description;
		Cost = cost;
		Rarity = rarity;
		_effectFactory = effectFactory;
	}

	public TotemEffect CreateEffect()
	{
		return _effectFactory();
	}
}
