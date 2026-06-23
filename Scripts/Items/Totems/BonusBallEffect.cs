using Godot;

public class BonusBallEffect : TotemEffect
{
	private OwnedBall _addedBall;

	public override void Initialize(GameState gameState, RoundManager roundManager)
	{
		base.Initialize(gameState, roundManager);
		AddBonusBall();
	}

	private void AddBonusBall()
	{
		int ballNumber = GameState.OwnedBalls.Count + 1;
		_addedBall = new OwnedBall(ballNumber, "Bonus Ball", isLocked: true);
		GameState.OwnedBalls.Add(_addedBall);
		GD.Print($"Bonus Ball added: Ball {ballNumber}");
	}

	public override void OnRemoved()
	{
		if (_addedBall != null && GameState.OwnedBalls.Contains(_addedBall))
		{
			GameState.OwnedBalls.Remove(_addedBall);
			GD.Print("Bonus Ball removed from bag");
		}
	}

	// no passive effects needed anymore
	public override void ApplyPassiveEffects() { }
}
