public class HotStreakEffect : TotemEffect
{
	private float _multiplier = 1.0f;
	private int _streak = 0;

	public override void OnScore(int points)
	{
		_streak++;
		_multiplier = 1.0f + (_streak * 0.1f);
		GameState.ScoreMultiplier *= _multiplier;
	}

	public override void OnMiss()
	{
		_streak = 0;
		_multiplier = 1.0f;
		GameState.ScoreMultiplier = 1.0f;
	}

	public override string GetDisplayValue()
	{
		return $"{_multiplier:0.0}x";
	}
}
