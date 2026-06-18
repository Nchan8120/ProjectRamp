using Godot;
using System.Collections.Generic;

public partial class TotemManager : Node
{
	private GameState _gameState;
	private RoundManager _roundManager;

	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");
	}

	// called by RoundManager to give TotemManager a reference
	public void SetRoundManager(RoundManager roundManager)
	{
		_roundManager = roundManager;
		InitializeEffects();
	}

	private void InitializeEffects()
	{
		foreach (OwnedTotem totem in _gameState.OwnedTotems)
		{
			if (totem?.Effect != null)
				totem.Effect.Initialize(_gameState, _roundManager);
		}
		ApplyPassiveEffects();
	}

	// rebuild passive effects whenever totems change
	public void ApplyPassiveEffects()
	{
		// reset to defaults first
		_gameState.MaxItems = 3;
		_gameState.BallsPerRound = 9;

		foreach (OwnedTotem totem in _gameState.OwnedTotems)
		{
			if (totem?.Effect != null)
				totem.Effect.ApplyPassiveEffects();
		}
	}

	public void BroadcastRoundStart()
	{
		foreach (OwnedTotem totem in _gameState.OwnedTotems)
			totem?.Effect?.OnRoundStart();
		ApplyPassiveEffects();
	}

	public void BroadcastScore(int points)
	{
		foreach (OwnedTotem totem in _gameState.OwnedTotems)
			totem?.Effect?.OnScore(points);
	}

	public void BroadcastMiss()
	{
		foreach (OwnedTotem totem in _gameState.OwnedTotems)
			totem?.Effect?.OnMiss();
	}

	public void BroadcastRoundEnd()
	{
		foreach (OwnedTotem totem in _gameState.OwnedTotems)
			totem?.Effect?.OnRoundEnd();
	}

	public void BroadcastShopEnter()
	{
		foreach (OwnedTotem totem in _gameState.OwnedTotems)
			totem?.Effect?.OnShopEnter();
	}

	public void BroadcastTotemMoved()
	{
		foreach (OwnedTotem totem in _gameState.OwnedTotems)
			totem?.Effect?.OnTotemMoved();
	}

	public void OnTotemAdded(OwnedTotem totem)
	{
		totem.Effect?.Initialize(_gameState, _roundManager);
		ApplyPassiveEffects();
	}

	public void OnTotemRemoved()
	{
		ApplyPassiveEffects();
	}
}
