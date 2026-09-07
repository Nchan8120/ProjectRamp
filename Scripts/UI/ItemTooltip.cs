using Godot;
using System;

public partial class ItemTooltip : Panel
{
	private Label _nameLabel;
	private Label _descriptionLabel;
	private Label _rarityLabel;
	private StyleBoxFlat _styleBox;

	public override void _Ready()
	{
		AddToGroup("ItemTooltip");
		
		// build style
		_styleBox = new StyleBoxFlat();
		_styleBox.BgColor = new Color(0.1f, 0.1f, 0.2f, 0.95f);
		_styleBox.BorderWidthTop = 2;
		_styleBox.BorderWidthBottom = 2;
		_styleBox.BorderWidthLeft = 2;
		_styleBox.BorderWidthRight = 2;
		_styleBox.CornerRadiusTopLeft = 4;
		_styleBox.CornerRadiusTopRight = 4;
		_styleBox.CornerRadiusBottomLeft = 4;
		_styleBox.CornerRadiusBottomRight = 4;
		_styleBox.ContentMarginLeft = 8;
		_styleBox.ContentMarginRight = 8;
		_styleBox.ContentMarginTop = 8;
		_styleBox.ContentMarginBottom = 8;
		AddThemeStyleboxOverride("panel", _styleBox);

		// build layout in code
		MarginContainer margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_left", 8);
		margin.AddThemeConstantOverride("margin_right", 8);
		margin.AddThemeConstantOverride("margin_top", 8);
		margin.AddThemeConstantOverride("margin_bottom", 8);
		AddChild(margin);

		VBoxContainer vbox = new VBoxContainer();
		vbox.AddThemeConstantOverride("separation", 6);
		margin.AddChild(vbox);

		_nameLabel = new Label();
		_nameLabel.AutowrapMode = TextServer.AutowrapMode.Word;
		_nameLabel.CustomMinimumSize = new Vector2(150, 0);
		_nameLabel.AddThemeFontSizeOverride("font_size", 16);
		vbox.AddChild(_nameLabel);

		_descriptionLabel = new Label();
		_descriptionLabel.AutowrapMode = TextServer.AutowrapMode.Word;
		_descriptionLabel.CustomMinimumSize = new Vector2(150, 0);
		_descriptionLabel.AddThemeFontSizeOverride("font_size", 13);
		vbox.AddChild(_descriptionLabel);

		_rarityLabel = new Label();
		_rarityLabel.AutowrapMode = TextServer.AutowrapMode.Word;
		_rarityLabel.CustomMinimumSize = new Vector2(150, 0);
		_rarityLabel.AddThemeFontSizeOverride("font_size", 12);
		vbox.AddChild(_rarityLabel);

		Visible = false;
	}

	public void ShowTotem(OwnedTotem totem, Control anchor)
	{
		_nameLabel.Text = totem.Name;
		_descriptionLabel.Text = totem.Description;
		_rarityLabel.Text = totem.Rarity.ToString();
		_rarityLabel.AddThemeColorOverride("font_color", GetRarityColor(totem.Rarity));
		_styleBox.BorderColor = GetRarityColor(totem.Rarity);

		Show();
		PositionTooltip(anchor);
	}

	public void ShowItem(OwnedItem item, Control anchor)
	{
		_nameLabel.Text = item.Name;
		_descriptionLabel.Text = item.Description;
		_rarityLabel.Text = item.Type.ToString();
		_rarityLabel.AddThemeColorOverride("font_color", new Color(1f, 1f, 1f));
		_styleBox.BorderColor = new Color(1f, 1f, 1f, 0.5f);

		Show();
		PositionTooltip(anchor);
	}

	public void ShowItemData(ItemData item, Control anchor, TotemRarity? rarity = null)
	{
		_nameLabel.Text = item.Name;
		_descriptionLabel.Text = item.Description;

		if (rarity.HasValue)
		{
			_rarityLabel.Text = rarity.Value.ToString();
			_rarityLabel.AddThemeColorOverride("font_color", GetRarityColor(rarity.Value));
			_styleBox.BorderColor = GetRarityColor(rarity.Value);
		}
		else
		{
			_rarityLabel.Text = item.Type.ToString();
			_rarityLabel.AddThemeColorOverride("font_color", new Color(1f, 1f, 1f));
			_styleBox.BorderColor = new Color(1f, 1f, 1f, 0.5f);
		}

		Show();
		PositionTooltip(anchor);
	}

	private void PositionTooltip(Control anchor)
	{
		// force layout update so size is correct
		ResetSize();

		Vector2 anchorPos = anchor.GlobalPosition;
		Vector2 anchorSize = anchor.Size;
		Vector2 tooltipSize = Size;
		Vector2 screenSize = GetViewportRect().Size;

		 float quarter = screenSize.X / 4f;
		float anchorMidX = anchorPos.X + anchorSize.X / 2f;

		float x, y;

		 if (anchorMidX < quarter)
			// zone 1 - far left, show right
			x = anchorPos.X + anchorSize.X + 8;
		else if (anchorMidX < quarter * 2)
			// zone 2 - middle left, show left
			x = anchorPos.X - tooltipSize.X - 150;
		else if (anchorMidX < quarter * 3)
			// zone 3 - middle right, show right
			x = anchorPos.X + anchorSize.X + 8;
		else
			// zone 4 - far right, show left
			x = anchorPos.X - tooltipSize.X - 150;

		// vertical - align with top of anchor, shift up if it goes off bottom
		y = anchorPos.Y;
		if (y + tooltipSize.Y > screenSize.Y)
			y = screenSize.Y - tooltipSize.Y - 8;

		// clamp to screen bounds
		x = Mathf.Clamp(x, 8, screenSize.X - tooltipSize.X - 8);
		y = Mathf.Clamp(y, 8, screenSize.Y - tooltipSize.Y - 8);

		GlobalPosition = new Vector2(x, y);
	}

	private Color GetRarityColor(TotemRarity rarity)
	{
		return rarity switch
		{
			TotemRarity.Common => new Color(0.8f, 0.8f, 0.8f),
			TotemRarity.Rare => new Color(0.4f, 0.6f, 1f),
			TotemRarity.Epic => new Color(0.7f, 0.3f, 1f),
			TotemRarity.Legendary => new Color(1f, 0.8f, 0.2f),
			_ => new Color(1f, 1f, 1f)
		};
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
		_styleBox.BorderColor = new Color(1f, 1f, 1f, 0.5f);

		Show();
		PositionTooltip(anchor);
	}
}
