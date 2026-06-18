public class BailoutEffect : TotemEffect
{
	public override void OnMiss()
	{
		GameState.AddMoney(1);
	}
}
