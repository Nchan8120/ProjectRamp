using Godot;

public class EmergencyEjectionEffect : TotemEffect
{
	// true only during a round (RoundManager is freed once we're in the shop)
	// and once score is over half the threshold
	private bool IsActive()
	{
		if (!GodotObject.IsInstanceValid(RoundManager)) return false;
		return RoundManager.CurrentScore > RoundManager.CurrentThreshold / 2f;
	}

	public override string GetDisplayValue()
	{
		return IsActive() ? "active!" : null;
	}

	// only called when the totem is sold
	public override void OnRemoved()
	{
		if (!IsActive()) return;

		GD.Print("Emergency Ejection: finishing round early");
		RoundManager.EjectRound();
	}
}
