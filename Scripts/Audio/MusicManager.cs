using Godot;

public partial class MusicManager : Node
{
	private AudioStreamPlayer _player;

	public override void _Ready()
	{
		_player = new AudioStreamPlayer();
		_player.Stream = GD.Load<AudioStream>("res://Audio/skeeyah concept 20SEP2026.mp3");
		_player.Bus = "Music";
		_player.Autoplay = true;
		_player.VolumeDb = -15f;
		AddChild(_player);
		_player.Play();
	}

	public void SetVolume(float db)
	{
		_player.VolumeDb = db;
	}

	public void Stop()
	{
		_player.Stop();
	}
}
