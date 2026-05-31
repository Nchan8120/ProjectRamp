using Godot;

public partial class ScoringHole : Area3D
{
	[Export] public int PointValue = 100;
	private RoundManager _roundManager;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		_roundManager = GetTree().GetFirstNodeInGroup("RoundManager") as RoundManager;
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is BallController ball)
		{
			GD.Print($"Scored {PointValue} points!");
			ball.OnScored();
			_roundManager?.OnBallScored(PointValue);
		}
	}
}
