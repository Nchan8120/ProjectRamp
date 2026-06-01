using Godot;

public partial class RoundManager : Node3D
{
	[Export] public int BallsPerRound = 9;
	[Export] public NodePath BallPath;
	[Export] public NodePath ScoreLabelPath;
	[Export] public NodePath BallsLabelPath;

	private int _ballsRemaining;
	private int _currentScore;
	private BallController _ball;
	private Label _scoreLabel;
	private Label _ballsLabel;

	public override void _Ready()
	{
		_ball = GetNode<BallController>(BallPath);
		_scoreLabel = GetNode<Label>(ScoreLabelPath);
		_ballsLabel = GetNode<Label>(BallsLabelPath);
		StartRound();
	}

	public void StartRound()
	{
		_ballsRemaining = BallsPerRound;
		_currentScore = 0;
		UpdateUI();
	}

	public void OnBallScored(int points)
	{
		_currentScore += points;
		_ballsRemaining--;
		UpdateUI();

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
	}

	private void EndRound()
	{
		GD.Print($"Round over! Final score: {_currentScore}");
	}
}
