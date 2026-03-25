using XYEngine;
using XYEngine.Resources;
using XYEngine.UI;
using XYEngine.UI.Widgets;

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
			new Image(Vault.GetAsset<Texture2D>("title")!)
			{
				pivot = new Vector2(0.5F, 1),
				anchors = new Vector2(0.5F, 0.75F),
				scale = new Vector2(1.25F)
			}
		);
		
		var layout = new Layout
		{
			spacing = 5,
			pivot = new Vector2(0.5F, 0),
			anchors = new Vector2(0.5F, 0),
		};
		layout.AddChild(new Button("Jouer", () => _ = Stage.Load(new Game(new Vector2Int(10)))));
		layout.AddChild(new Button("Apprendre", () => Log.Verbose("'Learn' clicked")));
		layout.AddChild(new Button("Quitter", () => Window.current.Close()));
		canvas.root.AddChild(layout);
	}
	
	public override void Update()
	{
		
	}
}