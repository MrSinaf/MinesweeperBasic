using MinesweeperBasic.Scenes;
using Ratelite;
using Ratelite.GO;
using Ratelite.Resources;
using Ratelite.Utils;

namespace MinesweeperBasic;

public class Map : RObject
{
	private readonly Texture2D texture;
	private readonly Vector2Int size;
	
	public Map(Vector2Int size)
	{
		material = new MaterialObject().SetTexture(texture = Vault.GetAsset<Texture2D>("tiles")!);
		
		this.size = size;
		var meshes = new (Rect vertices, Region uvs)[size.x * size.y];
		for (var x = 0; x < size.x; x++)
		for (var y = 0; y < size.y; y++)
		{
			var position = new Vector2(x, y);
			meshes[x + y * size.x] = (
				new Rect(position * Game.TILE_SIZE, new Vector2(Game.TILE_SIZE)),
				texture.GetUVRegion(new RectInt(0, 0, 16))
			);
		}
		mesh = MeshFactory.CreateQuads(meshes);
	}
	
	public void ApplyUpdate()
	{
		mesh.ApplyVertex();
	}
	
	public void UpdateTileUVAsBomb(Vector2Int position)
		=> UpdateTileUV(position, new RectInt(0, 48, 16));
	
	public void UpdateTileUV(Vector2Int position, TileType type)
	{
		UpdateTileUV(
			position,
			type switch
			{
				TileType.Closed => new RectInt(0, 0, 16),
				TileType.Flagged => new RectInt(16, 0, 16),
				TileType.Questionned => new RectInt(32, 0, 16),
				TileType.Opened => new RectInt(48, 0, 16),
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			}
		);
		
		var i = (position.x + position.y * size.x) * 4;
		mesh.ApplyVertex(i, 1);
		mesh.ApplyVertex(i + 1, 1);
		mesh.ApplyVertex(i + 2, 1);
		mesh.ApplyVertex(i + 3, 1);
	}
	
	public void UpdateTileUV(Vector2Int position, int n)
	{
		UpdateTileUV(
			position,
			n switch
			{
				0 => new RectInt(48, 0, 16),
				1 => new RectInt(0, 16, 16),
				2 => new RectInt(16, 16, 16),
				3 => new RectInt(32, 16, 16),
				4 => new RectInt(48, 16, 16),
				5 => new RectInt(0, 32, 16),
				6 => new RectInt(16, 32, 16),
				7 => new RectInt(32, 32, 16),
				8 => new RectInt(48, 32, 16),
				_ => throw new ArgumentOutOfRangeException(nameof(n), n, null)
			}
		);
	}
	
	private void UpdateTileUV(Vector2Int position, RectInt rect)
	{
		var uv = texture.GetUVRegion(rect);
		var i = (position.x + position.y * size.x) * 4;
		mesh!.vertices[i].uv = uv.position00;
		mesh!.vertices[i + 1].uv = new Vector2(uv.position11.x, uv.position00.y);
		mesh!.vertices[i + 2].uv = uv.position11;
		mesh!.vertices[i + 3].uv = new Vector2(uv.position00.x, uv.position11.y);
	}
}