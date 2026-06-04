using Godot;
using System.Collections.Generic;

public partial class TotemPanel : Control
{
	private GameState _gameState;
	private List<Panel> _slots = new List<Panel>();
	private List<Label> _nameLabels = new List<Label>();
	private List<Button> _sellButtons = new List<Button>();
	private int _selectedSlot = -1;

	// drag and drop state
	private bool _isDragging = false;
	private int _dragSourceSlot = -1;
	private Control _dragPreview;

	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");

		// collect slot references
		var container = GetNode<VBoxContainer>("SlotsContainer");
		for (int i = 0; i < 5; i++)
		{
			Panel slot = container.GetNode<Panel>($"TotemSlot{i + 1}");
			Label nameLabel = slot.GetNode<Label>("TotemName");
			Button sellButton = slot.GetNode<Button>("SellButton");

			_slots.Add(slot);
			_nameLabels.Add(nameLabel);
			_sellButtons.Add(sellButton);

			// capture index for lambda
			int index = i;
			sellButton.Pressed += () => OnSellPressed(index);

			// enable input on each slot for click and drag
			slot.GuiInput += (inputEvent) => OnSlotInput(inputEvent, index);
		}

		RefreshUI();
	}

	public void RefreshUI()
	{
		for (int i = 0; i < 5; i++)
		{
			// check for null explicitly
			bool hasTotem = i < _gameState.OwnedTotems.Count 
							&& _gameState.OwnedTotems[i] != null;

			if (hasTotem)
			{
				OwnedTotem totem = _gameState.OwnedTotems[i];
				_nameLabels[i].Text = totem.Name;
				_slots[i].SelfModulate = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				_nameLabels[i].Text = "[ empty ]";
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
				bool hasTotem = slotIndex < _gameState.OwnedTotems.Count;

				if (hasTotem)
				{
					// toggle selection
					if (_selectedSlot == slotIndex)
					{
						// deselect
						_selectedSlot = -1;
						_sellButtons[slotIndex].Visible = false;
					}
					else
					{
						// deselect previous
						if (_selectedSlot >= 0)
							_sellButtons[_selectedSlot].Visible = false;

						// select new
						_selectedSlot = slotIndex;
						OwnedTotem totem = _gameState.OwnedTotems[slotIndex];
						_sellButtons[slotIndex].Text = $"Sell ${totem.SellPrice}";
						_sellButtons[slotIndex].Visible = true;
					}
				}
			}
		}

		// drag start
		if (inputEvent is InputEventMouseMotion && Input.IsMouseButtonPressed(MouseButton.Left))
		{
			bool hasTotem = slotIndex < _gameState.OwnedTotems.Count;
			if (hasTotem && !_isDragging)
			{
				StartDrag(slotIndex);
			}
		}
	}

	private void StartDrag(int sourceSlot)
	{
		_isDragging = true;
		_dragSourceSlot = sourceSlot;

		// create a visual drag preview label
		_dragPreview = new Label();
		((Label)_dragPreview).Text = _gameState.OwnedTotems[sourceSlot].Name;
		_dragPreview.Size = new Vector2(160, 40);
		AddChild(_dragPreview);
	}

	public override void _Process(double delta)
	{
		// move drag preview with mouse
		if (_isDragging && _dragPreview != null)
		{
			_dragPreview.GlobalPosition = GetGlobalMousePosition() - new Vector2(80, 20);
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		// drop on mouse release
		if (_isDragging && inputEvent is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && !mouseButton.Pressed)
			{
				FinishDrag();
			}
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

		// find which slot the mouse is over
		int targetSlot = GetSlotUnderMouse();

		if (targetSlot >= 0 && targetSlot != _dragSourceSlot)
		{
			SwapSlots(_dragSourceSlot, targetSlot);
		}

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
		// pad list to max index needed with empty slots
		while (_gameState.OwnedTotems.Count <= Mathf.Max(a, b))
			_gameState.OwnedTotems.Add(null);

		OwnedTotem temp = _gameState.OwnedTotems[a];
		_gameState.OwnedTotems[a] = _gameState.OwnedTotems[b];
		_gameState.OwnedTotems[b] = temp;

		// clean up trailing nulls after swap
		while (_gameState.OwnedTotems.Count > 0 &&
			   _gameState.OwnedTotems[_gameState.OwnedTotems.Count - 1] == null)
			_gameState.OwnedTotems.RemoveAt(_gameState.OwnedTotems.Count - 1);
	}

	private void OnSellPressed(int slotIndex)
	{
		if (slotIndex >= _gameState.OwnedTotems.Count) return;

		OwnedTotem totem = _gameState.OwnedTotems[slotIndex];
		_gameState.AddMoney(totem.SellPrice);
		_gameState.OwnedTotems.RemoveAt(slotIndex);

		GD.Print($"Sold {totem.Name} for ${totem.SellPrice}");
		RefreshUI();
	}
}
