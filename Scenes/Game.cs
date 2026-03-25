using XYEngine;

namespace MinesweeperBasic.Scenes;

public class Game : Scene
{
	private Vector2Int size;
	
	public Game(Vector2Int size)
	{
		this.size = size;
	}
}