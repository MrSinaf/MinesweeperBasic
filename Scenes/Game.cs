using XYEngine;
using XYEngine.GO;
using XYEngine.UI;

namespace MinesweeperBasic.Scenes;

public enum TileType { Closed, Opened, Questionned, Flagged }

public class Game(Vector2Int size) : Scene
{
	public const int TILE_SIZE = 32;
	
	private readonly (TileType type, bool isBomb)[,] tiles = new (TileType, bool)[size.x, size.y];
	
	private World world = null!;
	private Canvas canvas = null!;
	
	private Map map = null!;
	private bool moveCamera;
	
	public override void Init()
	{
		world = AddPlugin<World>();
		canvas = AddPlugin<Canvas>();
		
		XY.game.window.mouseButtonPressed += OnMouseButtonPressed;
		XY.game.window.mouseButtonReleased += OnMouseButtonReleased;
		XY.game.window.cursorMoved += OnCursorMoved;
		XY.game.window.keyPressed += OnKeyPressed;
	}
	
	public override void Start()
	{
		var bombs = new Vector2[10];
		for (var i = 0; i < 10; i++)
		{
			Vector2 position;
			do
			{
				position = new Vector2(Random.Shared.Next(size.x), Random.Shared.Next(size.y));
			} while (bombs.Contains(position));
			bombs[i] = position;
		}
		
		for (var x = 0; x < size.x; x++)
		for (var y = 0; y < size.y; y++)
			tiles[x, y] = (TileType.Closed, bombs.Contains(new Vector2(x, y)));
		
		world.camera.position = size * TILE_SIZE * 0.5F;
		world.AddObject(map = new Map(size));
	}
	
	private void OnMouseButtonPressed(MouseButton button)
	{
		if (button == MouseButton.Middle)
			moveCamera = true;
		else
		{
			var position = (world.camera.ScreenToWorldPosition(XY.game.window.cursorPosition)
							/ TILE_SIZE).ToVector2Int(RoundingMode.Floor);
			
			if (!IsValidPosition(position))
				return;
			
			if (button == MouseButton.Left)
				OnTile(position);
			else if (button == MouseButton.Right)
			{
				ref var tile = ref tiles[position.x, position.y];
				switch (tile.type)
				{
					case TileType.Opened:
						return;
					case TileType.Closed:
						map.UpdateTileUV(position, tile.type = TileType.Questionned);
						break;
					case TileType.Questionned:
						map.UpdateTileUV(position, tile.type = TileType.Flagged);
						break;
					case TileType.Flagged:
						map.UpdateTileUV(position, tile.type = TileType.Closed);
						break;
				}
			}
			
			map.ApplyUpdate();
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
		if (key == Key.LeftCtrl)
			world.camera.position = size * TILE_SIZE * 0.5F;
	}
	
	private void OnTile(Vector2Int position)
	{
		ref var tile = ref tiles[position.x, position.y];
		if (tile.type == TileType.Opened)
			return;
		
		if (tile.isBomb)
			map.UpdateTileUVAsBomb(position);
		else
			RevealTile(position);
		
		void RevealTile(Vector2Int position)
		{
			if (!IsValidPosition(position))
				return;
			
			ref var tile = ref tiles[position.x, position.y];
			if (tile.type == TileType.Opened)
				return;
			
			tile.type = TileType.Opened;
			
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
				RevealTile(position + new Vector2Int(1, 0));
				RevealTile(position + new Vector2Int(-1, 0));
				RevealTile(position + new Vector2Int(0, 1));
				RevealTile(position + new Vector2Int(0, -1));
			}
		}
	}
	
	private bool IsValidPosition(Vector2Int position)
		=> position.x >= 0 && position.y >= 0 && position.x < size.x && position.y < size.y;
}