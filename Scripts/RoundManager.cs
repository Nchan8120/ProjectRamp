using Godot;

public partial class RoundManager : Node3D
{
	[Export] public int BallsPerRound = 9;
	[Export] public NodePath BallPath;

	private int _ballsRemaining;
	private int _currentScore;
	private BallController _ball;

	public override void _Ready()
	{
		_ball = GetNode<BallController>(BallPath);
		StartRound();
	}

	public void StartRound()
	{
		_ballsRemaining = BallsPerRound;
		_currentScore = 0;
		GD.Print($"Round started - Balls: {_ballsRemaining}");
	}

	public void OnBallScored(int points)
	{
		_currentScore += points;
		_ballsRemaining--;
		GD.Print($"Score: {_currentScore} | Balls remaining: {_ballsRemaining}");

		if (_ballsRemaining <= 0)
		{
			EndRound();
		}
	}

	public void OnBallMissed()
	{
		_ballsRemaining--;
		GD.Print($"Missed | Balls remaining: {_ballsRemaining}");

		if (_ballsRemaining <= 0)
		{
			EndRound();
		}
	}

	private void EndRound()
	{
		GD.Print($"Round over! Final score: {_currentScore}");
		// win/lose check comes next
	}
}
