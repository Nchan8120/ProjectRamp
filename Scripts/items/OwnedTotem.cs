using Godot;
using System;

public enum TotemRarity
{
	Common,
	Rare,
	Epic,
	Legendary
}

public class OwnedTotem
{
	public string Name;
	public string Description;
	public ItemType Type;
	public int PurchasePrice;
	public TotemRarity Rarity;
	public TotemEffect Effect;

	public OwnedTotem(TotemData data)
	{
		Name = data.Name;
		Description = data.Description;
		Type = ItemType.Totem;
		PurchasePrice = data.Cost;
		Rarity = data.Rarity;
		Effect = data.CreateEffect();
	}
	
	public int GetSellPrice(float multiplier)
	{
		return Mathf.RoundToInt(PurchasePrice * multiplier);
	}
}
