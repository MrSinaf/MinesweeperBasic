using MinesweeperBasic.Scenes;
using Ratelite;
using Ratelite.Resources;
using Ratelite.UI;
using Ratelite.UI.Widgets;
using Ratelite.Utils;

namespace MinesweeperBasic.UI;

public sealed class ButtonLevel : Button
{
	public ButtonLevel(Vector2Int tiles, int bomb) : base(
		$"{tiles.x}x{tiles.y}",
		delegate { },
		string.Empty
	)
	{
		var texture = Vault.GetAsset<Texture2D>("ui")!;
		
		mesh = Vault.GetAsset<Mesh>(UIModule.DEFAULT_MESH);
		material = new MaterialUI().SetTexture(texture)
								   .SetNinePatch(new Region(new Vector2(6)), 3);
		uv = texture.GetUVRegion(new RectInt(32, 32, 16, 16));
		padding = new Region(12);
		size = new Vector2(150, 150);
		
		label.pivot = label.anchors = new Vector2(0.5F, 1);
		label.tint = Color.white;
		
		// Création des tiles:
		var meshes = new (Rect vertices, Region uvs)[tiles.x * tiles.y];
		var tile = texture.GetUVRegion(new RectInt(176, 32, 4, 4));
		for (var x = 0; x < tiles.x; x++)
		for (var y = 0; y < tiles.y; y++)
			meshes[x + y * tiles.x] = (new Rect(new Vector2(x, y) * 4, new Vector2(4)), tile);
		AddChild(
			new UIElement
			{
				mesh = MeshFactory.CreateQuads(meshes),
				material = new MaterialUI().SetTexture(texture),
				size = new Vector2(tiles.x * 4, tiles.y * 4),
				useMeshBoundsSize = false,
				overflowHidden = false,
				pivot = new Vector2(0.5F),
				anchors = new Vector2(0.5F),
				isInteractif = false
			}
		);
		// Affichage du nombre de bombs:
		AddChild(
			new Label(bomb.ToString())
			{
				position = new Vector2(-22, 0),
				pivot = new Vector2(1, 0),
				anchors = new Vector2(1, 0)
			}
		);
		AddChild(
			new Image(texture)
			{
				uv = texture.GetUVRegion(new RectInt(212, 35, 8, 10)),
				position = new Vector2(0, 2),
				size = new Vector2(8, 10),
				scale = new Vector2(2),
				pivot = new Vector2(1, 0),
				anchors = new Vector2(1, 0)
			}
		);
		
		onClick += () => Stage.Load(new Game(tiles, bomb)).Wait();
		onPressed += OnPressed;
		onReleased += OnReleased;
		
		void OnPressed(UIElement e) => e.uv = texture.GetUVRegion(new RectInt(48, 32, 16, 16));
		void OnReleased(UIElement e) => e.uv = texture.GetUVRegion(new RectInt(32, 32, 16, 16));
	}
}