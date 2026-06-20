using Godot;
using System.Collections.Generic;

public partial class TotemPanel : Control
{
	private GameState _gameState;
	private List<Panel> _slots = new List<Panel>();
	private List<Label> _nameLabels = new List<Label>();
	private List<Label> _valueLabels = new List<Label>();
	private List<Button> _sellButtons = new List<Button>();
	private int _selectedSlot = -1;

	private bool _isDragging = false;
	private int _dragSourceSlot = -1;
	private Control _dragPreview;
	private VBoxContainer _slotsContainer;

	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");
		AddToGroup("TotemPanel");
		_slotsContainer = GetNode<VBoxContainer>("SlotsContainer");
		BuildSlots();
	}

	private void BuildSlots()
	{
		foreach (Node child in _slotsContainer.GetChildren())
			child.QueueFree();

		_slots.Clear();
		_nameLabels.Clear();
		_valueLabels.Clear();
		_sellButtons.Clear();
		_selectedSlot = -1;

		for (int i = 0; i < _gameState.MaxTotems; i++)
		{
			Panel slot = new Panel();
			slot.CustomMinimumSize = new Vector2(180, 90);

			Label nameLabel = new Label();
			nameLabel.Position = new Vector2(8, 8);
			nameLabel.Size = new Vector2(164, 20);
			nameLabel.AutowrapMode = TextServer.AutowrapMode.Word;

			Label valueLabel = new Label();
			valueLabel.Position = new Vector2(8, 30);
			valueLabel.Size = new Vector2(164, 18);
			// make the value stand out a bit
			valueLabel.AddThemeColorOverride("font_color", new Color(1f, 0.9f, 0.4f));

			Button sellButton = new Button();
			sellButton.Position = new Vector2(8, 56);
			sellButton.Size = new Vector2(164, 28);
			sellButton.Visible = false;

			slot.AddChild(nameLabel);
			slot.AddChild(valueLabel);
			slot.AddChild(sellButton);
			_slotsContainer.AddChild(slot);

			_slots.Add(slot);
			_nameLabels.Add(nameLabel);
			_valueLabels.Add(valueLabel);
			_sellButtons.Add(sellButton);

			int index = i;
			sellButton.Pressed += () => OnSellPressed(index);
			slot.GuiInput += (inputEvent) => OnSlotInput(inputEvent, index);
		}

		PopulateSlots();
	}

	private void PopulateSlots()
	{
		for (int i = 0; i < _slots.Count; i++)
		{
			bool hasTotem = i < _gameState.OwnedTotems.Count
							&& _gameState.OwnedTotems[i] != null;

			if (hasTotem)
			{
				OwnedTotem totem = _gameState.OwnedTotems[i];
				_nameLabels[i].Text = totem.Name;
				_slots[i].SelfModulate = GetRarityColor(totem.Rarity);

				// pull live display value from the effect if it has one
				string displayValue = totem.Effect?.GetDisplayValue();
				_valueLabels[i].Text = displayValue ?? "";
				_valueLabels[i].Visible = displayValue != null;
			}
			else
			{
				_nameLabels[i].Text = "[ empty ]";
				_valueLabels[i].Text = "";
				_valueLabels[i].Visible = false;
				_slots[i].SelfModulate = new Color(1f, 1f, 1f, 0.4f);
			}

			_sellButtons[i].Visible = false;
		}

		_selectedSlot = -1;
	}

	public void RefreshUI()
	{
		if (_slots.Count != _gameState.MaxTotems)
			BuildSlots();
		else
			PopulateSlots();
	}

	private Color GetRarityColor(TotemRarity rarity)
	{
		return rarity switch
		{
			TotemRarity.Common => new Color(0.8f, 0.8f, 0.8f),
			TotemRarity.Rare => new Color(0.4f, 0.6f, 1f),
			TotemRarity.Epic => new Color(0.7f, 0.3f, 1f),
			TotemRarity.Legendary => new Color(1f, 0.8f, 0.2f),
			_ => new Color(1f, 1f, 1f)
		};
	}

	private void OnSlotInput(InputEvent inputEvent, int slotIndex)
	{
		if (inputEvent is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				bool hasTotem = slotIndex < _gameState.OwnedTotems.Count
								&& _gameState.OwnedTotems[slotIndex] != null;

				if (hasTotem)
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
						OwnedTotem totem = _gameState.OwnedTotems[slotIndex];
						_sellButtons[slotIndex].Text = $"Sell ${totem.SellPrice}";
						_sellButtons[slotIndex].Visible = true;
					}
				}
			}
		}

		if (inputEvent is InputEventMouseMotion && Input.IsMouseButtonPressed(MouseButton.Left))
		{
			bool hasTotem = slotIndex < _gameState.OwnedTotems.Count
							&& _gameState.OwnedTotems[slotIndex] != null;
			if (hasTotem && !_isDragging)
				StartDrag(slotIndex);
		}
	}

	private void StartDrag(int sourceSlot)
	{
		_isDragging = true;
		_dragSourceSlot = sourceSlot;

		_dragPreview = new Label();
		((Label)_dragPreview).Text = _gameState.OwnedTotems[sourceSlot].Name;
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
		{
			SwapSlots(_dragSourceSlot, targetSlot);
			GetNode<TotemManager>("/root/TotemManager").BroadcastTotemMoved();
		}

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
		while (_gameState.OwnedTotems.Count <= Mathf.Max(a, b))
			_gameState.OwnedTotems.Add(null);

		OwnedTotem temp = _gameState.OwnedTotems[a];
		_gameState.OwnedTotems[a] = _gameState.OwnedTotems[b];
		_gameState.OwnedTotems[b] = temp;

		while (_gameState.OwnedTotems.Count > 0 &&
			   _gameState.OwnedTotems[_gameState.OwnedTotems.Count - 1] == null)
			_gameState.OwnedTotems.RemoveAt(_gameState.OwnedTotems.Count - 1);
	}

	private void OnSellPressed(int slotIndex)
	{
		if (slotIndex >= _gameState.OwnedTotems.Count) return;
		if (_gameState.OwnedTotems[slotIndex] == null) return;

		OwnedTotem totem = _gameState.OwnedTotems[slotIndex];
		_gameState.AddMoney(totem.SellPrice);
		_gameState.OwnedTotems.RemoveAt(slotIndex);
		RefreshMoneyLabel();
		
		GetNode<TotemManager>("/root/TotemManager").OnTotemRemoved();

		GD.Print($"Sold {totem.Name} for ${totem.SellPrice}");
		RefreshUI();
		
		// also refresh the item panel in case MaxItems changed
		ItemPanel itemPanel = GetTree().Root.FindChild("ItemPanel", true, false) as ItemPanel;
		itemPanel?.RefreshUI();

		CapsulePicker picker = GetTree().Root.FindChild("CapsulePicker", true, false) as CapsulePicker;
		picker?.RefreshConfirmButton();
	}

	private void RefreshMoneyLabel()
	{
		Label moneyLabel = GetTree().Root.FindChild("MoneyLabel", true, false) as Label;
		if (moneyLabel != null)
			moneyLabel.Text = $"${_gameState.Money}";
	}
}
