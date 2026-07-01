using Godot;

public abstract class TotemEffect
{
	protected GameState GameState;
	protected RoundManager RoundManager;
	
	// override this to show a live status value on the totem slot, e.g. "1.3x" or "+50"
	public virtual string GetDisplayValue() => null;

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
	// returns modified points after this totem's effect
	public virtual int ProcessScore(int points) => points;
}
