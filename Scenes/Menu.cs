using MinesweeperBasic.UI;
using Ratelite;
using Ratelite.Resources;
using Ratelite.UI;
using Ratelite.UI.Widgets;

namespace MinesweeperBasic.Scenes;

public class Menu : Scene
{
	private Canvas canvas = null!;
	private Panel levels = null!;
	
	public override void Init()
		=> canvas = AddPlugin<Canvas>();
	
	public override void Start()
	{
		var texture = Vault.GetAsset<Texture2D>("ui")!;
		canvas.root.padding = new Region(10);
		canvas.root.AddChild(
			new Image(texture)
			{
				name = "title",
				pivot = new Vector2(0.5F, 1),
				anchors = new Vector2(0.5F, 0.75F),
				scale = new Vector2(3F),
				uv = texture.GetUVRegion(new RectInt(0, 0, 228, 32)),
			}
		);
		
		// Liste de boutons:
		var layout = new Layout
		{
			spacing = 5,
			pivot = new Vector2(0.5F, 0),
			anchors = new Vector2(0.5F, 0),
		};
		layout.AddChild(
			new Button("Jouer", () => levels.active = !levels.active)
		);
		layout.AddChild(
			new Button("Apprendre", () => Stage.Load(new Learn()).Wait())
		);
		layout.AddChild(new Button("Quitter", () => Window.current.Close()));
		canvas.root.AddChild(layout);
		
		// Liste des niveaux:
		canvas.root.AddChild(
			levels = new Panel
			{
				active = false,
				pivot = new Vector2(0.5F),
				anchors = new Vector2(0.5F),
				padding = new Region(20),
				size = new Vector2(500, 200)
			}
		);
		var levelButtons = new Layout
		{
			pivot = new Vector2(0.5F),
			anchors = new Vector2(0.5F),
			orientation = Orientation.Horizontal,
			spacing = 10,
		};
		levelButtons.AddChild(new ButtonLevel(new Vector2Int(9, 9), 10));
		levelButtons.AddChild(new ButtonLevel(new Vector2Int(16, 16), 40));
		levelButtons.AddChild(new ButtonLevel(new Vector2Int(30, 16), 99));
		levels.AddChild(levelButtons);
	}
	
	public override void Update() { }
}