using Ratelite;
using Ratelite.UI;
using Ratelite.UI.Widgets;

namespace MinesweeperBasic.Scenes;

public class Learn : Scene
{
	private Canvas canvas = null!;
	
	public override void Init()
	{
		canvas = AddPlugin<Canvas>();
	}
	
	public override void Start()
	{
		var panel = new Panel
		{
			margin = new Region(10, 60, 10, 10),
		};
		canvas.root.AddChild(
			new Button(Local.Get("button.back"), () => Stage.Load(new Menu()).Wait())
			{
				pivot = new Vector2(0.5F, 0),
				anchors = new Vector2(0.5F, 0),
				position = new Vector2(0, 10),
			}
		);
		canvas.root.AddChild(panel);
	}
}