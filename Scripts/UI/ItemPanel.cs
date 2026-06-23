using Godot;
using System.Collections.Generic;

public partial class ItemPanel : Control
{
	private GameState _gameState;
	private List<Panel> _slots = new List<Panel>();
	private List<Label> _nameLabels = new List<Label>();
	private List<Label> _typeLabels = new List<Label>();
	private List<Button> _sellButtons = new List<Button>();
	private int _selectedSlot = -1;
	private bool _isDragging = false;
	private int _dragSourceSlot = -1;
	private Control _dragPreview;
	private VBoxContainer _slotsContainer;
	private int _selectedBallIndex = -1;
	private List<Button> _useButtons = new List<Button>();

	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");
		AddToGroup("ItemPanel");
		_slotsContainer = GetNode<VBoxContainer>("SlotsContainer");
		BuildSlots();
	}

	private void BuildSlots()
	{
		// clear existing slots
		foreach (Node child in _slotsContainer.GetChildren())
			child.QueueFree();

		_slots.Clear();
		_nameLabels.Clear();
		_typeLabels.Clear();
		_sellButtons.Clear();
		_useButtons.Clear();
		_selectedSlot = -1;
		
		 // show at least MaxItems slots, but more if currently overflowing
		int slotCount = Mathf.Max(_gameState.MaxItems, _gameState.OwnedItems.Count);

		// build slots dynamically based on MaxItems
		for (int i = 0; i < slotCount; i++)
		{
			Panel slot = new Panel();
			slot.CustomMinimumSize = new Vector2(120, 90);

			Label nameLabel = new Label();
			nameLabel.Position = new Vector2(6, 4);
			nameLabel.Size = new Vector2(108, 18);
			nameLabel.AutowrapMode = TextServer.AutowrapMode.Word;

			Label typeLabel = new Label();
			typeLabel.Position = new Vector2(6, 24);
			typeLabel.Size = new Vector2(108, 16);

			Button sellButton = new Button();
			sellButton.Position = new Vector2(62, 56);
			sellButton.Size = new Vector2(50, 28);
			sellButton.Visible = false;
			
			Button useButton = new Button();
			useButton.Text = "Use";
			useButton.Position = new Vector2(6, 56);
			useButton.Size = new Vector2(50, 28);
			useButton.Visible = false;

			slot.AddChild(nameLabel);
			slot.AddChild(typeLabel);
			slot.AddChild(sellButton);
			slot.AddChild(useButton);
			_slotsContainer.AddChild(slot);

			_slots.Add(slot);
			_nameLabels.Add(nameLabel);
			_typeLabels.Add(typeLabel);
			_sellButtons.Add(sellButton);
			_useButtons.Add(useButton);

			// capture index for lambda
			int index = i;
			sellButton.Pressed += () => OnSellPressed(index);
			useButton.Pressed += () => OnUsePressed(index);
			slot.GuiInput += (inputEvent) => OnSlotInput(inputEvent, index);
		}

		// populate with current data
		PopulateSlots();
	}

	private void PopulateSlots()
	{
		for (int i = 0; i < _slots.Count; i++)
		{
			bool hasItem = i < _gameState.OwnedItems.Count
						   && _gameState.OwnedItems[i] != null;

			if (hasItem)
			{
				OwnedItem item = _gameState.OwnedItems[i];
				_nameLabels[i].Text = item.Name;
				_typeLabels[i].Text = item.Type.ToString();
				_slots[i].SelfModulate = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				_nameLabels[i].Text = "[ empty ]";
				_typeLabels[i].Text = "";
				_slots[i].SelfModulate = new Color(1f, 1f, 1f, 0.4f);
			}

			_sellButtons[i].Visible = false;
		}

		_selectedSlot = -1;
	}

	public void RefreshUI()
	{
		int neededSlots = Mathf.Max(_gameState.MaxItems, _gameState.OwnedItems.Count);
		// rebuild slots if count changed (e.g. Juggler bought/sold)
		if (_slots.Count != _gameState.MaxItems)
			BuildSlots();
		else
			PopulateSlots();
	}

	private void OnSlotInput(InputEvent inputEvent, int slotIndex)
	{
		if (inputEvent is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				bool hasItem = slotIndex < _gameState.OwnedItems.Count
							   && _gameState.OwnedItems[slotIndex] != null;

				if (hasItem)
				{
					if (_selectedSlot == slotIndex)
					{
						_selectedSlot = -1;
						_sellButtons[slotIndex].Visible = false;
					}
					else
					{
						if (_selectedSlot >= 0)
							_sellButtons[_selectedSlot].Visible = false;

						_selectedSlot = slotIndex;
						OwnedItem item = _gameState.OwnedItems[slotIndex];
						_sellButtons[slotIndex].Text = $"Sell ${item.SellPrice}";
						_sellButtons[slotIndex].Visible = true;
					}
				}
			}
		}

		if (inputEvent is InputEventMouseMotion && Input.IsMouseButtonPressed(MouseButton.Left))
		{
			bool hasItem = slotIndex < _gameState.OwnedItems.Count
						   && _gameState.OwnedItems[slotIndex] != null;
			if (hasItem && !_isDragging)
				StartDrag(slotIndex);
		}
	}

	private void StartDrag(int sourceSlot)
	{
		_isDragging = true;
		_dragSourceSlot = sourceSlot;

		_dragPreview = new Label();
		((Label)_dragPreview).Text = _gameState.OwnedItems[sourceSlot].Name;
		_dragPreview.Size = new Vector2(160, 40);
		AddChild(_dragPreview);
	}

	public override void _Process(double delta)
	{
		if (_isDragging && _dragPreview != null)
			_dragPreview.GlobalPosition = GetGlobalMousePosition() - new Vector2(80, 20);
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (_isDragging && inputEvent is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && !mouseButton.Pressed)
				FinishDrag();
		}
	}

	private void FinishDrag()
	{
		_isDragging = false;

		if (_dragPreview != null)
		{
			_dragPreview.QueueFree();
			_dragPreview = null;
		}

		int targetSlot = GetSlotUnderMouse();

		if (targetSlot >= 0 && targetSlot != _dragSourceSlot)
			SwapSlots(_dragSourceSlot, targetSlot);

		_dragSourceSlot = -1;
		PopulateSlots();
	}

	private int GetSlotUnderMouse()
	{
		Vector2 mousePos = GetGlobalMousePosition();

		for (int i = 0; i < _slots.Count; i++)
		{
			Rect2 slotRect = new Rect2(_slots[i].GlobalPosition, _slots[i].Size);
			if (slotRect.HasPoint(mousePos))
				return i;
		}

		return -1;
	}

	private void SwapSlots(int a, int b)
	{
		while (_gameState.OwnedItems.Count <= Mathf.Max(a, b))
			_gameState.OwnedItems.Add(null);

		OwnedItem temp = _gameState.OwnedItems[a];
		_gameState.OwnedItems[a] = _gameState.OwnedItems[b];
		_gameState.OwnedItems[b] = temp;

		while (_gameState.OwnedItems.Count > 0 &&
			   _gameState.OwnedItems[_gameState.OwnedItems.Count - 1] == null)
			_gameState.OwnedItems.RemoveAt(_gameState.OwnedItems.Count - 1);
	}

	private void OnSellPressed(int slotIndex)
	{
		if (slotIndex >= _gameState.OwnedItems.Count) return;
		if (_gameState.OwnedItems[slotIndex] == null) return;

		OwnedItem item = _gameState.OwnedItems[slotIndex];
		_gameState.AddMoney(item.SellPrice);
		_gameState.OwnedItems.RemoveAt(slotIndex);

		RefreshMoneyLabel();

		CapsulePicker picker = GetTree().Root.FindChild("CapsulePicker", true, false) as CapsulePicker;
		picker?.RefreshConfirmButton();

		GD.Print($"Sold {item.Name} for ${item.SellPrice}");
		RefreshUI();
	}
	
	private void OnUsePressed(int slotIndex)
	{
		if (_selectedBallIndex < 0) return;
		if (slotIndex >= _gameState.OwnedItems.Count) return;
		if (_gameState.OwnedItems[slotIndex] == null) return;

		OwnedItem upgrade = _gameState.OwnedItems[slotIndex];
		
		OwnedBall targetBall = _gameState.OwnedBalls[_selectedBallIndex];

		// block upgrades on locked balls
		if (targetBall.IsLocked)
		{
			GD.Print("This ball cannot be modified!");
			return;
		}
		
		if (upgrade.Type != ItemType.BallUpgrade) return;

		// find ball bag and apply upgrade
		BallBag ballBag = GetTree().Root.FindChild("BallBag", true, false) as BallBag;
		ballBag?.ApplyUpgradeToBall(_selectedBallIndex, upgrade);

		// consume the item
		_gameState.OwnedItems.RemoveAt(slotIndex);
		_selectedBallIndex = -1;

		GD.Print($"Applied {upgrade.Name} to ball {_selectedBallIndex + 1}");
		RefreshUI();
	}

	private void RefreshMoneyLabel()
	{
		Label moneyLabel = GetTree().Root.FindChild("MoneyLabel", true, false) as Label;
		if (moneyLabel != null)
			moneyLabel.Text = $"${_gameState.Money}";
	}
	public void SetSelectedBall(int ballIndex)
{
	_selectedBallIndex = ballIndex;
	UpdateUseButtons();
}

	private void UpdateUseButtons()
	{
		for (int i = 0; i < _slots.Count; i++)
		{
			bool hasItem = i < _gameState.OwnedItems.Count && _gameState.OwnedItems[i] != null;
			bool isBallUpgrade = hasItem && _gameState.OwnedItems[i].Type == ItemType.BallUpgrade;
			bool ballSelected = _selectedBallIndex >= 0;

			if (_useButtons.Count > i)
				_useButtons[i].Visible = isBallUpgrade && ballSelected;
		}
	}
}
