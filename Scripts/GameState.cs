using Godot;
using System.Collections.Generic;

public partial class GameState : Node
{
	// Economy
	public int Money = 0;
	public int CurrentRound = 1;
	public int TotalRounds = 10;
	public int LeftoverBalls = 0;

	// Owned items
	public List<OwnedTotem> OwnedTotems = new List<OwnedTotem>();
	public List<OwnedItem> OwnedItems = new List<OwnedItem>();
	public int MaxItems = 3;
	public List<string> OwnedHouseRules = new List<string>();

	// Shop state
	public int RerollCost = 3;
	public string PersistentHouseRule = null; // carries over if not bought
	public bool HouseRuleAvailableThisShop = false;

	// Slot limits
	public int MaxTotems = 5;
	public int MaxStunts = 3;

	// Run stats 
	public int TotalScore = 0;
	public int HighestRoundReached = 1;
	public int BallsThrown = 0;
	public int ItemsBought = 0;
	public int TimesRerolled = 0;
	public int HighestRoundScore = 0;
	public string DefeatedByBoss = null; // null if not defeated by boss
	
	public bool IsEndlessMode = false;

	public void AddMoney(int amount)
	{
		Money += amount;
		GD.Print($"Money: {Money} (+{amount})");
	}

	public bool SpendMoney(int amount)
	{
		if (Money < amount)
		{
			GD.Print("Not enough money!");
			return false;
		}
		Money -= amount;
		GD.Print($"Money: {Money} (-{amount})");
		return true;
	}

	public void ResetRun()
	{
		Money = 0;
		CurrentRound = 1;
		RerollCost = 3;
		PersistentHouseRule = null;
		OwnedTotems.Clear();
		OwnedItems.Clear();
		TotalScore = 0;
		HighestRoundReached = 1;
		BallsThrown = 0;
		ItemsBought = 0;
		TimesRerolled = 0;
		HighestRoundScore = 0;
		DefeatedByBoss = null;
		LeftoverBalls = 0;
		IsEndlessMode = false;
	}

	public void AwardRoundEndMoney(int leftoverBalls)
	{
		int winBonus = 5;
		int ballBonus = leftoverBalls * 2;
		AddMoney(winBonus + ballBonus);
		GD.Print($"Round rewards: ${winBonus} win bonus + ${ballBonus} ball bonus");
	}
}
