using Godot;

public partial class ScoringHole : Area3D
{
	[Export] public int PointValue = 100;
	[Export] public int HoleIndex = 0;
	private RoundManager _roundManager;
	private StandardMaterial3D _material;
	private Color _normalColor = Colors.Black;
	private Color _highlightColor = Colors.Yellow;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		_roundManager = GetTree().GetFirstNodeInGroup("RoundManager") as RoundManager;
		AddToGroup("ScoringHole");

		// give each hole its own material instance so changing one doesn't change them all
		MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		_material = new StandardMaterial3D();
		_material.AlbedoColor = _normalColor;
		mesh.SetSurfaceOverrideMaterial(0, _material);
	}
	
	public void SetHighlighted(bool highlighted)
	{
		_material.AlbedoColor = highlighted ? _highlightColor : _normalColor;
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is BallController ball)
		{
			GD.Print($"Scored {PointValue} points!");
			ball.OnScored();
			_roundManager?.OnBallScored(PointValue, HoleIndex);
		}
	}
}
