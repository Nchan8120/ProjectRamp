using Godot;

public partial class ScoringHole : Area3D
{
	[Export] public int PointValue = 100;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body.Name == "Ball")
		{
			GD.Print($"Scored {PointValue} points!");
		}
	}
}
