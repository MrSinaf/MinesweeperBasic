namespace MinesweeperBasic;

public enum TileType { Hidden, Marque, Question, Visible}

public class Tile(int quadId)
{
    public readonly int quadId = quadId;
    
    public bool isBomb;
    public TileType type = TileType.Hidden;
}