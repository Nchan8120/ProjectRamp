using Godot;
using System;

public partial class ItemTooltip : PanelContainer
{
	private const float TooltipWidth = 200f;
	private const float ScreenPadding = 8f;
	private const float AnchorGap = 8f;

	private Label _nameLabel;
	private Label _descriptionLabel;
	private Label _rarityLabel;

	private StyleBoxFlat _styleBox;

	public override void _Ready()
	{
		AddToGroup("ItemTooltip");

		// Panel Style
		_styleBox = new StyleBoxFlat
		{
			BgColor = Colors.White,
			BorderColor = Colors.Gray,

			BorderWidthLeft = 2,
			BorderWidthRight = 2,
			BorderWidthTop = 2,
			BorderWidthBottom = 2,

			CornerRadiusTopLeft = 8,
			CornerRadiusTopRight = 8,
			CornerRadiusBottomLeft = 8,
			CornerRadiusBottomRight = 8
		};

		AddThemeStyleboxOverride("panel", _styleBox);


		MarginContainer margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_left", 12);
		margin.AddThemeConstantOverride("margin_right", 12);
		margin.AddThemeConstantOverride("margin_top", 10);
		margin.AddThemeConstantOverride("margin_bottom", 10);
		AddChild(margin);
		
		VBoxContainer vbox = new VBoxContainer();
		vbox.AddThemeConstantOverride("separation", 5);
		// This controls the WIDTH of the tooltip.
		// Height remains dynamic.
		vbox.CustomMinimumSize = new Vector2(
			TooltipWidth - 24,
			0
		);
		margin.AddChild(vbox);

		_nameLabel = new Label();
		_nameLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		_nameLabel.HorizontalAlignment = HorizontalAlignment.Left;
		_nameLabel.AddThemeFontSizeOverride("font_size", 16);
		_nameLabel.AddThemeColorOverride(
			"font_color",
			new Color(0.1f, 0.1f, 0.1f)
		);
		vbox.AddChild(_nameLabel);

		_descriptionLabel = new Label();
		_descriptionLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		_descriptionLabel.HorizontalAlignment = HorizontalAlignment.Left;
		_descriptionLabel.VerticalAlignment = VerticalAlignment.Top;
		_descriptionLabel.AddThemeFontSizeOverride("font_size", 13);
		_descriptionLabel.AddThemeColorOverride(
			"font_color",
			new Color(0.2f, 0.2f, 0.2f)
		);
		vbox.AddChild(_descriptionLabel);

		_rarityLabel = new Label();
		_rarityLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		_rarityLabel.HorizontalAlignment = HorizontalAlignment.Left;
		_rarityLabel.AddThemeFontSizeOverride("font_size", 12);
		vbox.AddChild(_rarityLabel);

		Hide();
	}

	public void ShowTotem(OwnedTotem totem, Control anchor)
	{
		_nameLabel.Text = totem.Name;
		_descriptionLabel.Text = totem.Description;
		_rarityLabel.Text = totem.Rarity.ToString();

		Color rarityColor = GetRarityColor(totem.Rarity);

		_rarityLabel.AddThemeColorOverride(
			"font_color",
			rarityColor
		);

		_styleBox.BorderColor = rarityColor;

		ShowTooltip(anchor);
	}

	public void ShowItem(OwnedItem item, Control anchor)
	{
		_nameLabel.Text = item.Name;
		_descriptionLabel.Text = item.Description;
		_rarityLabel.Text = item.Type.ToString();

		_rarityLabel.AddThemeColorOverride(
			"font_color",
			new Color(0.3f, 0.3f, 0.3f)
		);

		_styleBox.BorderColor = new Color(0.7f, 0.7f, 0.7f);

		ShowTooltip(anchor);
	}
	
