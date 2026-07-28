using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] public NodePath PlayButtonPath;
	[Export] public NodePath SettingsButtonPath;
	[Export] public NodePath QuitButtonPath;

	public override void _Ready()
	{
		GetNode<Button>(PlayButtonPath).Pressed += OnPlayPressed;
		GetNode<Button>(SettingsButtonPath).Pressed += OnSettingsPressed;
		GetNode<Button>(QuitButtonPath).Pressed += OnQuitPressed;
	}

	private void OnPlayPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/machine_select.tscn");
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
