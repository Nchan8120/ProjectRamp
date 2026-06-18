public class BonusBallEffect : TotemEffect
{
	public override void ApplyPassiveEffects()
	{
		GameState.BallsPerRound += 1;
	}
}
