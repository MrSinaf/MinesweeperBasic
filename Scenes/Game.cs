using MinesweeperBasic.UI;
using Ratelite;
using Ratelite.GO;
using Ratelite.Sounds;
using Ratelite.UI;

namespace MinesweeperBasic.Scenes;

public enum TileType { Closed, Opened, Questionned, Flagged }

public class Game(Vector2Int size, int nBomb) : Scene
{
	public const int TILE_SIZE = 32;
	
	private readonly (TileType type, bool isBomb)[,] tiles = new (TileType, bool)[size.x, size.y];
	
	private World world = null!;
	private Canvas canvas = null!;
	
	private GameUI ui = null!;
	private Map map = null!;
	private bool moveCamera;
	private int bombsLeft { get; set => ui.UpdateBombs(field = value); }
	private readonly Vector2Int[] bombs = new Vector2Int[nBomb];
	
	private float timer;
	private bool finished;
	private int bombFlagged;
	private int tileOpenLeft;
	private bool firstClick = true;
	
	private AudioSource audioSource = null!;
	
	public override void Init()
	{
		world = AddPlugin<World>();
		canvas = AddPlugin<Canvas>();
		audioSource = new AudioSource();
		
		R.game.window.mouseButtonPressed += OnMouseButtonPressed;
		R.game.window.mouseButtonReleased += OnMouseButtonReleased;
		R.game.window.cursorMoved += OnCursorMoved;
		R.game.window.keyPressed += OnKeyPressed;
	}
	
	public override void Unload()
	{
		R.game.window.mouseButtonPressed -= OnMouseButtonPressed;
		R.game.window.mouseButtonReleased -= OnMouseButtonReleased;
		R.game.window.cursorMoved -= OnCursorMoved;
		R.game.window.keyPressed -= OnKeyPressed;
		audioSource.Dispose();
	}
	
	public override void Start()
	{
		audioSource.audio = Vault.GetAsset<AudioClip>("click");
		canvas.root.AddChild(ui = new GameUI());
		bombsLeft = nBomb;
		tileOpenLeft = size.x * size.y - nBomb;
		
		world.camera.position = size * TILE_SIZE * 0.5F;
		world.AddObject(map = new Map(size));
	}
	
	public override void Update()
	{
		if (!finished)
			ui.UpdateTimer(timer += Time.delta);
	}
	
	private void Loose()
	{
		audioSource.audio = Vault.GetAsset<AudioClip>("boom");
		audioSource.Play();
		foreach (var bomb in bombs)
			map.UpdateTileUVAsBomb(bomb);
		
		Task.Run(async () =>
			{
				var position = world.camera.position;
				for (var i = 0; i < 25; i++)
				{
					world.camera.position = position + new Vector2Int(
						Random.Shared.Next(-10, 10),
						Random.Shared.Next(-10, 10)
					);
					await Task.Delay(15);
				}
				world.camera.position = position;
				ui.ShowLoosePanel(bombs.Length - bombFlagged);
			}
		);
		
		finished = true;
	}
	
	private void OnMouseButtonPressed(MouseButton button)
	{
		if (canvas.hasElementHovered)
			return;
		
		if (button == MouseButton.Middle)
			moveCamera = true;
		else
		{
			var position = (world.camera.ScreenToWorldPosition(R.game.window.cursorPosition)
							/ TILE_SIZE).ToVector2Int(RoundingMode.Floor);
			
			if (!IsValidPosition(position))
				return;
			
			if (button == MouseButton.Left)
			{
				if (firstClick)
				{
					CreateBombs(position);
					firstClick = false;
				}
				
				OnTile(position);
			}
			else if (button == MouseButton.Right)
			{
				ref var tile = ref tiles[position.x, position.y];
				switch (tile.type)
				{
					case TileType.Opened:
						return;
					case TileType.Closed:
						map.UpdateTileUV(position, tile.type = TileType.Flagged);
						bombsLeft--;
						
						audioSource.Play();
						if (tile.isBomb)
							bombFlagged++;
						break;
					case TileType.Flagged:
						map.UpdateTileUV(position, tile.type = TileType.Questionned);
						bombsLeft++;
						audioSource.Play();
						
						if (tile.isBomb)
							bombFlagged--;
						break;
					case TileType.Questionned:
						map.UpdateTileUV(position, tile.type = TileType.Closed);
						break;
				}
			}
		}
		
		if (tileOpenLeft == 0)
		{
			finished = true;
			ui.ShowWinPanel();
		}
	}
	
	private void OnMouseButtonReleased(MouseButton button)
	{
		if (button == MouseButton.Middle)
			moveCamera = false;
	}
	
	private void OnCursorMoved(Vector2 delta)
	{
		if (moveCamera)
		{
			world.camera.position -= delta / world.camera.zoom;
		}
	}
	
	private void OnKeyPressed(Key key, int _)
	{
		switch (key)
		{
			case Key.LeftCtrl:
				world.camera.position = size * TILE_SIZE * 0.5F;
				break;
			case Key.Escape:
				Stage.Load(new Menu()).Wait();
				break;
			case Key.R:
				Stage.Load(new Game(size, nBomb)).Wait();
				break;
		}
	}
	
	private void OnTile(Vector2Int position)
	{
		if (finished)
			return;
		
		ref var tile = ref tiles[position.x, position.y];
		if (tile.type == TileType.Opened)
			return;
		
		if (tile.isBomb)
		{
			tile.type = TileType.Opened;
			map.UpdateTileUVAsBomb(position);
			Loose();
		}
		else
			RevealTile(position);
		
		void RevealTile(Vector2Int position)
		{
			if (!IsValidPosition(position))
				return;
			
			ref var tile = ref tiles[position.x, position.y];
			if (tile.type == TileType.Opened)
				return;
			
			if (tile.type == TileType.Flagged)
				bombsLeft++;
			
			tile.type = TileType.Opened;
			tileOpenLeft--;
			
			var bombCount = 0;
			for (var dx = -1; dx <= 1; dx++)
			for (var dy = -1; dy <= 1; dy++)
			{
				var nx = position.x + dx;
				var ny = position.y + dy;
				
				if (IsValidPosition(new Vector2Int(nx, ny)) && tiles[nx, ny].isBomb)
					bombCount++;
			}
			
			map.UpdateTileUV(position, bombCount);
			if (bombCount == 0)
			{
				for (var x = -1; x <= 1; x++)
				for (var y = -1; y <= 1; y++)
				{
					if (x == 0 && y == 0)
						continue;
					
					RevealTile(position + new Vector2Int(x, y));
				}
			}
		}
		
		map.ApplyUpdate();
	}
	
	private void CreateBombs(Vector2Int position)
	{
		for (var i = 0; i < bombs.Length; i++)
		{
			Vector2Int bombPos;
			do
			{
				bombPos = new Vector2Int(Random.Shared.Next(size.x), Random.Shared.Next(size.y));
			} while (bombs.Contains(bombPos) || bombPos.IsInsideBounds(
						 position - Vector2Int.one,
						 position + Vector2Int.one
					 ));
			bombs[i] = bombPos;
		}
		
		for (var x = 0; x < size.x; x++)
		for (var y = 0; y < size.y; y++)
			tiles[x, y] = (TileType.Closed, bombs.Contains(new Vector2Int(x, y)));
	}
	
	private bool IsValidPosition(Vector2Int position)
		=> position.x >= 0 && position.y >= 0 && position.x < size.x && position.y < size.y;
}