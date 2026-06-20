using Godot;

public partial class RoundManager : Node3D
{
	[Export] public int StartingThreshold = 700;
	[Export] public int ThresholdIncrement = 200;
	[Export] public int TotalRounds = 10;
	[Export] public NodePath BallPath;
	[Export] public NodePath ScoreLabelPath;
	[Export] public NodePath BallsLabelPath;
	[Export] public NodePath ThresholdLabelPath;
	[Export] public NodePath RoundLabelPath;
	[Export] public NodePath MoneyLabelPath;

	private int _ballsRemaining;
	private int _currentScore;
	private int _currentRound;
	private int _currentThreshold;
	private BallController _ball;
	private Label _scoreLabel;
	private Label _ballsLabel;
	private Label _thresholdLabel;
	private Label _roundLabel;
	private GameState _gameState;
	private Label _moneyLabel;
	
	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");
		TotemManager totemManager = GetNode<TotemManager>("/root/TotemManager");
		totemManager.SetRoundManager(this);
		
		_ball = GetNode<BallController>(BallPath);
		_scoreLabel = GetNode<Label>(ScoreLabelPath);
		_ballsLabel = GetNode<Label>(BallsLabelPath);
		_thresholdLabel = GetNode<Label>(ThresholdLabelPath);
		_roundLabel = GetNode<Label>(RoundLabelPath);
		_moneyLabel = GetNode<Label>(MoneyLabelPath);
		_currentRound = _gameState.CurrentRound;
		StartRound();
	}

	public void StartRound()
	{
		_ballsRemaining = _gameState.BallsPerRound; // ← use GameState value
		_currentScore = 0;
		_currentThreshold = StartingThreshold + (ThresholdIncrement * (_currentRound - 1));

		TotemManager totemManager = GetNode<TotemManager>("/root/TotemManager");
		totemManager.BroadcastRoundStart();

		UpdateUI();
		GD.Print($"Round {_currentRound} | Threshold: {_currentThreshold}");
	}

	public void OnBallScored(int points)
	{
		// apply clutch gene if last ball
		if (_ballsRemaining == 1 && _gameState.ClutchGeneActive)
		{
			points *= 2;
		}
		
		points = Mathf.RoundToInt(points * _gameState.ScoreMultiplier);
		_currentScore += points;
		_ballsRemaining--;
		_gameState.BallsThrown++;
		
		GetNode<TotemManager>("/root/TotemManager").BroadcastScore(points);
		
		UpdateUI();
		TotemPanel totemPanel = GetTree().Root.FindChild("TotemPanel", true, false) as TotemPanel;
		totemPanel?.RefreshUI();
		
		if (_currentScore >= _currentThreshold)
		{
			WinRound();
			return;
		}

		if (_ballsRemaining <= 0)
			EndRound();
	}

	public void OnBallMissed()
	{
		_ballsRemaining--;
		_gameState.BallsThrown++;
		GetNode<TotemManager>("/root/TotemManager").BroadcastMiss();
		UpdateUI();
		
		TotemPanel totemPanel = GetTree().Root.FindChild("TotemPanel", true, false) as TotemPanel;
		totemPanel?.RefreshUI();

		if (_ballsRemaining <= 0)
			EndRound();
	}

	private void UpdateUI()
	{
		_scoreLabel.Text = $"SCORE: {_currentScore}";
		_ballsLabel.Text = $"BALLS: {_ballsRemaining}";
		_thresholdLabel.Text = $"NEED: {_currentThreshold}";
		_roundLabel.Text = $"ROUND: {_currentRound}/{TotalRounds}";
		_moneyLabel.Text = $"${_gameState.Money}";
	}

	private void EndRound()
	{
		if (_currentScore > _gameState.HighestRoundScore)
		{
		_gameState.HighestRoundScore = _currentScore;
		}
		if (_currentScore >= _currentThreshold)
		{
			WinRound();
		}
		else
		{
			GameOver();
		}
	}

	private void WinRound()
	{
		if (_gameState.CurrentRound > _gameState.HighestRoundReached)
			_gameState.HighestRoundReached = _gameState.CurrentRound;

		_gameState.LeftoverBalls = _ballsRemaining;
		_gameState.TotalScore += _currentScore;
		_gameState.CurrentRound++;

		if (_gameState.CurrentRound > _gameState.TotalRounds && !_gameState.IsEndlessMode)
		{
			GD.Print("YOU WIN!");
			GetTree().CallDeferred("change_scene_to_file", "res://scenes/run_end_screen.tscn");
		}
		else
		{
			GetTree().CallDeferred("change_scene_to_file", "res://scenes/shop_scene.tscn");
		}
	}

	private void GameOver()
	{
		if (_gameState.CurrentRound > _gameState.HighestRoundReached)
		_gameState.HighestRoundReached = _gameState.CurrentRound;
		GetTree().CallDeferred("change_scene_to_file", "res://scenes/run_end_screen.tscn");
	}
	
}
