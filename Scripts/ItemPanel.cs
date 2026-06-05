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

	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");

		var container = GetNode<VBoxContainer>("SlotsContainer");
		for (int i = 0; i < 3; i++)
		{
			Panel slot = container.GetNode<Panel>($"ItemSlot{i + 1}");
			Label nameLabel = slot.GetNode<Label>("ItemName");
			Label typeLabel = slot.GetNode<Label>("ItemType");
			Button sellButton = slot.GetNode<Button>("SellButton");

			_slots.Add(slot);
			_nameLabels.Add(nameLabel);
			_typeLabels.Add(typeLabel);
			_sellButtons.Add(sellButton);

			int index = i;
			sellButton.Pressed += () => OnSellPressed(index);
			slot.GuiInput += (inputEvent) => OnSlotInput(inputEvent, index);
		}

		RefreshUI();
	}

	public void RefreshUI()
	{
		for (int i = 0; i < 3; i++)
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
		RefreshUI();
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

		GD.Print($"Sold {item.Name} for ${item.SellPrice}");
		RefreshUI();
	}
}
