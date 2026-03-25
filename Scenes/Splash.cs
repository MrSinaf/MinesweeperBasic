using XYEngine;
using XYEngine.Resources;
using XYEngine.UI;
using XYEngine.UI.Widgets;

namespace MinesweeperBasic.Scenes;

public class Splash : Scene
{
	private Canvas canvas = null!;
	private Image icon = null!;
	private float time;
	
	public override void Init()
	{
		canvas = AddPlugin<Canvas>();
	}
	public override void Start()
	{
		canvas.root.AddChild(icon = new Image(Vault.GetAsset<Texture2D>("purrvert-icon")!)
		{
			pivot = new Vector2(0.5F),
			anchors = new Vector2(0.5F),
			scale = new Vector2(5)
		});
	}
	
	public override void Update()
	{
		time += Time.delta;
		
		icon.opacity = time;
		if (time > 1)
		{
			icon.position = new Vector2(
				Random.Shared.Next(-5, 5),
				Random.Shared.Next(-5, 5)
			);
			icon.scale += new Vector2(Time.delta * 2);
		}
		
		if (time > 2)
			_ = Stage.Load(new Menu());
	}
}