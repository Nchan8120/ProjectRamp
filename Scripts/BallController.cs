using Godot;
using System.Collections.Generic;

public partial class BallController : RigidBody3D
{
	[Export] public float MaxThrowSpeed = 15.0f;
	[Export] public float MouseSensitivity = 0.5f;
	[Export] public int BufferFrames = 5;
	[Export] public float HorizontalInfluence = 1.0f;   // left/right strength
	[Export] public float VerticalInfluence = -1.0f;    // positive or negative depending on camera
	[Export] public float ForwardInfluence = -1.0f;     // positive or negative depending on camera
	[Export] public NodePath MeshPath; // assign in inspector
	[Export] public float MissTimeout = 3.0f; // seconds before ball is considered a miss
	
	private float _airTime = 0f;
	private bool _hasScored = false;
	private RoundManager _roundManager;
	private MeshInstance3D _mesh;
	private StandardMaterial3D _material;
	private enum BallState { Idle, Gripping, Throwing, Scored }
	private BallState _state = BallState.Idle;
	private Vector3 _startPosition;
	private List<Vector2> _mouseBuffer = new List<Vector2>();
	private Vector2 _lastMousePos;

	public override void _Ready()
	{
		_startPosition = GlobalPosition;
		Freeze = true; // ball doesnt move until thrown
		
		// grab the mesh and create a material we can change color on
		_mesh = GetNode<MeshInstance3D>(MeshPath);
		_material = new StandardMaterial3D();
		_material.AlbedoColor = Colors.White;
		_mesh.MaterialOverride = _material;
		 _roundManager = GetTree().GetFirstNodeInGroup("RoundManager") as RoundManager;
	}

	public override void _Process(double delta)
	{
		if (_state == BallState.Throwing)
		{
			_airTime += (float)delta;

			// if ball has been in play too long without scoring, call it a miss
			if (_airTime >= MissTimeout)
			{
				_roundManager?.OnBallMissed();
				ResetBall();
			}
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				if (mouseButton.Pressed && _state == BallState.Idle)
				{
					// only start grip if the click is on the ball
					if (IsMouseOverBall(mouseButton.Position))
					{
						StartGrip(mouseButton.Position);
					}
				}
				else if (!mouseButton.Pressed && _state == BallState.Gripping)
				{
					ReleaseBall();
				}
			}
		}

		if (@event is InputEventMouseMotion mouseMotion && _state == BallState.Gripping)
		{
			_mouseBuffer.Add(mouseMotion.Relative);
			if (_mouseBuffer.Count > BufferFrames)
				_mouseBuffer.RemoveAt(0);
		}
	}

	private bool IsMouseOverBall(Vector2 screenPos)
	{
		Camera3D camera = GetViewport().GetCamera3D();
		if (camera == null) return false;

		// project the ball's 3D position to screen space
		Vector2 ballScreenPos = camera.UnprojectPosition(GlobalPosition);

		// check distance from click to ball's screen position
		float clickRadius = 40f; // pixels, adjust based on ball size on screen
		return screenPos.DistanceTo(ballScreenPos) <= clickRadius;
	}

	private void StartGrip(Vector2 mousePos)
	{
		_state = BallState.Gripping;
		_lastMousePos = mousePos;
		_mouseBuffer.Clear();
		_material.AlbedoColor = Colors.Yellow; // yellow when gripped
		 GD.Print("Grip started"); // add this
	}

	private void ReleaseBall()
	{
		GD.Print($"Release - buffer count: {_mouseBuffer.Count}"); // add this
		if (_mouseBuffer.Count == 0)
		{
			_state = BallState.Idle;
			return;
		}

		// average the buffered mouse movement
		Vector2 avgMovement = Vector2.Zero;
		foreach (var m in _mouseBuffer)
			avgMovement += m;
		avgMovement /= _mouseBuffer.Count;

		// convert 2D mouse movement to 3D throw direction
		// tweak these multipliers based on your camera orientation
		Vector3 throwDirection = new Vector3(
			avgMovement.X * MouseSensitivity * HorizontalInfluence,
			avgMovement.Y * MouseSensitivity * VerticalInfluence,
			avgMovement.Y * MouseSensitivity * ForwardInfluence
		);

		// clamp to max speed
		if (throwDirection.Length() > MaxThrowSpeed)
			throwDirection = throwDirection.Normalized() * MaxThrowSpeed;

		// unfreeze and apply velocity
		Freeze = false;
		LinearVelocity = throwDirection;

		_state = BallState.Throwing;
	}

	public async void OnScored()
	{
		_hasScored = true;
		_airTime = 0f;
		_state = BallState.Scored;
		ResetBall();
	}

	private void ResetBall()
	{
		_hasScored = false;
		_airTime = 0f;
		Freeze = true;
		LinearVelocity = Vector3.Zero;
		AngularVelocity = Vector3.Zero;
		GlobalPosition = _startPosition;
		_state = BallState.Idle;
		_mouseBuffer.Clear();
		_material.AlbedoColor = Colors.White;
	}
}
