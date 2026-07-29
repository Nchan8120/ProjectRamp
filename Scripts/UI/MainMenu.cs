using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] public NodePath PlayButtonPath;
	[Export] public NodePath ProfileButtonPath;
	[Export] public NodePath SettingsButtonPath;
	[Export] public NodePath QuitButtonPath;
	
	private ProfileSelect _profileSelect;

	public override void _Ready()
	{
		GetNode<Button>(PlayButtonPath).Pressed += OnPlayPressed;
		GetNode<Button>(ProfileButtonPath).Pressed += OnProfilePressed;
		GetNode<Button>(SettingsButtonPath).Pressed += OnSettingsPressed;
		GetNode<Button>(QuitButtonPath).Pressed += OnQuitPressed;
		
		_profileSelect = GetNode<ProfileSelect>("ProfileSelect");
	}

	private void OnPlayPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/machine_select.tscn");
	}
	
	private void OnProfilePressed()
	{
		_profileSelect.Open();
	}

	private void OnSettingsPressed()
	{
		GD.Print("Settings - coming soon");
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
