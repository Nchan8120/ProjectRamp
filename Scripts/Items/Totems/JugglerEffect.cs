public class JugglerEffect : TotemEffect
{
	public override void ApplyPassiveEffects()
	{
		GameState.MaxItems += 1;
	}
}
