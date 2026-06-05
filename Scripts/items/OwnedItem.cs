public class OwnedItem
{
	public string Name;
	public string Description;
	public ItemType Type;
	public int PurchasePrice;
	public int SellPrice => PurchasePrice / 2;

	public OwnedItem(ItemData data)
	{
		Name = data.Name;
		Description = data.Description;
		Type = data.Type;
		PurchasePrice = data.Cost;
	}
}
