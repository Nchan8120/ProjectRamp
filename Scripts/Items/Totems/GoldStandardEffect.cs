using Godot;
using System;

public class GoldStandardEffect : TotemEffect
{
	private const int PayoutPerBall = 5;

	// golden balls that haven't been thrown yet this round
	private int CountHeldGoldenBalls()
	{
		// RoundManager can be a freed instance while in the shop scene,
		// so only use it if it's still valid
		int firstUnthrown = GodotObject.IsInstanceValid(RoundManager)
			? RoundManager.CurrentBallIndex
			: 0;

		int count = 0;
		for (int i = firstUnthrown; i < GameState.OwnedBalls.Count; i++)
		{
			if (GameState.OwnedBalls[i].UpgradeType == "Golden Ball")
				count++;
		}
		return count;
	}

	public override void OnRoundEnd()
	{
		int held = CountHeldGoldenBalls();
		if (held <= 0) return;

		int payout = held * PayoutPerBall;
		GameState.AddMoney(payout);
		GD.Print($"Gold Standard: {held} held Golden Balls = +${payout}");
	}

	public override string GetDisplayValue()
	{
		int held = CountHeldGoldenBalls();
		return held > 0 ? $"+${held * PayoutPerBall}" : null;
	}
}
