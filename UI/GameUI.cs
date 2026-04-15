using MinesweeperBasic.Scenes;
using Ratelite;
using Ratelite.Resources;
using Ratelite.UI;
using Ratelite.UI.Widgets;

namespace MinesweeperBasic.UI;

public sealed class GameUI : UIElement
{
	private readonly Label timer;
	private readonly Label bombsLeft;
	
	public GameUI()
	{
		anchorMin = Vector2.zero;
		anchorMax = Vector2.one;
		isInteractif = false;
		
		var uiTextures = Vault.GetAsset<Texture2D>("ui")!;
		var panel = new Panel
		{
			material = new MaterialUI().SetTexture(uiTextures)
									   .SetNinePatch(new Region(5), 3),
			uv = uiTextures.GetUVRegion(new RectInt(80, 32, 16, 16)),
			cornerRadius = Region.zero,
			tint = Color.white,
			pivot = new Vector2(0.5F, 1),
			anchors = new Vector2(0.5F, 1),
			size = new Vector2(150, 35),
			padding = new Region(12, 5, 12, 0)
		};
		AddChild(panel);
		panel.AddChild(
			timer = new Label("00:00")
			{
				name = "timer",
				pivot = new Vector2(0, 0.5F),
				anchors = new Vector2(0, 0.5F)
			}
		);
		panel.AddChild(
			bombsLeft = new Label("0")
			{
				name = "bombsLeft",
				position = new Vector2(-18, 0),
				pivot = new Vector2(1, 0.5F),
				anchors = new Vector2(1, 0.5F)
			}
		);
		panel.AddChild(
			new Image(uiTextures)
			{
				name = "bomb",
				uv = uiTextures.GetUVRegion(new RectInt(212, 35, 8, 10)),
				size = new Vector2(8, 10),
				scale = new Vector2(1.5F),
				pivot = new Vector2(1, 0.5F),
				anchors = new Vector2(1, 0.5F)
			}
		);
	}
	
	public void ShowWinPanel()
	{
		this[0].active = false;
		
		var panel = new Panel
		{
			pivot = new Vector2(0.5F),
			anchors = new Vector2(0.5F),
			size = new Vector2(250, 150)
		};
		panel.AddChild(
			new Label(Local.Get("dialog.win.title"))
			{
				pivot = new Vector2(0.5F, 1),
				anchors = new Vector2(0.5F, 1),
				tint = Color.green
			}
		);
		panel.AddChild(
			new Label(Local.Get("dialog.win.timer", timer.text))
			{
				pivot = new Vector2(0.5F),
				anchors = new Vector2(0.5F),
			}
		);
		panel.AddChild(new Button(Local.Get("button.exit"), () => Stage.Load(new Menu()).Wait())
		{
			mesh = null,
			size = new Vector2(100, 35),
			pivot = new Vector2(0.5F, 0),
			anchors = new Vector2(0.5F, 0)
		});
		AddChild(panel);
	}
	
	public void ShowLoosePanel(int bombsLeft)
	{
		this[0].active = false;
		
		var panel = new Panel
		{
			pivot = new Vector2(0.5F),
			anchors = new Vector2(0.5F),
			size = new Vector2(250, 150)
		};
		panel.AddChild(
			new Label(Local.Get("dialog.lose.title"))
			{
				pivot = new Vector2(0.5F, 1),
				anchors = new Vector2(0.5F, 1),
				tint = Color.red
			}
		);
		panel.AddChild(
			new Label(
				Local.Get("dialog.lose.timer", timer.text) + "\n" +
				Local.Get("dialog.lose.mine", bombsLeft)
			)
			{
				pivot = new Vector2(0.5F),
				anchors = new Vector2(0.5F),
			}
		);
		panel.AddChild(new Button(Local.Get("button.exit"), () => Stage.Load(new Menu()).Wait())
		{
			mesh = null,
			size = new Vector2(100, 35),
			pivot = new Vector2(0.5F, 0),
			anchors = new Vector2(0.5F, 0)
		});
		AddChild(panel);
	}
	
	public void UpdateTimer(float timer)
	{
		this.timer.text = $"{(int)timer / 60:00}:{timer % 60:00}";
	}
	
	public void UpdateBombs(int bomb)
		=> bombsLeft.text = bomb.ToString();
}