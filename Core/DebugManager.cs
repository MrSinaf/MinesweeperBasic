#if DEBUG
using Ratelite;

namespace MinesweeperBasic;

public class DebugManager : IModule
{
	public int priority => 0;
	
	public void Init()
	{
		RDebug.showMenuBar = false;
		R.game.window.keyPressed += OnMouseButtonPressed;
		R.game.window.keyReleased += OnMouseButtonReleased;
	}
	
	private static void OnMouseButtonPressed(Key key, int _)
	{
		if (key == Key.LeftAlt)
			RDebug.showMenuBar = true;
	}
	
	private static void OnMouseButtonReleased(Key key, int _)
	{
		if (key == Key.LeftAlt)
			RDebug.showMenuBar = false;
	}
}
#endif