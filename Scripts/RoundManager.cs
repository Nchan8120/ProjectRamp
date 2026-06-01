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

	private int _ballsRemaining;
	private int _currentScore;
	private int _currentRound = 1;
	private int _currentThreshold;
	private BallController _ball;
	private Label _scoreLabel;
	private Label _ballsLabel;
	private Label _thresholdLabel;
	private Label _roundLabel;

	public override void _Ready()
	{
		_ball = GetNode<BallController>(BallPath);
		_scoreLabel = GetNode<Label>(ScoreLabelPath);
		_ballsLabel = GetNode<Label>(BallsLabelPath);
		_thresholdLabel = GetNode<Label>(ThresholdLabelPath);
		_roundLabel = GetNode<Label>(RoundLabelPath);
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
		GD.Print($"Round cleared with {leftoverBalls} balls remaining!");
		
		if (_currentRound >= TotalRounds)
		{
			GD.Print("YOU WIN! Run complete!");
			// victory state comes later
		}
		else
		{
			GD.Print($"Round {_currentRound} cleared! Moving to shop...");
			_currentRound++;
			// shop comes later, for now just start next round
			StartRound();
		}
	}

	private void GameOver()
	{
		GD.Print($"GAME OVER! Score: {_currentScore} | Needed: {_currentThreshold}");
		// game over screen comes later
		// for now reset to round 1
		_currentRound = 1;
		StartRound();
	}
}
