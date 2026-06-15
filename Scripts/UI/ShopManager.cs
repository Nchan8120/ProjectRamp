using Godot;
using System.Collections.Generic;

public partial class ShopManager : Control
{
	// node references
	[Export] public NodePath MoneyLabelPath;
	[Export] public NodePath RerollButtonPath;
	[Export] public NodePath RerollCostLabelPath;
	[Export] public NodePath ContinueButtonPath;

	// item slots
	[Export] public NodePath ItemSlot1NamePath;
	[Export] public NodePath ItemSlot1TypePath;
	[Export] public NodePath ItemSlot1CostPath;
	[Export] public NodePath ItemSlot1BuyPath;

	[Export] public NodePath ItemSlot2NamePath;
	[Export] public NodePath ItemSlot2TypePath;
	[Export] public NodePath ItemSlot2CostPath;
	[Export] public NodePath ItemSlot2BuyPath;

	// capsule machines
	[Export] public NodePath Capsule1TypePath;
	[Export] public NodePath Capsule1SizePath;
	[Export] public NodePath Capsule1CostPath;
	[Export] public NodePath Capsule1BuyPath;

	[Export] public NodePath Capsule2TypePath;
	[Export] public NodePath Capsule2SizePath;
	[Export] public NodePath Capsule2CostPath;
	[Export] public NodePath Capsule2BuyPath;

	// house rule slot
	[Export] public NodePath HouseRuleNamePath;
	[Export] public NodePath HouseRuleCostPath;
	[Export] public NodePath HouseRuleBuyPath;
	[Export] public NodePath HouseRulePersistPath;
	[Export] public NodePath HouseRuleSlotPath;

	// labels
	private Label _moneyLabel;
	private Label _rerollCostLabel;
	private Button _rerollButton;
	private Button _continueButton;

	// item slot UI
	private Label _item1Name, _item1Type, _item1Cost;
	private Button _item1Buy;
	private Label _item2Name, _item2Type, _item2Cost;
	private Button _item2Buy;

	// capsule UI
	private Label _cap1Type, _cap1Size, _cap1Cost;
	private Button _cap1Buy;
	private Label _cap2Type, _cap2Size, _cap2Cost;
	private Button _cap2Buy;

	// house rule UI
	private Label _houseRuleName, _houseRuleCost, _houseRulePersist;
	private Button _houseRuleBuy;
	private Control _houseRuleSlot;

	// shop state
	private GameState _gameState;
	private ItemData _item1Data;
	private ItemData _item2Data;
	private CapsuleData _capsule1Data;
	private CapsuleData _capsule2Data;
	private ItemData _houseRuleData;
	private bool _item1Sold = false;
	private bool _item2Sold = false;
	private bool _cap1Sold = false;
	private bool _cap2Sold = false;
	private bool _houseRuleSold = false;

	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");

		// get node references
		_moneyLabel = GetNode<Label>(MoneyLabelPath);
		_rerollCostLabel = GetNode<Label>(RerollCostLabelPath);
		_rerollButton = GetNode<Button>(RerollButtonPath);
		_continueButton = GetNode<Button>(ContinueButtonPath);

		_item1Name = GetNode<Label>(ItemSlot1NamePath);
		_item1Type = GetNode<Label>(ItemSlot1TypePath);
		_item1Cost = GetNode<Label>(ItemSlot1CostPath);
		_item1Buy = GetNode<Button>(ItemSlot1BuyPath);

		_item2Name = GetNode<Label>(ItemSlot2NamePath);
		_item2Type = GetNode<Label>(ItemSlot2TypePath);
		_item2Cost = GetNode<Label>(ItemSlot2CostPath);
		_item2Buy = GetNode<Button>(ItemSlot2BuyPath);

		_cap1Type = GetNode<Label>(Capsule1TypePath);
		_cap1Size = GetNode<Label>(Capsule1SizePath);
		_cap1Cost = GetNode<Label>(Capsule1CostPath);
		_cap1Buy = GetNode<Button>(Capsule1BuyPath);

		_cap2Type = GetNode<Label>(Capsule2TypePath);
		_cap2Size = GetNode<Label>(Capsule2SizePath);
		_cap2Cost = GetNode<Label>(Capsule2CostPath);
		_cap2Buy = GetNode<Button>(Capsule2BuyPath);

		_houseRuleName = GetNode<Label>(HouseRuleNamePath);
		_houseRuleCost = GetNode<Label>(HouseRuleCostPath);
		_houseRuleBuy = GetNode<Button>(HouseRuleBuyPath);
		_houseRulePersist = GetNode<Label>(HouseRulePersistPath);
		_houseRuleSlot = GetNode<Control>(HouseRuleSlotPath);