	public void ShowItemData(
		ItemData item,
		Control anchor,
		TotemRarity? rarity = null)
	{
		_nameLabel.Text = item.Name;
		_descriptionLabel.Text = item.Description;

		if (rarity.HasValue)
		{
			_rarityLabel.Text = rarity.Value.ToString();

			Color rarityColor = GetRarityColor(rarity.Value);

			_rarityLabel.AddThemeColorOverride(
				"font_color",
				rarityColor
			);

			_styleBox.BorderColor = rarityColor;
		}
		else
		{
			_rarityLabel.Text = item.Type.ToString();

			_rarityLabel.AddThemeColorOverride(
				"font_color",
				new Color(0.3f, 0.3f, 0.3f)
			);

			_styleBox.BorderColor = new Color(0.7f, 0.7f, 0.7f);
		}

		ShowTooltip(anchor);
	}

	public void ShowCapsule(CapsuleData capsule, Control anchor)
	{
		_nameLabel.Text = $"{capsule.Size} {capsule.Type} Capsule";

		_descriptionLabel.Text = capsule.Size switch
		{
			CapsuleSize.Small => "Pick 1 of 2 items",
			CapsuleSize.Medium => "Pick 1 of 4 items",
			CapsuleSize.Large => "Pick up to 2 of 6 items",
			_ => ""
		};

		_rarityLabel.Text = "";

		_styleBox.BorderColor = new Color(0.7f, 0.7f, 0.7f);

		ShowTooltip(anchor);
	}

	private void ShowTooltip(Control anchor)
	{
		Show();

		// PanelContainer automatically calculates its size from
		// its children. Defer positioning until that calculation
		// has happened.
		CallDeferred(
			MethodName.PositionTooltip,
			anchor
		);
	}

	private void PositionTooltip(Control anchor)
	{
		if (!IsInstanceValid(anchor))
			return;

		Vector2 screenSize = GetViewportRect().Size;

		Vector2 anchorPos = anchor.GlobalPosition;
		Vector2 anchorSize = anchor.Size;

		Vector2 tooltipSize = Size;

		float anchorCenterX =
			anchorPos.X + anchorSize.X / 2f;

		float quarter = screenSize.X / 4f;

		float x;

		// ZONE 1 — FAR LEFT
		// Tooltip goes RIGHT
		if (anchorCenterX < quarter)
		{
			x = anchorPos.X + anchorSize.X + AnchorGap;
		}


		// ZONE 2 — MIDDLE LEFT
		// Tooltip goes LEFT
		else if (anchorCenterX < quarter * 2f)
		{
			x = anchorPos.X - tooltipSize.X - AnchorGap;
		}

		// ZONE 3 — MIDDLE RIGHT
		// Tooltip goes RIGHT
		else if (anchorCenterX < quarter * 3f)
		{
			x = anchorPos.X + anchorSize.X + AnchorGap;
		}

		// ZONE 4 — FAR RIGHT
		// Tooltip goes LEFT
		else
		{
			x = anchorPos.X - tooltipSize.X - AnchorGap;
		}

		// VERTICAL POSITION
		float y = anchorPos.Y;

		// If tooltip extends below screen,
		// move it upward.
		if (y + tooltipSize.Y >
			screenSize.Y - ScreenPadding)
		{
			y = screenSize.Y -
				tooltipSize.Y -
				ScreenPadding;
		}

		// If tooltip extends above screen,
		// move it down.
		if (y < ScreenPadding)
		{
			y = ScreenPadding;
		}

		// SCREEN CLAMP
		x = Mathf.Clamp(
			x,
			ScreenPadding,
			screenSize.X -
			tooltipSize.X -
			ScreenPadding
		);

		y = Mathf.Clamp(
			y,
			ScreenPadding,
			screenSize.Y -
			tooltipSize.Y -
			ScreenPadding
		);

		GlobalPosition = new Vector2(x, y);
	}

	private Color GetRarityColor(TotemRarity rarity)
	{
		return rarity switch
		{
			TotemRarity.Common =>
				new Color(0.65f, 0.65f, 0.65f),

			TotemRarity.Rare =>
				new Color(0.4f, 0.6f, 1f),

			TotemRarity.Epic =>
				new Color(0.7f, 0.3f, 1f),

			TotemRarity.Legendary =>
				new Color(1f, 0.75f, 0.15f),

			_ =>
				Colors.Gray
		};
	}
}
