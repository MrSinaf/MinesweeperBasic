using MinesweeperBasic.UI;
using Ratelite;
using Ratelite.Resources;
using Ratelite.Sounds;
using Ratelite.UI;
using Ratelite.UI.Widgets;

namespace MinesweeperBasic.Scenes;

public class Menu : Scene
{
	private static AudioSource? music;
	public static float effectVolume { get; private set; } = 1;
	
	private Canvas canvas = null!;
	private Panel levels = null!;
	
	public override void Init()
	{
		canvas = AddPlugin<Canvas>();
		
		if (music == null)
		{
			music = new AudioSource
			{
				looping = true,
				audio = Vault.GetAsset<AudioClip>("music")
			};
			music.Play();
		}
	}
	
	public override void Start()
	{
		var texture = Vault.GetAsset<Texture2D>("ui")!;
		canvas.root.padding = new Region(10);
		canvas.root.AddChild(
			new Image(texture)
			{
				name = "title",
				pivot = new Vector2(0.5F, 1),
				anchors = new Vector2(0.5F, 0.75F),
				scale = new Vector2(3F),
				uv = texture.GetUVRegion(new RectInt(0, 0, 228, 32)),
			}
		);
		
		// Liste de boutons:
		var layout = new Layout
		{
			spacing = 5,
			pivot = new Vector2(0.5F, 0),
			anchors = new Vector2(0.5F, 0),
		};
		layout.AddChild(
			new Button(
				Local.Get("button.play"),
				() =>
				{
					levels.active = !levels.active;
					levels.MoveOnTop();
				}
			)
		);
		layout.AddChild(new Button(Local.Get("button.exit"), () => Window.current.Close()));
		canvas.root.AddChild(layout);
		
		// Liste des niveaux:
		canvas.root.AddChild(
			levels = new Panel
			{
				active = false,
				pivot = new Vector2(0.5F),
				anchors = new Vector2(0.5F),
				padding = new Region(20),
				size = new Vector2(500, 200)
			}
		);
		var levelButtons = new Layout
		{
			pivot = new Vector2(0.5F),
			anchors = new Vector2(0.5F),
			orientation = Orientation.Horizontal,
			spacing = 10,
		};
		levelButtons.AddChild(new ButtonLevel(new Vector2Int(9, 9), 10));
		levelButtons.AddChild(new ButtonLevel(new Vector2Int(16, 16), 40));
		levelButtons.AddChild(new ButtonLevel(new Vector2Int(30, 16), 99));
		levels.AddChild(levelButtons);
		
		// Paramètres:
		var mainLayout = new Layout
		{
			pivot = new Vector2(1, 0),
			anchors = new Vector2(1, 0),
			orientation = Orientation.Vertical,
			spacing = 10,
			alignment = 1
		};
		
		var musicLayout = new Layout { orientation = Orientation.Vertical, alignment = 1 };
		musicLayout.AddChild(new Label(Local.Get("slider.volume.music")));
		musicLayout.AddChild(
			new Slider(x => music!.volume = x, Orientation.Horizontal)
			{
				value = music!.volume
			}
		);
		mainLayout.AddChild(musicLayout);
		
		
		var effectLayout = new Layout { orientation = Orientation.Vertical, alignment = 1 };
		effectLayout.AddChild(new Label(Local.Get("slider.volume.effects")));
		effectLayout.AddChild(
			new Slider(x => effectVolume = x, Orientation.Horizontal)
			{
				value = effectVolume
			}
		);
		mainLayout.AddChild(effectLayout);
		
		var langLayout = new Layout
		{
			orientation = Orientation.Horizontal, spacing = 5
		};
		langLayout.AddChild(
			new ButtonFlag(
				0,
				() =>
				{
					Local.Load("fr");
					Stage.Load(new Menu()).Wait();
				}
			)
		);
		langLayout.AddChild(
			new ButtonFlag(
				1,
				() =>
				{
					Local.Load("en");
					Stage.Load(new Menu()).Wait();
				}
			)
		);
		mainLayout.AddChild(langLayout);
		
		canvas.root.AddChild(mainLayout);
	}
	
	public override void Update() { }
}