using Ratelite;
using Ratelite.Resources;
using Ratelite.UI.Widgets;

namespace MinesweeperBasic.UI;

public sealed class ButtonFlag : ElementButton
{
	public ButtonFlag(int index, Action? action) : base(new Image(), action, null)
	{
		var image = (Image)element;
		image.texture = Vault.GetAsset<Texture2D>("ui");
		image.uv = image.texture.GetUVRegion(
			index == 0 ? new RectInt(176, 37, 9, 5) : new RectInt(176, 43, 9, 5)
		);
		size = image.size = new Vector2(9, 5);
		scale = image.scale = new Vector2(3);
	}
}