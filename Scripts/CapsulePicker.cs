using Godot;
using System.Collections.Generic;

public partial class CapsulePicker : Control
{
	private GameState _gameState;
	private List<Panel> _slots = new List<Panel>();
	private List<Label> _nameLabels = new List<Label>();
	private List<Label> _descLabels = new List<Label>();
	private List<Button> _selectButtons = new List<Button>();
	private Button _confirmButton;
	private Button _skipButton;

	private List<ItemData> _choices = new List<ItemData>();
	private int _pickCount = 1; // 1 for small/medium, 2 for large
	private List<int> _selectedIndices = new List<int>();

	// callback to run when picking is done
	private System.Action<List<ItemData>> _onComplete;

	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");

		var container = GetNode<HBoxContainer>("ChoicesContainer");
		for (int i = 0; i < 6; i++)
		{
			Panel slot = container.GetNode<Panel>($"ChoiceSlot{i + 1}");
			Label nameLabel = slot.GetNode<Label>("NameLabel");
			Label descLabel = slot.GetNode<Label>("DescLabel");
			Button selectButton = slot.GetNode<Button>("SelectButton");

			_slots.Add(slot);
			_nameLabels.Add(nameLabel);
			_descLabels.Add(descLabel);
			_selectButtons.Add(selectButton);

			int index = i;
			selectButton.Pressed += () => OnSelectPressed(index);
		}

		_confirmButton = GetNode<Button>("ConfirmButton");
		_skipButton = GetNode<Button>("SkipButton");

		_confirmButton.Pressed += OnConfirmPressed;
		_skipButton.Pressed += OnSkipPressed;
	}

	// call this to open the picker with a set of choices
	public void Open(List<ItemData> choices, int pickCount, System.Action<List<ItemData>> onComplete)
	{
		_choices = choices;
		_pickCount = pickCount;
		_selectedIndices.Clear();
		_onComplete = onComplete;

		_confirmButton.Visible = false;

		// populate slots
		for (int i = 0; i < 6; i++)
		{
			if (i < _choices.Count)
			{
				_slots[i].Visible = true;
				_nameLabels[i].Text = _choices[i].Name;
				_descLabels[i].Text = _choices[i].Description;
				_selectButtons[i].Text = "Select";
				_selectButtons[i].Disabled = false;
				_slots[i].SelfModulate = new Color(1, 1, 1, 1);
			}
			else
			{
				_slots[i].Visible = false;
			}
		}

		Visible = true;
	}

	private void OnSelectPressed(int index)
	{
		if (_selectedIndices.Contains(index))
		{
			// deselect
			_selectedIndices.Remove(index);
			_selectButtons[index].Text = "Select";
			_slots[index].SelfModulate = new Color(1, 1, 1, 1);
		}
		else
		{
			if (_selectedIndices.Count >= _pickCount)
			{
				// already at max selections, ignore
				return;
			}

			_selectedIndices.Add(index);
			_selectButtons[index].Text = "Selected";
			_slots[index].SelfModulate = new Color(0.6f, 1f, 0.6f, 1f); // greenish highlight
		}

		// show confirm button if at least 1 item is selected
		_confirmButton.Visible = _selectedIndices.Count >= 1;
	}

	private void OnConfirmPressed()
	{
		List<ItemData> picked = new List<ItemData>();
		foreach (int index in _selectedIndices)
			picked.Add(_choices[index]);

		Visible = false;
		_onComplete?.Invoke(picked);
	}

	private void OnSkipPressed()
	{
		Visible = false;
		_onComplete?.Invoke(new List<ItemData>()); // empty list = nothing picked
	}
}
