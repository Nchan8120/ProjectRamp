using Godot;
using System;
using System.Collections.Generic;

public partial class MachineSelect : Control
{
	[Export] public NodePath ArrowLeftPath;
	[Export] public NodePath ArrowRightPath;
	[Export] public NodePath MachineNamePath;
	[Export] public NodePath MachineDescriptionPath;
	[Export] public NodePath LockLabelPath;
	[Export] public NodePath SelectButtonPath;
	[Export] public NodePath DotsContainerPath;
	[Export] public NodePath BackButtonPath;
	[Export] public NodePath DifficultyNamePath;
	[Export] public NodePath DifficultyDescriptionPath;
	[Export] public NodePath DifficultyArrowLeftPath;
	[Export] public NodePath DifficultyArrowRightPath;
	[Export] public NodePath DifficultyLockLabelPath;
	[Export] public NodePath DifficultyDotsContainerPath;

	private Button _arrowLeft;
	private Button _arrowRight;
	private Label _machineName;
	private Label _machineDescription;
	private Label _lockLabel;
	private Button _selectButton;
	private HBoxContainer _dotsContainer;
	private Button _backButton;
	private Label _difficultyName;
	private Label _difficultyDescription;
	private Button _difficultyArrowLeft;
	private Button _difficultyArrowRight;
	private Label _difficultyLockLabel;
	private HBoxContainer _difficultyDotsContainer;

	private GameState _gameState;
	private int _currentIndex = 0;
	private List<MachineData> _machines;
	private List<Panel> _dots = new List<Panel>();
	
	private int _currentDifficultyIndex = 0;
	private List<Panel> _difficultyDots = new List<Panel>();
	// store selected difficulty per machine
	private Dictionary<int, int> _selectedDifficulties = new Dictionary<int, int>();
	private SaveManager _saveManager;

	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");    
		_saveManager = GetNode<SaveManager>("/root/SaveManager");

		_arrowLeft = GetNode<Button>(ArrowLeftPath);
		_arrowRight = GetNode<Button>(ArrowRightPath);
		_machineName = GetNode<Label>(MachineNamePath);
		_machineDescription = GetNode<Label>(MachineDescriptionPath);
		_lockLabel = GetNode<Label>(LockLabelPath);
		_selectButton = GetNode<Button>(SelectButtonPath);
		_dotsContainer = GetNode<HBoxContainer>(DotsContainerPath);
		_backButton = GetNode<Button>(BackButtonPath);
		_difficultyName = GetNode<Label>(DifficultyNamePath);
		_difficultyDescription = GetNode<Label>(DifficultyDescriptionPath);
		_difficultyArrowLeft = GetNode<Button>(DifficultyArrowLeftPath);
		_difficultyArrowRight = GetNode<Button>(DifficultyArrowRightPath);
		_difficultyLockLabel = GetNode<Label>(DifficultyLockLabelPath);
		_difficultyDotsContainer = GetNode<HBoxContainer>(DifficultyDotsContainerPath);


		_arrowLeft.Pressed += OnArrowLeftPressed;
		_arrowRight.Pressed += OnArrowRightPressed;
		_difficultyArrowLeft.Pressed += OnDifficultyArrowLeftPressed;
		_difficultyArrowRight.Pressed += OnDifficultyArrowRightPressed;
		_selectButton.Pressed += OnSelectPressed;
		_backButton.Pressed += OnBackPressed;

