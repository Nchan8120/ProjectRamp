using Godot;

public partial class RoundManager : Node3D
{
	[Export] public int BallsPerRound = 9;
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
	private int _currentRound = 1;
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
		_ball = GetNode<BallController>(BallPath);
		_scoreLabel = GetNode<Label>(ScoreLabelPath);
		_ballsLabel = GetNode<Label>(BallsLabelPath);
		_thresholdLabel = GetNode<Label>(ThresholdLabelPath);
		_roundLabel = GetNode<Label>(RoundLabelPath);
		_moneyLabel = GetNode<Label>(MoneyLabelPath);
		StartRound();
	}

	public void StartRound()
	{
		_ballsRemaining = BallsPerRound;
		_currentScore = 0;
		_currentThreshold = StartingThreshold + (ThresholdIncrement * (_currentRound - 1));
		UpdateUI();
		GD.Print($"Round {_currentRound} | Threshold: {_currentThreshold}");
	}

	public void OnBallScored(int points)
	{
		_currentScore += points;
		_ballsRemaining--;
		UpdateUI();
		
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
		UpdateUI();

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
		int leftoverBalls = _ballsRemaining;
		GameState gameState = GetNode<GameState>("/root/GameState");
		_gameState.AwardRoundEndMoney(leftoverBalls);
		_gameState.TotalScore += _currentScore;
		_gameState.CurrentRound++;

		if (_gameState.CurrentRound > _gameState.TotalRounds)
		{
			GD.Print("YOU WIN!");
			// victory screen later
		}
		else
		{
			// go to shop
			GetTree().ChangeSceneToFile("res://scenes/shop_scene.tscn");
		}
	}

	private void GameOver()
	{
		GD.Print($"GAME OVER!");
		_gameState.ResetRun();
		GetTree().ChangeSceneToFile("res://scenes/game_scene.tscn");
	}
	
}
