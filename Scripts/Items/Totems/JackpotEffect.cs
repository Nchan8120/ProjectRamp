using Godot;

public class JackpotEffect : TotemEffect
{
	private int _targetHole = -1;
	private const int JackpotReward = 4;
	private ScoringHole _selectedHole;

	public override void Initialize(GameState gameState, RoundManager roundManager)
	{
		base.Initialize(gameState, roundManager);
		SelectRandomHole();
	}

	private void SelectRandomHole()
	{
		_selectedHole?.SetHighlighted(false); // clear old highlight

		var holes = RoundManager.GetScoringHoles();
		if (holes.Count > 0)
		{
			_selectedHole = holes[(int)GD.RandRange(0, holes.Count - 1)];
			_selectedHole.SetHighlighted(true);
		}
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
