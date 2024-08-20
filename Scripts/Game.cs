using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SPEngine;
using SPEngine.UI;

namespace MinesweeperBasic;

public class Game(Point size, int nBomb, int nLife) : Scene
{
    private Map map;
    public Label time { get; private set; }
    public Label life { get; private set; }
    public Label bombs { get; private set; }

    public Panel popup;

    protected override void Start()
    {
        CreateUI();
        map = new Map(new AtlasData(ContentLoader.LoadData<AtlasDataConfig>("Minesweeper.atlas")), size.X, size.Y, nBomb, nLife);
    }

    protected override void Update()
    {
        if (Input.IsKeyPressed(Keys.Escape))
            SceneManager.SetMain(new MainMenu());
        
        if (Input.IsKeyPressed(Keys.R))
            map.Recreate();
    }

    private void CreateUI()
    {
        _ = new GameManager();

        var uiAtlas = new AtlasData(ContentLoader.LoadData<AtlasDataConfig>("UI.atlas"));
        var defaultFont = UIManager.fonts["Default"];

        var stretchTexture = new StretchTexture(uiAtlas.texture, uiAtlas.atlasRegions[1], new Vector2(4, 3), new Vector2(4, 5), 2);
        var panelTop = new Panel(stretchTexture) { size = new Vector2(200, 30) };
        panelTop.SetPivotAndAnchors(PositionAlignment.TopCenter);
        panelTop.AddInRoot();

        var timePanel = new Panel(new StretchTexture(uiAtlas.texture, uiAtlas.atlasRegions[6], new Vector2(2), new Vector2(2), 2))
            { position = new Vector2(0, -3), size = new Vector2(60, 16) };
        timePanel.SetPivotAndAnchors(PositionAlignment.MiddleCenter);
        panelTop.AddChild(timePanel);

        time = new Label(defaultFont);
        time.SetPivotAndAnchors(PositionAlignment.MiddleCenter);
        timePanel.AddChild(time);

        var bombIcon = new Image(uiAtlas.texture, uiAtlas.atlasRegions[4]) { position = new Vector2(-8, -3) };
        bombIcon.SetPivotAndAnchors(PositionAlignment.MiddleRight);
        panelTop.AddChild(bombIcon);

        bombs = new Label(defaultFont) { position = new Vector2(-3, 0) };
        bombs.SetAnchors(PositionAlignment.MiddleLeft);
        bombs.SetPivot(PositionAlignment.MiddleRight);
        bombIcon.AddChild(bombs);

        var lifeIcon = new Image(uiAtlas.texture, uiAtlas.atlasRegions[5]) { position = new Vector2(8, -3) };
        lifeIcon.SetPivotAndAnchors(PositionAlignment.MiddleLeft);
        panelTop.AddChild(lifeIcon);

        life = new Label(defaultFont) { position = new Vector2(3, 0) };
        life.SetAnchors(PositionAlignment.MiddleRight);
        life.SetPivot(PositionAlignment.MiddleLeft);
        lifeIcon.AddChild(life);

        popup = new Panel(new StretchTexture(uiAtlas.texture, uiAtlas.atlasRegions[3], new Vector2(6), new Vector2(6), 2))
            { active = false, size = new Vector2(400, 35), position = new Vector2(0, -15) };
        popup.SetPivotAndAnchors(PositionAlignment.DownCenter);
        popup.AddInRoot();

        var label = new Label(UIManager.fonts["Default"]) {text = "Tu as perdu ;-;"};
        label.SetPivotAndAnchors(PositionAlignment.MiddleCenter);
        popup.AddChild(label);
    }
}