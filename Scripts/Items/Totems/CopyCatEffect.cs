using Godot;
using System;

public class CopycatEffect : TotemEffect
{
	private TotemEffect _copiedEffect;

	public override void OnTotemMoved()
	{
		UpdateCopiedEffect();
	}

	public override void OnRoundStart()
	{
		UpdateCopiedEffect();
		_copiedEffect?.OnRoundStart();
	}

	public override void OnScore(int points)
	{
		_copiedEffect?.OnScore(points);
	}

	public override void OnMiss()
	{
		_copiedEffect?.OnMiss();
	}

	public override void OnRoundEnd()
	{
		_copiedEffect?.OnRoundEnd();
	}

	public override void ApplyPassiveEffects()
	{
		_copiedEffect?.ApplyPassiveEffects();
	}

	private void UpdateCopiedEffect()
	{
		// find copycat's position in totem list
		int copycatIndex = -1;
		for (int i = 0; i < GameState.OwnedTotems.Count; i++)
		{
			if (GameState.OwnedTotems[i]?.Effect is CopycatEffect)
			{
				copycatIndex = i;
				break;
			}
		}

		// get totem below (next index)
		int belowIndex = copycatIndex + 1;
		if (belowIndex < GameState.OwnedTotems.Count && GameState.OwnedTotems[belowIndex] != null)
		{
			_copiedEffect = GameState.OwnedTotems[belowIndex].Effect;
			GD.Print($"Copycat copying: {GameState.OwnedTotems[belowIndex].Name}");
		}
		else
		{
			_copiedEffect = null;
			GD.Print("Copycat has nothing to copy");
		}
	}
}
