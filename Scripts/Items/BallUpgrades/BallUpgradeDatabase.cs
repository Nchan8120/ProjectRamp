using Godot;
using System;

public static class BallUpgradeDatabase
{
	public static BallUpgradeEffect GetEffect(string upgradeType)
	{
		return upgradeType switch
		{
			"Glass Ball" => new GlassBallEffect(),
			"Golden Ball" => new GoldenBallEffect(),
			"Rubber Ball" => new RubberBallEffect(),
			_ => null // Standard ball has no effect
		};
	}
}