		// connect buttons
		_rerollButton.Pressed += OnRerollPressed;
		_continueButton.Pressed += OnContinuePressed;
		_item1Buy.Pressed += () => OnItemBuyPressed(1);
		_item2Buy.Pressed += () => OnItemBuyPressed(2);
		_cap1Buy.Pressed += () => OnCapsuleBuyPressed(1);
		_cap2Buy.Pressed += () => OnCapsuleBuyPressed(2);
		_houseRuleBuy.Pressed += OnHouseRuleBuyPressed;

		// award round end money before generating shop
		_gameState.AwardRoundEndMoney(_gameState.LeftoverBalls);

		GenerateShop();
		UpdateUI();
	}

	private void GenerateShop()
	{
		// first shop always has small totem capsule
		if (_gameState.CurrentRound == 2) // round incremented before shop
		{
			_capsule1Data = new CapsuleData(ItemType.Totem, CapsuleSize.Small);
		}
		else
		{
			_capsule1Data = new CapsuleData(ItemDatabase.GetRandomType(), ItemDatabase.GetRandomSize());
		}
		_capsule2Data = new CapsuleData(ItemDatabase.GetRandomType(), ItemDatabase.GetRandomSize());

		// generate item slots
		_item1Data = ItemDatabase.GetRandom(ItemDatabase.GetRandomType());
		_item2Data = ItemDatabase.GetRandom(ItemDatabase.GetRandomType());

		// house rule logic
		GenerateHouseRule();

		UpdateShopUI();
	}

	private void GenerateHouseRule()
	{
		bool isOddShop = (_gameState.CurrentRound % 2 == 0); // round incremented so round 2 = shop 1

		if (_gameState.PersistentHouseRule != null)
		{
			// carry over unsold rule
			_houseRuleData = ItemDatabase.HouseRules.Find(r => r.Name == _gameState.PersistentHouseRule);
			_houseRuleSlot.Visible = true;
			_houseRulePersist.Text = "RETURNING!";
		}
		else if (isOddShop)
		{
			// generate new house rule on odd shops
			_houseRuleData = ItemDatabase.GetRandom(ItemType.HouseRule);
			_gameState.PersistentHouseRule = _houseRuleData.Name;
			_houseRuleSlot.Visible = true;
			_houseRulePersist.Text = "";
		}
		else
		{
			// no house rule this shop
			_houseRuleSlot.Visible = false;
			_houseRuleData = null;
		}
	}

	private void UpdateShopUI()
	{
		// item slots
		_item1Name.Text = _item1Data.Name;
		_item1Type.Text = _item1Data.Type.ToString();
		_item1Cost.Text = $"${_item1Data.Cost}";

		_item2Name.Text = _item2Data.Name;
		_item2Type.Text = _item2Data.Type.ToString();
		_item2Cost.Text = $"${_item2Data.Cost}";

		// capsules
		_cap1Type.Text = _capsule1Data.Type.ToString();
		_cap1Size.Text = _capsule1Data.Size.ToString();
		_cap1Cost.Text = $"${_capsule1Data.Cost}";

		_cap2Type.Text = _capsule2Data.Type.ToString();
		_cap2Size.Text = _capsule2Data.Size.ToString();
		_cap2Cost.Text = $"${_capsule2Data.Cost}";

		// house rule
		if (_houseRuleData != null)
		{
			_houseRuleName.Text = _houseRuleData.Name;
			_houseRuleCost.Text = $"${_houseRuleData.Cost}";
		}
	}

	private void UpdateUI()
	{
		_moneyLabel.Text = $"${_gameState.Money}";
		_rerollCostLabel.Text = $"Reroll: ${_gameState.RerollCost}";
	}

	private void OnItemBuyPressed(int slot)
	{
		ItemData item = slot == 1 ? _item1Data : _item2Data;
		bool sold = slot == 1 ? _item1Sold : _item2Sold;

		if (sold || item == null) return;

		if (_gameState.SpendMoney(item.Cost))
		{
			AddItemToInventory(item);

			if (slot == 1)
			{
				_item1Sold = true;
				_item1Buy.Text = "SOLD";
				_item1Buy.Disabled = true;
			}
			else
			{
				_item2Sold = true;
				_item2Buy.Text = "SOLD";
				_item2Buy.Disabled = true;
			}

			UpdateUI();
		}
	}

	private void OnCapsuleBuyPressed(int slot)
	{
		CapsuleData capsule = slot == 1 ? _capsule1Data : _capsule2Data;
		bool sold = slot == 1 ? _cap1Sold : _cap2Sold;

		if (sold || capsule == null) return;

		if (_gameState.SpendMoney(capsule.Cost))
		{
			UpdateUI();

			// determine choice count and pick count based on size
			int choiceCount;
			int pickCount;
			switch (capsule.Size)
			{
				case CapsuleSize.Small:
					choiceCount = 2;
					pickCount = 1;
					break;
				case CapsuleSize.Medium:
					choiceCount = 4;
					pickCount = 1;
					break;
				case CapsuleSize.Large:
					choiceCount = 6;
					pickCount = 2;
					break;
				default:
					choiceCount = 2;
					pickCount = 1;
					break;
			}

			// generate unique random choices from the capsule's item type
			List<ItemData> choices = GetUniqueRandomItems(capsule.Type, choiceCount);

			// mark as sold immediately since money is spent
			if (slot == 1)
			{
				_cap1Sold = true;
				_cap1Buy.Text = "SOLD";
				_cap1Buy.Disabled = true;
			}
			else
			{
				_cap2Sold = true;
				_cap2Buy.Text = "SOLD";
				_cap2Buy.Disabled = true;
			}

			// open the picker
			CapsulePicker picker = GetNode<CapsulePicker>("CapsulePicker");
			picker.Open(choices, pickCount, (List<ItemData> picked) =>
			{
				foreach (ItemData item in picked)
					AddItemToInventory(item);
			});
		}
	}

	private List<ItemData> GetUniqueRandomItems(ItemType type, int count)
	{
		var sourceList = new List<ItemData>(ItemDatabase.GetListByType(type));
		var result = new List<ItemData>();

		count = Mathf.Min(count, sourceList.Count); // cant pick more than exist

		for (int i = 0; i < count; i++)
		{
			int randomIndex = (int)GD.RandRange(0, sourceList.Count - 1);
			result.Add(sourceList[randomIndex]);
			sourceList.RemoveAt(randomIndex);
		}

		return result;
	}

	private void OnHouseRuleBuyPressed()
	{
		if (_houseRuleSold || _houseRuleData == null) return;

		if (_gameState.SpendMoney(_houseRuleData.Cost))
		{
			_gameState.OwnedHouseRules.Add(_houseRuleData.Name);
			_gameState.PersistentHouseRule = null; // clear persistence on purchase
			_houseRuleSold = true;
			_houseRuleBuy.Text = "SOLD";
			_houseRuleBuy.Disabled = true;
			UpdateUI();
		}
	}

	private void OnRerollPressed()
	{
		if (!_gameState.SpendMoney(_gameState.RerollCost)) return;

		_gameState.RerollCost++;
		_item1Sold = false;
		_item2Sold = false;
		_cap1Sold = false;
		_cap2Sold = false;

		// regenerate items and capsules but keep house rule
		_item1Data = ItemDatabase.GetRandom(ItemDatabase.GetRandomType());
		_item2Data = ItemDatabase.GetRandom(ItemDatabase.GetRandomType());
		_capsule1Data = new CapsuleData(ItemDatabase.GetRandomType(), ItemDatabase.GetRandomSize());
		_capsule2Data = new CapsuleData(ItemDatabase.GetRandomType(), ItemDatabase.GetRandomSize());

		// re enable buy buttons
		_item1Buy.Text = "Buy";
		_item1Buy.Disabled = false;
		_item2Buy.Text = "Buy";
		_item2Buy.Disabled = false;
		_cap1Buy.Text = "Buy";
		_cap1Buy.Disabled = false;
		_cap2Buy.Text = "Buy";
		_cap2Buy.Disabled = false;

		UpdateShopUI();
		UpdateUI();
	}

	private void OnContinuePressed()
	{
		// reset reroll cost each shop visit
		_gameState.RerollCost = 3;
		GetTree().ChangeSceneToFile("res://scenes/game_scene.tscn");
	}

	private void AddItemToInventory(ItemData item)
	{
		switch (item.Type)
		{
			case ItemType.Totem:
				if (_gameState.OwnedTotems.Count < _gameState.MaxTotems)
				{
					_gameState.OwnedTotems.Add(new OwnedTotem(item));
					RefreshTotemPanel();
				}
				else
				{
					GD.Print("Totem slots full!");
					_gameState.AddMoney(item.Cost);
				}
				break;
			case ItemType.BallUpgrade:
			case ItemType.Engineering:
			case ItemType.Stunt:
				if (_gameState.OwnedItems.Count < _gameState.MaxItems)
				{
					_gameState.OwnedItems.Add(new OwnedItem(item));
					RefreshItemPanel();
				}
				else
				{
					GD.Print("Item slots full!");
					_gameState.AddMoney(item.Cost);
				}
				break;
		}
		GD.Print($"Bought: {item.Name}");
	}

	private void RefreshItemPanel()
	{
		ItemPanel panel = GetNodeOrNull<ItemPanel>("ItemPanel");
		panel?.RefreshUI();
	}
	
	private void RefreshTotemPanel()
	{
		TotemPanel panel = GetNodeOrNull<TotemPanel>("TotemPanel");
		panel?.RefreshUI();
	}
}
