using MinesweeperBasic.Scenes;
using XYEngine;
using XYEngine.GO;
using XYEngine.Resources;
using XYEngine.UI;
using XYEngine.UI.Widgets;

XY.CreateGame()
  .SetIcon("assets/icon.png")
  .AddModule<GOModule>()
  .AddModule<UIModule>()
  .SetStartingScene<Splash>()
  .SetWindowOptions(new WindowOptions("Minesweeper BASIC", 720, 460)
  {
	  resizable = false
  })
  .LoadingAssets(async progress =>
  {
	  UIPrefab.Add<Button>(string.Empty, ButtonPrefab);
	  
	  await Vault.LoadResource<Texture2D>("textures/purrvert.png", "purrvert-icon");
	  progress.Report(0.25F);
	  await Vault.LoadResource<Texture2D>("textures/title.png", "title");
	  progress.Report(0.50F);
	  await Vault.LoadResource<Texture2D>("textures/tiles.png", "tiles");
	  progress.Report(0.75F);
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
	e.uv = texture.GetUVRegion(new RectInt(0, 0, 16, 16));
	e.size = new Vector2(200, 35);
	
	e.label.pivot = e.label.anchors = new Vector2(0.5F);
	e.label.position = new Vector2(0, 2);
	e.label.tint = Color.white;
	
	UIEvent.Register(e, UIEvent.Type.CursorEnter, OnCursorEnter);
	UIEvent.Register(e, UIEvent.Type.CursorExit, OnCursorExit);
	
	void OnCursorEnter(UIElement e) => ((Button)e).label.tint = Color.green;
	void OnCursorExit(UIElement e) => ((Button)e).label.tint = Color.white;
}