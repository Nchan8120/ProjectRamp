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
		AddChild(_player);
		_player.Play();

		ApplySavedVolume();
	}

	private void ApplySavedVolume()
	{
		SaveManager saveManager = GetNode<SaveManager>("/root/SaveManager");
		float savedVolume = saveManager.GetMusicVolume();
		int busIndex = AudioServer.GetBusIndex("Music");

		float linear = savedVolume / 100f;
		float db = Mathf.LinearToDb(Mathf.Max(linear, 0.0001f));
		AudioServer.SetBusVolumeDb(busIndex, savedVolume <= 0 ? -80f : db);
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
