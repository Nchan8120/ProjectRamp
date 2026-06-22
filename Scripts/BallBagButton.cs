using Godot;

public partial class BallBagButton : Button
{
	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		BallBag ballBag = GetTree().Root.FindChild("BallBag", true, false) as BallBag;
		ballBag?.Open();
	}
}
