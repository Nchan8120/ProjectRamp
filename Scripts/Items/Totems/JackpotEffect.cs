using Godot;

public class JackpotEffect : TotemEffect
{
	private int _targetHole = -1;

	private const int JackpotReward = 4;

	public override void Initialize(GameState gameState, RoundManager roundManager)
	{
		base.Initialize(gameState, roundManager);
		SelectRandomHole();
	}

	private void SelectRandomHole()
	{
		_targetHole = (int)GD.RandRange(0, 6);
		GD.Print($"Jackpot target hole: {_targetHole}");
	}

	public override void OnMiss()
	{
		SelectRandomHole();
	}

	public override void OnHoleScored(int holeIndex)
	{
		if (holeIndex == _targetHole)
		{
			GameState.AddMoney(JackpotReward);
			GD.Print($"Jackpot! Hole {_targetHole} awarded ${JackpotReward}");
		}

		SelectRandomHole();
	}
}