		InitializeMachines();
		BuildDots();
		BuildDifficultyDots();
		UpdateDisplay();
	}

	private void InitializeMachines()
	{
		_machines = new List<MachineData>
		{
			new MachineData(
				"The Original",
				"The classic machine. No special effects. Pure skee-ball.",
				true
			),
			new MachineData(
				"The Dojo",
				"+1 item slots. Start with a mimic",
				false
			),
			new MachineData(
				"The Factory",
				"Conveyor belts push the ball in different directions each round.",
				false
			),
			new MachineData(
				"Lucky Lanes",
				"All listed probabilites are doubled",
				false
			),
			new MachineData(
				"Space Station",
				"Reduced gravity changes everything.",
				false
			),
			new MachineData(
				"The Shrine",
				"+1 totem slots. -2 starting balls.",
				false
			),
		};
	}

	private void BuildDots()
	{
		// clear existing dots
		foreach (Node child in _dotsContainer.GetChildren())
			child.QueueFree();
		_dots.Clear();

		for (int i = 0; i < _machines.Count; i++)
		{
			Panel dot = new Panel();
			dot.CustomMinimumSize = new Vector2(12, 12);
			// add white background
			StyleBoxFlat style = new StyleBoxFlat();
			style.BgColor = new Color(1f, 1f, 1f, 1f);
			style.CornerRadiusTopLeft = 6;
			style.CornerRadiusTopRight = 6;
			style.CornerRadiusBottomLeft = 6;
			style.CornerRadiusBottomRight = 6;
			dot.AddThemeStyleboxOverride("panel", style);
			_dotsContainer.AddChild(dot);
			_dots.Add(dot);
		}
	}
	
	private void BuildDifficultyDots()
	{
		foreach (Node child in _difficultyDotsContainer.GetChildren())
			child.QueueFree();
		_difficultyDots.Clear();

		for (int i = 0; i < DifficultyDatabase.AllDifficulties.Count; i++)
		{
			Panel dot = new Panel();
			dot.CustomMinimumSize = new Vector2(12, 12);
			// add white background
			StyleBoxFlat style = new StyleBoxFlat();
			style.BgColor = new Color(1f, 1f, 1f, 1f);
			style.CornerRadiusTopLeft = 6;
			style.CornerRadiusTopRight = 6;
			style.CornerRadiusBottomLeft = 6;
			style.CornerRadiusBottomRight = 6;
			dot.AddThemeStyleboxOverride("panel", style);
			_difficultyDotsContainer.AddChild(dot);
			_difficultyDots.Add(dot);
		}
	}
	
	private void UpdateDifficultyDisplay()
	{
		string machineName = _machines[_currentIndex].Name;
		int unlockedUpTo = _saveManager.GetHighestDifficultyUnlocked(machineName);
		DifficultyData difficulty = DifficultyDatabase.AllDifficulties[_currentDifficultyIndex];

		_difficultyName.Text = difficulty.Name;
		_difficultyDescription.Text = difficulty.Description;

		bool isUnlocked = _currentDifficultyIndex <= unlockedUpTo;

		if (isUnlocked)
		{
			_difficultyDescription.Visible = true;
			_difficultyLockLabel.Visible = false;
		}
		else
		{
			string previousDifficulty = DifficultyDatabase.AllDifficulties[_currentDifficultyIndex - 1].Name;
			_difficultyLockLabel.Text = $"🔒 Beat {previousDifficulty} to unlock";
			_difficultyLockLabel.Visible = true;
			_difficultyDescription.Visible = false;
		}

		// update arrows
		_difficultyArrowLeft.Visible = _currentDifficultyIndex > 0;
		_difficultyArrowRight.Visible = _currentDifficultyIndex < DifficultyDatabase.AllDifficulties.Count - 1;

		// update dots
		for (int i = 0; i < _difficultyDots.Count; i++)
		{
			bool dotUnlocked = i <= unlockedUpTo;
			if (i == _currentDifficultyIndex)
				_difficultyDots[i].SelfModulate = new Color(1f, 1f, 1f, 1f);
			else if (dotUnlocked)
				_difficultyDots[i].SelfModulate = new Color(1f, 1f, 1f, 0.5f);
			else
				_difficultyDots[i].SelfModulate = new Color(1f, 1f, 1f, 0.15f);
		}
	}

	private void UpdateDisplay()
	{
		MachineData machine = _machines[_currentIndex];

		_machineName.Text = machine.Name;
		_machineDescription.Text = machine.Description;

		if (machine.IsUnlocked)
		{
			_selectButton.Visible = true;
			_lockLabel.Visible = false;
		}
		else
		{
			_selectButton.Visible = false;
			_lockLabel.Visible = true;
			_lockLabel.Text = $"🔒 {machine.UnlockCondition}";
		}

		// update arrows
		_arrowLeft.Visible = _currentIndex > 0;
		_arrowRight.Visible = _currentIndex < _machines.Count - 1;

		// update dots
		for (int i = 0; i < _dots.Count; i++)
		{
			_dots[i].SelfModulate = i == _currentIndex
				? new Color(1f, 1f, 1f, 1f)      // active dot bright
				: new Color(1f, 1f, 1f, 0.3f);   // inactive dot dim
		}
		
		// reset difficulty to highest unlocked for this machine
		string machineName = _machines[_currentIndex].Name;
		_currentDifficultyIndex = _saveManager.GetHighestDifficultyUnlocked(machineName);
		UpdateDifficultyDisplay();
	}

	private void OnArrowLeftPressed()
	{
		if (_currentIndex > 0)
		{
			_currentIndex--;
			UpdateDisplay();
		}
	}

	private void OnArrowRightPressed()
	{
		if (_currentIndex < _machines.Count - 1)
		{
			_currentIndex++;
			UpdateDisplay();
		}
	}
	
	private void OnDifficultyArrowLeftPressed()
	{
		if (_currentDifficultyIndex > 0)
		{
			_currentDifficultyIndex--;
			UpdateDifficultyDisplay();
		}
	}

	private void OnDifficultyArrowRightPressed()
	{
		if (_currentDifficultyIndex < DifficultyDatabase.AllDifficulties.Count - 1)
		{
			_currentDifficultyIndex++;
			UpdateDifficultyDisplay();
		}
	}

	private void OnSelectPressed()
	{
		string machineName = _machines[_currentIndex].Name;
		int unlockedUpTo = _saveManager.GetHighestDifficultyUnlocked(machineName);

		// block if selected difficulty is locked
		if (_currentDifficultyIndex > unlockedUpTo)
		{
			GD.Print("Difficulty locked!");
			return;
		}

		_gameState.ResetRun();
		_gameState.CurrentMachine = machineName;
		_gameState.CurrentDifficulty = (Difficulty)_currentDifficultyIndex;
		GetTree().ChangeSceneToFile("res://scenes/game_scene.tscn");
	}

	private void OnBackPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
	}
}
