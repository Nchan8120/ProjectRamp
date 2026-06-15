using Godot;

public enum ItemType
{
	Totem,
	BallUpgrade,
	Engineering,
	Stunt,
	HouseRule
}

public enum CapsuleSize
{
	Small,  // pick 1 of 2 - $5
	Medium, // pick 1 of 4 - $8
	Large   // pick 2 of 6 - $10
}

public class ItemData
{
	public string Name;
	public string Description;
	public ItemType Type;
	public int Cost;

	public ItemData(string name, string description, ItemType type, int cost)
	{
		Name = name;
		Description = description;
		Type = type;
		Cost = cost;
	}
}

public class CapsuleData
{
	public ItemType Type;
	public CapsuleSize Size;
	public int Cost;

	public CapsuleData(ItemType type, CapsuleSize size)
	{
		Type = type;
		Size = size;
		Cost = size switch
		{
			CapsuleSize.Small => 5,
			CapsuleSize.Medium => 8,
			CapsuleSize.Large => 10,
			_ => 5
		};
	}
}
