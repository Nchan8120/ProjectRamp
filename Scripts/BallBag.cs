using Godot;
using System.Collections.Generic;

public partial class BallBag : Control
{
	[Export] public NodePath BallListPath;
	[Export] public NodePath CloseButtonPath;

	private VBoxContainer _ballList;
	private Button _closeButton;
	private GameState _gameState;
	private RoundManager _roundManager;

	private List<Panel> _slots = new List<Panel>();
	private int _selectedBallIndex = -1;
	private int _selectedItemIndex = -1;

	// drag state
	private bool _isDragging = false;
	private int _dragSourceIndex = -1;
	private Control _dragPreview;

	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");
		_ballList = GetNode<VBoxContainer>(BallListPath);
		_closeButton = GetNode<Button>(CloseButtonPath);

		_closeButton.Pressed += OnClosePressed;

		Visible = false;
		BuildBallList();
	}

	public void Open()
	{
		// block ball input while bag is open
		BallController ball = GetTree().Root.FindChild("Ball", true, false) as BallController;
		if (ball != null) ball.InputBlocked = true;
		BuildBallList();
		Visible = true;
	}
	
	public void SetRoundManager(RoundManager roundManager)
	{
		_roundManager = roundManager;
	}

	public void BuildBallList()
	{
		// clear existing slots
		foreach (Node child in _ballList.GetChildren())
			child.QueueFree();

		_slots.Clear();
		_selectedBallIndex = -1;

		for (int i = 0; i < _gameState.OwnedBalls.Count; i++)
		{
			OwnedBall ball = _gameState.OwnedBalls[i];
			bool isThrown = _roundManager != null && i < _roundManager.CurrentBallIndex;
			bool isActive = _roundManager != null && i == _roundManager.CurrentBallIndex;

			Panel slot = new Panel();
			slot.CustomMinimumSize = new Vector2(480, 60);

			// dim thrown balls
			if (isThrown)
				slot.SelfModulate = new Color(1f, 1f, 1f, 0.3f);
			else if (isActive)
				slot.SelfModulate = new Color(1f, 0.85f, 0.3f, 1f); // yellow

			Label numberLabel = new Label();
			numberLabel.Text = $"Ball {ball.BallNumber}";
			numberLabel.Position = new Vector2(8, 8);
			numberLabel.Size = new Vector2(100, 40);

			Label typeLabel = new Label();
			typeLabel.Text = ball.IsLocked 
				? $"{ball.UpgradeType} 🔒" 
				: ball.UpgradeType;
			typeLabel.Position = new Vector2(120, 8);
			typeLabel.Size = new Vector2(250, 40);
			
			

			slot.AddChild(numberLabel);
			slot.AddChild(typeLabel);
			_ballList.AddChild(slot);
			_slots.Add(slot);

			int index = i;

			// only allow interaction with balls that havent been thrown yet
			if (!isThrown)
			{
				slot.GuiInput += (inputEvent) => OnSlotInput(inputEvent, index);
			}
		}
	}

	private void OnSlotInput(InputEvent inputEvent, int slotIndex)
	{
		if (inputEvent is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			{
				// deselect previous ball
				if (_selectedBallIndex >= 0 && _selectedBallIndex < _slots.Count)
					_slots[_selectedBallIndex].SelfModulate = new Color(1f, 1f, 1f, 1f);

				if (_selectedBallIndex == slotIndex)
				{
					// deselect
					_selectedBallIndex = -1;
				}
				else
				{
					_selectedBallIndex = slotIndex;
					_slots[slotIndex].SelfModulate = new Color(1f, 0.85f, 0.3f, 1f); // yellow highlight
				}

				// notify item panel of selected ball
				NotifyItemPanel();
			}
		}

		// drag to reorder
		if (inputEvent is InputEventMouseMotion && Input.IsMouseButtonPressed(MouseButton.Left))
		{
			if (!_isDragging)
				StartDrag(slotIndex);
		}
	}

	private void NotifyItemPanel()
	{
		// tell item panel which ball is selected so it can show Use button
		var panels = GetTree().GetNodesInGroup("ItemPanel");
		foreach (Node node in panels)
		{
			if (node is ItemPanel itemPanel)
				itemPanel.SetSelectedBall(_selectedBallIndex);
		}
	}

	private void StartDrag(int sourceIndex)
	{
		_isDragging = true;
		_dragSourceIndex = sourceIndex;

		_dragPreview = new Label();
		((Label)_dragPreview).Text = $"Ball {_gameState.OwnedBalls[sourceIndex].BallNumber}";
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

		int targetIndex = GetSlotUnderMouse();

		// prevent swapping with thrown balls
		int activeIndex = _roundManager?.CurrentBallIndex ?? 0;
		bool targetIsThrown = targetIndex < activeIndex;
		bool sourceIsThrown = _dragSourceIndex < activeIndex;

		if (!targetIsThrown && !sourceIsThrown && targetIndex != _dragSourceIndex)
			SwapBalls(_dragSourceIndex, targetIndex);

		_dragSourceIndex = -1;
		BuildBallList();
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

	private void SwapBalls(int a, int b)
	{
		OwnedBall temp = _gameState.OwnedBalls[a];
		_gameState.OwnedBalls[a] = _gameState.OwnedBalls[b];
		_gameState.OwnedBalls[b] = temp;
	}

	public void ApplyUpgradeToBall(int ballIndex, OwnedItem upgrade)
	{
		if (ballIndex < 0 || ballIndex >= _gameState.OwnedBalls.Count) return;

		_gameState.OwnedBalls[ballIndex].UpgradeType = upgrade.Name;
		_selectedBallIndex = -1;

		NotifyItemPanel();
		BuildBallList();
	}

	private void OnClosePressed()
	{
		// restore ball input on close
		BallController ball = GetTree().Root.FindChild("Ball", true, false) as BallController;
		if (ball != null) ball.InputBlocked = false;
		_selectedBallIndex = -1;
		NotifyItemPanel();
		Visible = false;
	}
}
