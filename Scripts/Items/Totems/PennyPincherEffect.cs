public class PennyPincherEffect : TotemEffect
{
	public override void ApplyPassiveEffects()
	{
		GameState.LeftoverBallValue = 4; // doubled from base $2
	}
}
