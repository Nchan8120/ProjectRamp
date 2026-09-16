using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class DevConsole : Control
{
	private GameState _gameState;
	private TotemManager _totemManager;
	private bool _isOpen = false;

	// totem tab
	private LineEdit _totemSearch;
	private VBoxContainer _totemList;

	// ball upgrade tab
	private LineEdit _ballSearch;
	private VBoxContainer _ballList;

	// item tab
	private LineEdit _itemSearch;
	private VBoxContainer _itemList;

	// other tab
	private HSlider _moneySlider;
	private Label _moneyLabel;

	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");
		_totemManager = GetNode<TotemManager>("/root/TotemManager");

		// dim background
		ColorRect dim = new ColorRect();
		dim.Color = new Color(0, 0, 0, 0.7f);
		dim.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		dim.MouseFilter = MouseFilterEnum.Ignore;
		AddChild(dim);

		// main panel
		Panel panel = new Panel();
		panel.Position = new Vector2(176, 60);
		panel.Size = new Vector2(800, 528);
		AddChild(panel);

		// title
		Label title = new Label();
		title.Text = "DEV CONSOLE";
		title.Position = new Vector2(20, 10);
		title.AddThemeFontSizeOverride("font_size", 20);
		panel.AddChild(title);

		// close button
		Button closeBtn = new Button();
		closeBtn.Text = "X";
		closeBtn.Position = new Vector2(750, 10);
		closeBtn.Size = new Vector2(40, 40);
		closeBtn.Pressed += Close;
		panel.AddChild(closeBtn);

		// tab container
		TabContainer tabs = new TabContainer();
		tabs.Position = new Vector2(10, 60);
		tabs.Size = new Vector2(780, 458);
		panel.AddChild(tabs);

		// build each tab
		tabs.AddChild(BuildSearchTab("Totems", out _totemSearch, out _totemList));
		tabs.AddChild(BuildSearchTab("Ball Upgrades", out _ballSearch, out _ballList));
		tabs.AddChild(BuildSearchTab("Items", out _itemSearch, out _itemList));
		tabs.AddChild(BuildOtherTab());

		// connect search boxes
		_totemSearch.TextChanged += _ => PopulateTotemList();
		_ballSearch.TextChanged += _ => PopulateBallList();
		_itemSearch.TextChanged += _ => PopulateItemList();

		Visible = false;
	}

	private Control BuildSearchTab(string tabName, out LineEdit searchBox, out VBoxContainer list)
	{
		VBoxContainer tab = new VBoxContainer();
		tab.Name = tabName;

		LineEdit search = new LineEdit();
		search.PlaceholderText = "Search...";
		search.CustomMinimumSize = new Vector2(760, 40);
		tab.AddChild(search);

		ScrollContainer scroll = new ScrollContainer();
		scroll.CustomMinimumSize = new Vector2(760, 400);
		tab.AddChild(scroll);

		VBoxContainer itemList = new VBoxContainer();
		scroll.AddChild(itemList);

		searchBox = search;
		list = itemList;
		return tab;
	}

	private Control BuildOtherTab()
	{
		VBoxContainer tab = new VBoxContainer();
		tab.Name = "Other";
		tab.AddThemeConstantOverride("separation", 10);

		Label moneyTitle = new Label();
		moneyTitle.Text = "Add Money:";
		tab.AddChild(moneyTitle);

		_moneyLabel = new Label();
		_moneyLabel.Text = "$50";
		tab.AddChild(_moneyLabel);

		_moneySlider = new HSlider();
		_moneySlider.MinValue = 0;
		_moneySlider.MaxValue = 500;
		_moneySlider.Value = 50;
		_moneySlider.CustomMinimumSize = new Vector2(760, 40);
		_moneySlider.ValueChanged += OnMoneySliderChanged;
		tab.AddChild(_moneySlider);

		Button addMoneyBtn = new Button();
		addMoneyBtn.Text = "Add Money";
		addMoneyBtn.CustomMinimumSize = new Vector2(200, 40);
		addMoneyBtn.Pressed += OnAddMoneyPressed;
		tab.AddChild(addMoneyBtn);

		return tab;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode == Key.Backslash)
			{
				if (_isOpen)
					Close();
				else
					Open();
			}

			if (_isOpen && keyEvent.Keycode == Key.Escape)
				Close();
		}
	}

	private void Open()
	{
		_isOpen = true;
		Visible = true;
		PopulateTotemList();
		PopulateBallList();
		PopulateItemList();
	}

	private void Close()
	{
		_isOpen = false;
		Visible = false;
	}

	// --- TOTEMS ---

	private void PopulateTotemList()
	{
		foreach (Node child in _totemList.GetChildren())
			child.QueueFree();

		string search = _totemSearch.Text.ToLower();

		foreach (TotemData totem in TotemDatabase.AllTotems)
		{
			if (!string.IsNullOrEmpty(search) && !totem.Name.ToLower().Contains(search))
				continue;

			Button btn = new Button();
			btn.Text = $"{totem.Name} [{totem.Rarity}] - {totem.Description}";
			btn.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			btn.CustomMinimumSize = new Vector2(740, 0);
			btn.Alignment = HorizontalAlignment.Left;

			TotemData captured = totem;
			btn.Pressed += () => AddTotem(captured);

			_totemList.AddChild(btn);
		}
	}

	private void AddTotem(TotemData data)
	{
		if (_gameState.OwnedTotems.Count >= _gameState.MaxTotems)
		{
			GD.Print("DEV: Totem slots full");
			return;
		}

		OwnedTotem totem = new OwnedTotem(data);
		_gameState.OwnedTotems.Add(totem);
		_totemManager.OnTotemAdded(totem);

		var panels = GetTree().GetNodesInGroup("TotemPanel");
		foreach (Node node in panels)
			if (node is TotemPanel tp) tp.RefreshUI();

		GD.Print($"DEV: Added totem {data.Name}");
	}

	// --- BALL UPGRADES ---

	private void PopulateBallList()
	{
		foreach (Node child in _ballList.GetChildren())
			child.QueueFree();

		string search = _ballSearch.Text.ToLower();

		// get all ball upgrades from ItemDatabase
		foreach (ItemData item in ItemDatabase.BallUpgrades)
		{
			if (!string.IsNullOrEmpty(search) && !item.Name.ToLower().Contains(search))
				continue;

			Button btn = new Button();
			btn.Text = $"{item.Name} - {item.Description}";
			btn.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			btn.CustomMinimumSize = new Vector2(740, 0);
			btn.Alignment = HorizontalAlignment.Left;

			ItemData captured = item;
			btn.Pressed += () => AddBallUpgrade(captured);

			_ballList.AddChild(btn);
		}
	}

	private void AddBallUpgrade(ItemData item)
	{
		if (_gameState.OwnedItems.Count >= _gameState.MaxItems)
		{
			GD.Print("DEV: Item slots full");
			return;
		}

		_gameState.OwnedItems.Add(new OwnedItem(item));

		var panels = GetTree().GetNodesInGroup("ItemPanel");
		foreach (Node node in panels)
			if (node is ItemPanel ip) ip.RefreshUI();

		GD.Print($"DEV: Added ball upgrade {item.Name}");
	}

	// --- ITEMS (Stunts, Engineering, House Rules) ---

	private void PopulateItemList()
	{
		foreach (Node child in _itemList.GetChildren())
			child.QueueFree();

		string search = _itemSearch.Text.ToLower();

		// combine stunts and engineering
		List<ItemData> allItems = new List<ItemData>();
		allItems.AddRange(ItemDatabase.Stunts);
		allItems.AddRange(ItemDatabase.Engineering);

		foreach (ItemData item in allItems)
		{
			if (!string.IsNullOrEmpty(search) && !item.Name.ToLower().Contains(search))
				continue;

			Button btn = new Button();
			btn.Text = $"[{item.Type}] {item.Name} - {item.Description}";
			btn.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			btn.CustomMinimumSize = new Vector2(740, 0);
			btn.Alignment = HorizontalAlignment.Left;

			ItemData captured = item;
			btn.Pressed += () => AddItem(captured);

			_itemList.AddChild(btn);
		}

		// house rules separately
		foreach (HouseRuleData rule in HouseRuleDatabase.AllHouseRules)
		{
			if (!string.IsNullOrEmpty(search) && !rule.Name.ToLower().Contains(search))
				continue;

			Button btn = new Button();
			btn.Text = $"[HouseRule] {rule.Name} - {rule.Description}";
			btn.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			btn.CustomMinimumSize = new Vector2(740, 0);
			btn.Alignment = HorizontalAlignment.Left;

			HouseRuleData captured = rule;
			btn.Pressed += () => AddHouseRule(captured);

			_itemList.AddChild(btn);
		}
	}

	private void AddItem(ItemData item)
	{
		if (_gameState.OwnedItems.Count >= _gameState.MaxItems)
		{
			GD.Print("DEV: Item slots full");
			return;
		}

		_gameState.OwnedItems.Add(new OwnedItem(item));

		var panels = GetTree().GetNodesInGroup("ItemPanel");
		foreach (Node node in panels)
			if (node is ItemPanel ip) ip.RefreshUI();

		GD.Print($"DEV: Added item {item.Name}");
	}

	private void AddHouseRule(HouseRuleData rule)
	{
		HouseRuleEffect effect = rule.CreateEffect();
		effect.Initialize(_gameState);
		effect.OnPurchased();
		_gameState.OwnedHouseRules.Add(rule.Name);

		var totemPanels = GetTree().GetNodesInGroup("TotemPanel");
		foreach (Node node in totemPanels)
			if (node is TotemPanel tp) tp.RefreshUI();

		var itemPanels = GetTree().GetNodesInGroup("ItemPanel");
		foreach (Node node in itemPanels)
			if (node is ItemPanel ip) ip.RefreshUI();

		GD.Print($"DEV: Added house rule {rule.Name}");
	}

	// --- OTHER ---

	private void OnMoneySliderChanged(double value)
	{
		_moneyLabel.Text = $"${(int)value}";
	}

	private void OnAddMoneyPressed()
	{
		int amount = (int)_moneySlider.Value;
		_gameState.AddMoney(amount);

		Label moneyLabel = GetTree().Root.FindChild("MoneyLabel", true, false) as Label;
		if (moneyLabel != null)
			moneyLabel.Text = $"${_gameState.Money}";

		GD.Print($"DEV: Added ${amount}");
	}
}
