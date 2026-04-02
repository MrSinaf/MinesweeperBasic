using Ratelite;
using Ratelite.Resources;
using Ratelite.UI;
using Ratelite.UI.Widgets;

namespace MinesweeperBasic.Scenes;

public class Menu : Scene
{
	private Canvas canvas = null!;
	
	public override void Init()
		=> canvas = AddPlugin<Canvas>();
	
	public override void Start()
	{
		canvas.root.padding = new Region(10);
		canvas.root.AddChild(
			new Image(Vault.GetAsset<Texture2D>("ui")!)
			{
				name = "title",
				pivot = new Vector2(0.5F, 1),
				anchors = new Vector2(0.5F, 0.75F),
				scale = new Vector2(3F),
				uv = Vault.GetAsset<Texture2D>("ui").GetUVRegion(new RectInt(0, 0, 228, 32)),
			}
		);
		
		var layout = new Layout
		{
			spacing = 5,
			pivot = new Vector2(0.5F, 0),
			anchors = new Vector2(0.5F, 0),
		};
		layout.AddChild(
			new Button("Jouer", () => Stage.Load(new Game(new Vector2Int(12), 20)).Wait())
		);
		layout.AddChild(
			new Button("Apprendre", () => Stage.Load(new Learn()).Wait())
		);
		layout.AddChild(new Button("Quitter", () => Window.current.Close()));
		canvas.root.AddChild(layout);
	}
	
	public override void Update() { }
}