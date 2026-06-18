public class FreeRerollEffect : TotemEffect
{
	public override void OnShopEnter()
	{
		GameState.HasFreeReroll = true;
	}
}
