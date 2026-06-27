using Godot;
using System;

public abstract class BallUpgradeEffect
{
	protected GameState GameState;
	protected RoundManager RoundManager;

	public virtual void Initialize(GameState gameState, RoundManager roundManager)
	{
		GameState = gameState;
		RoundManager = roundManager;
	}

	// returns modified point value
	public virtual int OnScore(int points, OwnedBall ball) => points;
	public virtual void OnMiss(OwnedBall ball) { }
	public bool BallWasRemoved = false;
}
