public class UnderPressureEffect : TotemEffect
{
	private int _missCount = 0;

	public override void OnMiss()
	{
		_missCount++;
		GameState.ScoreMultiplier += 0.25f;
	}

	public override void OnRoundStart()
	{
		_missCount = 0;
		// remove this totem's contribution to multiplier
		GameState.ScoreMultiplier = 1.0f;
	}
}
