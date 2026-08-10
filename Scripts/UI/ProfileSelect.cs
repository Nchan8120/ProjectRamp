using Godot;
using System.Collections.Generic;

public partial class ProfileSelect : Control
{
	[Export] public NodePath CloseButtonPath;
	[Export] public NodePath UnlockAllButtonPath;
	[Export] public NodePath ConfirmUnlockButtonPath;

	private Button _closeButton;
	private Button _unlockAllButton;
	private Button _confirmUnlockButton;

	private SaveManager _saveManager;

	private List<Panel> _slots = new List<Panel>();
	private List<Label> _profileNumbers = new List<Label>();
	private List<Label> _profileInfos = new List<Label>();
	private List<Button> _selectButtons = new List<Button>();
	private List<Button> _deleteButtons = new List<Button>();

	public override void _Ready()
	{
		_saveManager = GetNode<SaveManager>("/root/SaveManager");

		_closeButton = GetNode<Button>(CloseButtonPath);
		_unlockAllButton = GetNode<Button>(UnlockAllButtonPath);
		_confirmUnlockButton = GetNode<Button>(ConfirmUnlockButtonPath);

		_closeButton.Pressed += OnClosePressed;
		_unlockAllButton.Pressed += OnUnlockAllPressed;
		_confirmUnlockButton.Pressed += OnConfirmUnlockPressed;

		// collect slot references
		var container = GetNode<HBoxContainer>("ProfilesContainer");
		
		for (int i = 0; i < 3; i++)
			{
				Panel slot = new Panel();
				slot.CustomMinimumSize = new Vector2(280, 300);

				Label number = new Label();
				number.Position = new Vector2(10, 10);
				number.Size = new Vector2(260, 40);
				number.AddThemeFontSizeOverride("font_size", 24);

				Label info = new Label();
				info.Position = new Vector2(10, 60);
				info.Size = new Vector2(260, 160);
				info.AutowrapMode = TextServer.AutowrapMode.Word;

				Button select = new Button();
				select.Position = new Vector2(10, 230);
				select.Size = new Vector2(260, 40);

				Button delete = new Button();
				delete.Position = new Vector2(10, 280);
				delete.Size = new Vector2(260, 40);
				delete.Text = "Delete";

				slot.AddChild(number);
				slot.AddChild(info);
				slot.AddChild(select);
				slot.AddChild(delete);
				container.AddChild(slot);

				_slots.Add(slot);
				_profileNumbers.Add(number);
				_profileInfos.Add(info);
				_selectButtons.Add(select);
				_deleteButtons.Add(delete);

				int index = i;
				select.Pressed += () => OnSelectPressed(index);
				delete.Pressed += () => OnDeletePressed(index);
			}
		Visible = false;
		RefreshUI();
	}

	public void Open()
	{
		_confirmUnlockButton.Visible = false;
		RefreshUI();
		Visible = true;
	}

	private void RefreshUI()
	{
		
		for (int i = 0; i < 3; i++)
		{
			_profileNumbers[i].Text = $"Profile {i + 1}";

			bool isEmpty = _saveManager.IsProfileEmpty(i);
			bool isActive = _saveManager.ActiveProfile == i;

			if (isEmpty)
			{
				_profileInfos[i].Text = "New Game";
				_deleteButtons[i].Visible = false;
			}
			else
			{
				int runs = _saveManager.GetProfileRunsPlayed(i);
				int highestDiff = _saveManager.GetProfileHighestDifficulty(i, "The Original");
				string diffName = DifficultyDatabase.AllDifficulties[highestDiff].Name;
				_profileInfos[i].Text = $"Runs Played: {runs}\nHighest Difficulty: {diffName}";
				_deleteButtons[i].Visible = true;
			}

			// highlight active profile
			_slots[i].SelfModulate = isActive
				? new Color(0.6f, 1f, 0.6f, 1f)
				: new Color(1f, 1f, 1f, 1f);

			_selectButtons[i].Text = isActive ? "Active" : "Select";
			_selectButtons[i].Disabled = isActive;
		}
	}

	private void OnSelectPressed(int profileIndex)
	{
		_saveManager.SetActiveProfile(profileIndex);
		RefreshUI();
	}

	private void OnDeletePressed(int profileIndex)
	{
		_saveManager.DeleteProfile(profileIndex);
		RefreshUI();
	}

	private void OnUnlockAllPressed()
	{
		_confirmUnlockButton.Visible = true;
	}

	private void OnConfirmUnlockPressed()
	{
		_saveManager.UnlockEverything();
		_confirmUnlockButton.Visible = false;
		RefreshUI();
		GD.Print("Everything unlocked for active profile");
	}

	private void OnClosePressed()
	{
		_confirmUnlockButton.Visible = false;
		Visible = false;
	}
}
