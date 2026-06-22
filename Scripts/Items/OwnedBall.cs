public class OwnedBall
{
	public int BallNumber;
	public string UpgradeType;
	public bool IsStandard => UpgradeType == "Standard";

	public OwnedBall(int ballNumber, string upgradeType = "Standard")
	{
		BallNumber = ballNumber;
		UpgradeType = upgradeType;
	}
}
