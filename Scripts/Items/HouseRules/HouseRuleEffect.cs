using Godot;
using System;

public abstract class HouseRuleEffect
{
	protected GameState GameState;

	public virtual void Initialize(GameState gameState)
	{
		GameState = gameState;
	}

	// fires once when purchased
	public virtual void OnPurchased() { }
}
