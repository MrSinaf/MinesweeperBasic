using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SPEngine;
using SPEngine.UI;

namespace MinesweeperBasic;

public class MainMenu : Scene
{
    private UIElement mainMenu;
    private UIElement levels;
    private UIElement options;

    private int currentSelectDifficulty = -1;

    protected override void Start()
    {
        _ = new GameManager();
        var title = new Image(ContentLoader.LoadTexture("Textures/Title")) { position = new Vector2(0, 5) }.AddInRoot();
        title.SetPivotAndAnchors(PositionAlignment.TopCenter);
        CreateMainMenu();
        CreateSelectLevels();
        CreateInfos();
    }

    protected override void Update()
    {
        if (Input.IsKeyPressed(Keys.A))
        {
            SceneManager.SetMain(new Game(new Point(16, 16), 42, 3));
        }
    }

    private void CreateMainMenu()
    {
        mainMenu = UIElement.CreateContainer().AddInRoot();

        var layout = new Layout { position = new Vector2(0, -5), spacing = 3, vertical = true };
        layout.SetPivotAndAnchors(PositionAlignment.DownCenter);
        mainMenu.AddChild(layout);

        var playButton = new Button("Jouer");
        playButton.clicked += () =>
        {
            mainMenu.active = false;
            levels.active = true;
        };
        layout.AddChild(playButton);
        
        var infosButton = new Button("Infos");
        infosButton.clicked += () =>
        {
            mainMenu.active = false;
            options.active = true;
        };
        layout.AddChild(infosButton);

        var quitButton = new Button("Quitter");
        quitButton.clicked += GameMain.QuitGame;
        layout.AddChild(quitButton);
    }

    private void CreateSelectLevels()
    {
        levels = UIElement.CreateContainer().AddInRoot();

        var returnButton = new Button("Retour") { position = new Vector2(0, -5) };
        returnButton.SetPivotAndAnchors(PositionAlignment.DownCenter);
        returnButton.clicked += () =>
        {
            levels.active = false;
            mainMenu.active = true;
        };
        levels.AddChild(returnButton);

        var selects = new Layout { spacing = 15, vertical = true };
        selects.SetPivotAndAnchors(PositionAlignment.MiddleCenter);
        levels.AddChild(selects);

        var description = new Panel(new StretchTexture(GameManager.uiAtlas.texture, GameManager.uiAtlas.atlasRegions[3], new Vector2(6), new Vector2(6), 2))
            { size = new Vector2(100, 60), active = false };
        description.SetPivotAndAnchors(PositionAlignment.MiddleCenter);

        var descriptionLabel = new Label(UIManager.fonts["Default"]);
        descriptionLabel.SetPivotAndAnchors(PositionAlignment.MiddleCenter);
        description.AddChild(descriptionLabel);

        var buttons = new Layout { spacing = 10 };
        buttons.SetPivotAndAnchors(PositionAlignment.MiddleCenter);

        var easy = new Button("Facile");
        easy.clicked += () => SceneManager.SetMain(new Game(new Point(9, 9), 10, 3));
        UIEventManager.Register(easy, UIEventType.MouseEnter, () => OnButtonOver(0));
        UIEventManager.Register(easy, UIEventType.MouseExit, () => OnButtonExit(0));
        buttons.AddChild(easy);

        var normal = new Button("Normal");
        normal.clicked += () => SceneManager.SetMain(new Game(new Point(16, 16), 42, 3));
        UIEventManager.Register(normal, UIEventType.MouseEnter, () => OnButtonOver(1));
        UIEventManager.Register(normal, UIEventType.MouseExit, () => OnButtonExit(1));
        buttons.AddChild(normal);

        var hard = new Button("Difficile");
        hard.clicked += () => SceneManager.SetMain(new Game(new Point(32, 16), 100, 3));
        UIEventManager.Register(hard, UIEventType.MouseEnter, () => OnButtonOver(2));
        UIEventManager.Register(hard, UIEventType.MouseExit, () => OnButtonExit(2));
        buttons.AddChild(hard);
        selects.AddChild(buttons);
        selects.AddChild(description);

        levels.active = false;
        return;

        void OnButtonExit(int difficulty)
        {
            if (currentSelectDifficulty == difficulty)
                description.active = false;
        }

        void OnButtonOver(int difficulty)
        {
            description.active = true;
            descriptionLabel.text = difficulty switch
            {
                0 => "Taille : 9x9\nBombes : 10",
                1 => "Taille : 16x16\nBombes : 42",
                2 => "Taille : 32x16\nBombes : 100",
                _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
            };

            currentSelectDifficulty = difficulty;
        }
    }

    private void CreateInfos()
    {
        options = UIElement.CreateContainer().AddInRoot();

        var returnButton = new Button("Retour") { position = new Vector2(0, -5) };
        returnButton.SetPivotAndAnchors(PositionAlignment.DownCenter);
        returnButton.clicked += () =>
        {
            options.active = false;
            mainMenu.active = true;
        };
        options.AddChild(returnButton);

        var panel = new Panel(new StretchTexture(GameManager.uiAtlas.texture, GameManager.uiAtlas.atlasRegions[3], new Vector2(6), new Vector2(6), 2))
            { size = new Vector2(400), anchorMin = new Vector2(.5F, 0), anchorMax = new Vector2(.5F, 1), position00 = new Point(0, 80), position11 = new Point(0, 50) };
        panel.SetPivot(PositionAlignment.MiddleCenter);
        options.AddChild(panel);

        var text = new Label(UIManager.fonts["Default"])
        {
            text = "Jeu créé par Mickaël Dancoisne (MrSinaf)\n" +
                   "En utilisant la première version de SPEngine\n\n" +
                   "Conseils :\n" +
                   "F : Fenêtré / Sans bordure / Plein écran\n" +
                   "R : pour relancer le niveau.\n" +
                   "Escape : pour revenir au menu principal.",
            position = new Vector2(10, 5)
        };
        panel.AddChild(text);

        options.active = false;
    }
}