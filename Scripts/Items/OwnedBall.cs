public class OwnedBall
{
	public int BallNumber;
	public string UpgradeType;
	public bool IsLocked; // locked balls cannot have upgrades applied
	public int BonusPoints;
	public bool IsStandard => UpgradeType == "Standard";

	public OwnedBall(int ballNumber, string upgradeType = "Standard", bool isLocked = false)
	{
		BallNumber = ballNumber;
		UpgradeType = upgradeType;
		IsLocked = isLocked;
		BonusPoints = 0;
	}
}
