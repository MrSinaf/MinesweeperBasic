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
	private readonly Button quitButton;
	
	public GameUI()
	{
		anchorMin = Vector2.zero;
		anchorMax = Vector2.one;
		
		var uiTextures = Vault.GetAsset<Texture2D>("ui")!;
		var panel = new Panel
		{
			material = new MaterialUI().SetTexture(uiTextures)
									   .SetNinePatch(new Region(5), 3),
			uv = uiTextures.GetUVRegion(new RectInt(32, 32, 16, 16)),
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
				pivot = new Vector2(0, 0.5F),
				anchors = new Vector2(0, 0.5F)
			}
		);
		panel.AddChild(
			bombsLeft = new Label("0")
			{
				position = new Vector2(-18, 0),
				pivot = new Vector2(1, 0.5F),
				anchors = new Vector2(1, 0.5F)
			}
		);
		panel.AddChild(
			new Image(uiTextures)
			{
				// Texture de bombe
				uv = uiTextures.GetUVRegion(new RectInt(132, 35, 8, 10)),
				size = new Vector2(8, 10),
				scale = new Vector2(1.5F),
				pivot = new Vector2(1, 0.5F),
				anchors = new Vector2(1, 0.5F)
			}
		);
		AddChild(
			quitButton = new Button("Quitter", () => Stage.Load(new Menu()).Wait())
			{
				size = new Vector2(100, 35),
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
			new Label("Vous avez gagnez !")
			{
				pivot = new Vector2(0.5F, 1),
				anchors = new Vector2(0.5F, 1),
				tint = Color.green
			}
		);
		panel.AddChild(
			new Label($"Temps écoulé : {timer.text}")
			{
				pivot = new Vector2(0.5F),
				anchors = new Vector2(0.5F),
			}
		);
		quitButton.mesh = null;
		quitButton.pivot = quitButton.anchors = new Vector2(0.5F, 0);
		panel.AddChild(quitButton);
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
			new Label("Vous avez perdu !")
			{
				pivot = new Vector2(0.5F, 1),
				anchors = new Vector2(0.5F, 1),
				tint = Color.red
			}
		);
		panel.AddChild(
			new Label($"Temps écoulé : {timer.text}\nBombes restantes : {bombsLeft}")
			{
				pivot = new Vector2(0.5F),
				anchors = new Vector2(0.5F),
			}
		);
		quitButton.mesh = null;
		quitButton.pivot = quitButton.anchors = new Vector2(0.5F, 0);
		panel.AddChild(quitButton);
		AddChild(panel);
	}
	
	public void UpdateTimer(float timer)
	{
		this.timer.text = $"{(int)timer / 60:00}:{timer % 60:00}";
	}
	
	public void UpdateBombs(int bomb)
		=> bombsLeft.text = bomb.ToString();
}