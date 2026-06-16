using Godot;
using System.Collections.Generic;

public partial class RunEndScreen : Control
{
	[Export] public NodePath TitleLabelPath;
	[Export] public NodePath DefeatedByLabelPath;
	[Export] public NodePath HighScoreLabelPath;
	[Export] public NodePath RoundLabelPath;
	[Export] public NodePath BallsThrownLabelPath;
	[Export] public NodePath ItemsBoughtLabelPath;
	[Export] public NodePath RerollsLabelPath;
	[Export] public NodePath MainMenuButtonPath;
	[Export] public NodePath ActionButtonPath;

	private Label _titleLabel;
	private Label _defeatedByLabel;
	private Label _highScoreLabel;
	private Label _roundLabel;
	private Label _ballsThrownLabel;
	private Label _itemsBoughtLabel;
	private Label _rerollsLabel;
	private Button _mainMenuButton;
	private Button _actionButton;

	private GameState _gameState;
	private bool _isVictory;

	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");

		_titleLabel = GetNode<Label>(TitleLabelPath);
		_defeatedByLabel = GetNode<Label>(DefeatedByLabelPath);
		_highScoreLabel = GetNode<Label>(HighScoreLabelPath);
		_roundLabel = GetNode<Label>(RoundLabelPath);
		_ballsThrownLabel = GetNode<Label>(BallsThrownLabelPath);
		_itemsBoughtLabel = GetNode<Label>(ItemsBoughtLabelPath);
		_rerollsLabel = GetNode<Label>(RerollsLabelPath);
		_mainMenuButton = GetNode<Button>(MainMenuButtonPath);
		_actionButton = GetNode<Button>(ActionButtonPath);

		_mainMenuButton.Pressed += OnMainMenuPressed;
		_actionButton.Pressed += OnActionPressed;

		_isVictory = _gameState.DefeatedByBoss == null 
					 && _gameState.CurrentRound > _gameState.TotalRounds;

		PopulateScreen();
	}

	private void PopulateScreen()
	{
		// title
		_titleLabel.Text = _isVictory ? "VICTORY!" : "GAME OVER";

		// boss defeat label
		if (_gameState.DefeatedByBoss != null)
		{
			_defeatedByLabel.Visible = true;
			_defeatedByLabel.Text = $"Defeated by {_gameState.DefeatedByBoss}";
		}
		else
		{
			_defeatedByLabel.Visible = false;
		}

		// stats
		_highScoreLabel.Text = $"Highest Round Score:  {_gameState.HighestRoundScore}";
		_roundLabel.Text = $"Round Reached:  {_gameState.HighestRoundReached}";
		_ballsThrownLabel.Text = $"Balls Thrown:  {_gameState.BallsThrown}";
		_itemsBoughtLabel.Text = $"Items Bought:  {_gameState.ItemsBought}";
		_rerollsLabel.Text = $"Times Rerolled:  {_gameState.TimesRerolled}";

		// action button text
		_actionButton.Text = _isVictory ? "ENDLESS MODE" : "TRY AGAIN";
	}

	private void OnMainMenuPressed()
	{
		_gameState.ResetRun();
		// main menu scene comes later
		// for now go back to game scene
		GetTree().ChangeSceneToFile("res://scenes/game_scene.tscn");
	}

	private void OnActionPressed()
	{
		if (_isVictory)
		{
			// endless mode - continue run without resetting
			_gameState.IsEndlessMode = true;
			GetTree().ChangeSceneToFile("res://scenes/game_scene.tscn");
		}
		else
		{
			// try again - full reset
			_gameState.ResetRun();
			GetTree().ChangeSceneToFile("res://scenes/game_scene.tscn");
		}
	}
}
