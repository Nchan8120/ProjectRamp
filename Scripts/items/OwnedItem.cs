using Godot;
using System;

public class OwnedItem
{
	public string Name;
	public string Description;
	public ItemType Type;
	public int PurchasePrice;

	public OwnedItem(ItemData data)
	{
		Name = data.Name;
		Description = data.Description;
		Type = data.Type;
		PurchasePrice = data.Cost;
	}
	
	public int GetSellPrice(float multiplier)
	{
		return Mathf.RoundToInt(PurchasePrice * multiplier);
	}
}
