using Godot;

public abstract class TotemEffect
{
	protected GameState GameState;
	protected RoundManager RoundManager;

	public virtual void Initialize(GameState gameState, RoundManager roundManager)
	{
		GameState = gameState;
		RoundManager = roundManager;
	}

	public virtual void OnRoundStart() { }
	public virtual void OnBallThrown() { }
	public virtual void OnScore(int points) { }
	public virtual void OnMiss() { }
	public virtual void OnRoundEnd() { }
	public virtual void OnShopEnter() { }
	public virtual void OnTotemMoved() { }
	public virtual void ApplyPassiveEffects() { }
	public virtual void OnRemoved() { }
}
