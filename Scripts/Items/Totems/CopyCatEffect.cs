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

	public override string GetDisplayValue()
	{
		// ensure we're looking at the right totem before reading its value
		UpdateCopiedEffect();
		return _copiedEffect?.GetDisplayValue();
	}

	private void UpdateCopiedEffect()
	{
		int copycatIndex = -1;
		for (int i = 0; i < GameState.OwnedTotems.Count; i++)
		{
			if (GameState.OwnedTotems[i]?.Effect is CopycatEffect)
			{
				copycatIndex = i;
				break;
			}
		}

		int belowIndex = copycatIndex + 1;
		if (belowIndex < GameState.OwnedTotems.Count && GameState.OwnedTotems[belowIndex] != null)
		{
			_copiedEffect = GameState.OwnedTotems[belowIndex].Effect;
		}
		else
		{
			_copiedEffect = null;
		}
	}
}
