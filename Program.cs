using MinesweeperBasic.Scenes;
using Ratelite;
using Ratelite.GO;
using Ratelite.Resources;
using Ratelite.UI;
using Ratelite.UI.Widgets;

R.CreateGame()
  .SetIcon("assets/icon.png")
  .AddModule<GOModule>()
  .AddModule<UIModule>()
  .SetStartingScene<Splash>()
  .SetWindowOptions(new WindowOptions("Minesweeper BASIC", 960, 550)
  {
	  resizable = false
  })
  .LoadingAssets(async progress =>
  {
	  UIPrefab.Add<Button>(string.Empty, ButtonPrefab);
	  UIPrefab.Add<Panel>(string.Empty, PanelPrefab);
	  
	  await Vault.LoadResource<Texture2D>("textures/purrvert.png", "purrvert-icon");
	  progress.Report(0.25F);
	  await Vault.LoadResource<Texture2D>("textures/tiles.png", "tiles");
	  progress.Report(0.5F);
	  await Vault.LoadResource<Texture2D>("textures/ui.png", "ui");
	  progress.Report(1);
  })
  .Run();


void ButtonPrefab(Button e)
{
	e.mesh = Vault.GetAsset<Mesh>(UIModule.DEFAULT_MESH);
	var texture = Vault.GetAsset<Texture2D>("ui")!;
	e.material = new MaterialUI().SetTexture(texture)
								 .SetNinePatch(new Region(new Vector2(3, 2), new Vector2(3, 4)), 3);
	e.uv =  texture.GetUVRegion(new RectInt(0, 32, 16, 16));
	e.size = new Vector2(200, 35);
	
	e.label.pivot = e.label.anchors = new Vector2(0.5F);
	e.label.position = new Vector2(0, 2);
	e.label.tint = Color.white;
	
	e.cursorEnter += OnCursorEnter;
	e.cursorExit += OnCursorExit;
	e.onPressed += OnPressed;
	e.onReleased += OnReleased;
	
	void OnCursorEnter(UIElement e) => ((Button)e).label.tint = Color.green;
	void OnCursorExit(UIElement e) => ((Button)e).label.tint = Color.white;
	void OnPressed(UIElement e)
	{
		if (e is Button b)
		{
			b.label.position = new Vector2(0, 1);
			b.uv = texture.GetUVRegion(new RectInt(16, 32, 16, 16));
		}
	}
	void OnReleased(UIElement e)
	{
		if (e is Button b)
		{
			b.label.position = new Vector2(0, 2);
			e.uv = texture.GetUVRegion(new RectInt(0, 32, 16, 16));
		}
	}
}

void PanelPrefab(Panel e)
{
	var texture = Vault.GetAsset<Texture2D>("ui")!;
	e.mesh = Vault.GetAsset<Mesh>(UIModule.DEFAULT_MESH);
	e.padding = new Region(10);
	e.anchorMin = Vector2.zero;
	e.anchorMax = Vector2.one;
	e.material = new MaterialUI().SetTexture(texture)
								 .SetNinePatch(new Region(6), 3);
	e.cornerRadius = Region.zero;
	e.uv = texture.GetUVRegion(new RectInt(64, 32, 16, 16));
}