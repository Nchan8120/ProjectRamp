public class FreeRerollEffect : TotemEffect
{
	public override void Initialize(GameState gameState, RoundManager roundManager)
	{
		base.Initialize(gameState, roundManager);
		// give free reroll immediately on purchase
		GameState.HasFreeReroll = true;
	}
	
	public override void OnShopEnter()
	{
		GameState.HasFreeReroll = true;
	}
}
