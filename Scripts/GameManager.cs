using Microsoft.Xna.Framework;
using SPEngine;
using SPEngine.UI;
using SPEngine.UI.Styles;

namespace MinesweeperBasic;

public class GameManager
{
    public static AtlasData uiAtlas;

    public GameManager()
    {
        InitStyle();
    }
    
    private void InitStyle()
    {
        uiAtlas = new AtlasData(ContentLoader.LoadData<AtlasDataConfig>("UI.atlas"));
        StyleManager.AddDefaultStyle(typeof(Button), new ButtonStyle(new StretchTexture(uiAtlas.texture, uiAtlas.atlasRegions[0], new Vector2(3, 2), new Vector2(3, 4)))
        {
            upperLabel = true,
            size = new Vector2(80, 18),
            textAlignement = PositionAlignment.MiddleCenter,
            onMouseEnter = button => button.label.diffuseColor = new Vector3(0, 1, 0),
            onMouseExit = button => button.label.diffuseColor = Vector3.One
        });
    }
}