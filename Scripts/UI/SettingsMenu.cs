using Godot;

public partial class SettingsMenu : Control
{
	[Export] public NodePath MusicSliderPath;
	[Export] public NodePath BackButtonPath;

	private HSlider _musicSlider;
	private Button _backButton;
	private int _musicBusIndex;
	private SaveManager _saveManager;

	public override void _Ready()
	{
		_musicSlider = GetNode<HSlider>(MusicSliderPath);
		_backButton = GetNode<Button>(BackButtonPath);
		_saveManager = GetNode<SaveManager>("/root/SaveManager");

		_musicBusIndex = AudioServer.GetBusIndex("Music");

		// slider range 0-100, representing linear volume percentage
		_musicSlider.MinValue = 0;
		_musicSlider.MaxValue = 100;

		// initialize slider from saved value
		float savedVolume = _saveManager.GetMusicVolume();
		_musicSlider.Value = savedVolume;
		ApplyVolume(savedVolume);

		_musicSlider.ValueChanged += OnMusicVolumeChanged;
		_backButton.Pressed += OnBackPressed;
	}

	private void OnMusicVolumeChanged(double value)
	{
		ApplyVolume((float)value);
		_saveManager.SetMusicVolume((float)value);
	}
	
		private void ApplyVolume(float volumePercent)
	{
		float linear = volumePercent / 100f;
		float db = Mathf.LinearToDb(Mathf.Max(linear, 0.0001f));
		AudioServer.SetBusVolumeDb(_musicBusIndex, volumePercent <= 0 ? -80f : db);
	}

	private void OnBackPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
	}
}
