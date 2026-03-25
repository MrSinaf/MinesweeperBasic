using XYEngine;
using XYEngine.GO;
using XYEngine.Resources;
using XYEngine.Utils;

namespace MinesweeperBasic.Scenes;

public enum TileType { Closed, Opened, Flagged }

public class Game(Vector2Int size) : Scene
{
	private const int TILE_SIZE = 32;
	private readonly RectInt[] tilesN =
	[
		new (48, 0, 16, 16), new (0, 16, 16, 16), new (16, 16, 16, 16), new (32, 16, 16, 16),
		new (48, 16, 16, 16), new (0, 32, 16, 16), new (16, 32, 16, 16), new (32, 32, 16, 16),
		new (48, 32, 16, 16)
	];
	
	private readonly (TileType type, bool isBomb)[,] tiles = new (TileType, bool)[size.x, size.y];
	
	private World world = null!;
	private XYObject map = null!;
	private Texture2D tilesTexture = null!;
	private bool moveCamera;
	
	public override void Init()
	{
		world = AddPlugin<World>();
		XY.game.window.mouseButtonPressed += OnMouseButtonPressed;
		XY.game.window.mouseButtonReleased += OnMouseButtonReleased;
		XY.game.window.cursorMoved += OnCursorMoved;
		XY.game.window.keyPressed += OnKeyPressed;
	}
	
	public override void Start()
	{
		tilesTexture = Vault.GetAsset<Texture2D>("tiles")!;
		
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
		
		var meshes = new (Rect vertices, Region uvs)[size.x * size.y];
		for (var x = 0; x < size.x; x++)
		for (var y = 0; y < size.y; y++)
		{
			var position = new Vector2(x, y);
			tiles[x, y] = (TileType.Closed, bombs.Contains(position));
			meshes[x + y * size.x] = (
				new Rect(position * TILE_SIZE, new Vector2(TILE_SIZE)),
				tilesTexture.GetUVRegion(new RectInt(0, 0, 16, 16))
			);
		}
		
		world.camera.position = size * TILE_SIZE * 0.5F;
		world.AddObject(
			map = new XYObject
			{
				material = new MaterialObject().SetTexture(tilesTexture),
				mesh = MeshFactory.CreateQuads(meshes).Apply()
			}
		);
	}
	
	private void OnMouseButtonPressed(MouseButton button)
	{
		if (button == MouseButton.Left)
		{
			OnTile(
				(world.camera.ScreenToWorldPosition(XY.game.window.cursorPosition) / TILE_SIZE)
				.ToVector2Int(RoundingMode.Floor)
			);
			
			map.mesh.Apply();
		}
		else if (button == MouseButton.Right)
			moveCamera = true;
	}
	
	private void OnMouseButtonReleased(MouseButton button)
	{
		if (button == MouseButton.Right)
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
		if (!IsValidPosition(position))
			return;
		
		var tile = tiles[position.x, position.y];
		if (tile.type == TileType.Opened)
			return;
		
		if (tile.isBomb)
		{
			tile.type = TileType.Opened;
			UpdateTileUV(position, tilesTexture.GetUVRegion(new RectInt(0, 48, 16, 16)));
		}
		else
			RevealTile(position);
		
		void RevealTile(Vector2Int position)
		{
			if (!IsValidPosition(position))
				return;
			
			var tile = tiles[position.x, position.y];
			if (tile.type == TileType.Opened)
				return;
			
			tiles[position.x, position.y].type = TileType.Opened;
			
			var bombCount = 0;
			for (var dx = -1; dx <= 1; dx++)
			for (var dy = -1; dy <= 1; dy++)
			{
				var nx = position.x + dx;
				var ny = position.y + dy;
				
				if (IsValidPosition(new Vector2Int(nx, ny)) && tiles[nx, ny].isBomb)
					bombCount++;
			}
			
			UpdateTileUV(position, tilesTexture.GetUVRegion(tilesN[bombCount]));
			if (bombCount == 0)
			{
				RevealTile(position + new Vector2Int(1, 0));
				RevealTile(position + new Vector2Int(-1, 0));
				RevealTile(position + new Vector2Int(0, 1));
				RevealTile(position + new Vector2Int(0, -1));
			}
		}
	}
	
	private void UpdateTileUV(Vector2Int position, Region uv)
	{
		var i = (position.x + position.y * size.x) * 4;
		map.mesh.vertices[i].uv = uv.position00;
		map.mesh.vertices[i + 1].uv = new Vector2(uv.position11.x, uv.position00.y);
		map.mesh.vertices[i + 2].uv = uv.position11;
		map.mesh.vertices[i + 3].uv = new Vector2(uv.position00.x, uv.position11.y);
	}
	
	private bool IsValidPosition(Vector2Int position)
		=> position.x >= 0 && position.y >= 0 && position.x < size.x && position.y < size.y;
}