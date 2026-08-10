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
	public bool HasFreeReroll = false;
	public float SellValueMultiplier = 0.5f; // default 50% sell back

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
	
	// Totem flags
	public int BallsPerRound = 9;
	public int LeftoverBallValue = 2;
	
	// Ball Bag
	public List<OwnedBall> OwnedBalls = new List<OwnedBall>();
	public int StartingBallCount = 9;
	
	// Machines/Difficulty
	public string CurrentMachine = "The Original";
	public Difficulty CurrentDifficulty = Difficulty.Easy;
	public int CurrentThreshold = 0;
	// difficulty modifiers - read by game systems
	public int WinBonus => CurrentDifficulty >= Difficulty.Medium ? 2 : 5;
	public float ThresholdMultiplier => CurrentDifficulty >= Difficulty.Master ? 1.8f : 
										CurrentDifficulty >= Difficulty.Hard ? 1.3f : 0f;
	public bool UsesFlatThresholdScaling => CurrentDifficulty < Difficulty.Hard;
	public int StartingBallPenalty => CurrentDifficulty >= Difficulty.Expert ? 1 : 0;
	public int BaseItemSlots => CurrentDifficulty >= Difficulty.TheChosenOne ? 2 : 3;
	
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
		LeftoverBallValue = 2;
		SellValueMultiplier = 0.5f;
		CurrentMachine = "The Original";
		CurrentDifficulty = Difficulty.Easy;
		MaxItems = BaseItemSlots;
		HasFreeReroll = false;
		CurrentThreshold = 0;
		InitializeBalls();
	}

	public void AwardRoundEndMoney(int leftoverBalls)
	{
		int winBonus = WinBonus; // ← uses difficulty modifier
		int ballBonus = leftoverBalls * LeftoverBallValue;
		AddMoney(winBonus + ballBonus);
		GD.Print($"Round rewards: ${winBonus} win bonus + ${ballBonus} ball bonus");
	}
	
	public void InitializeBalls()
	{
		OwnedBalls.Clear();
		int count = StartingBallCount - StartingBallPenalty;
		for (int i = 0; i < count; i++)
			OwnedBalls.Add(new OwnedBall(i + 1));
	}
	
	public override void _Ready()
	{
		InitializeBalls();
	}
}
