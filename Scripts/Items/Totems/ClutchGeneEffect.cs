public class ClutchGeneEffect : TotemEffect
{
	public override void OnRoundStart()
	{
		// handled in RoundManager when last ball is thrown
		// set a flag so RoundManager knows to double the score
		GameState.ClutchGeneActive = true;
	}

	public override void OnRemoved()
	{
		GameState.ClutchGeneActive = false;
	}
}
