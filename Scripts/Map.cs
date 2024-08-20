using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SPEngine;
using SPEngine.Essentials;
using SPEngine.UI;

namespace MinesweeperBasic;

public class Map : ObjectBehaviour
{
    private readonly AtlasData data;
    private readonly int nTileVisibleTarget;
    private readonly Grid<Tile> tiles;
    private readonly Game game;
    private readonly int defaultNLife;
    private readonly int defaultNBomb;

    private bool pause;
    private float timer;
    private int nTileVisible;
    private int nLife;
    private int nBomb;

    public Map(AtlasData data, int width, int height, int nBomb, int nLife)
    {
        this.data = data;
        game = SceneManager.main as Game;
        renderer2D = new MeshRenderer2D(width * height) { texture = data.texture };
        nTileVisibleTarget = width * height - nBomb;
        position = -(new Vector2(width, height) * .5F);
        defaultNBomb = nBomb;
        defaultNLife = nLife;

        var iQuad = -1;
        tiles = new Grid<Tile>(width, height, (x, y) =>
        {
            iQuad++;
            renderer2D.SetQuad(iQuad, new Vector2(x, y), Vector2.One, data.atlasRegions[0].uv00, data.atlasRegions[0].uv11);

            return new Tile(iQuad);
        });

        Init();
    }

    public void Recreate()
    {
        game.popup.active = false;
        
        timer = 0;
        nTileVisible = 0;
        
        foreach (var tile in tiles)
        {
            tile.type = TileType.Hidden;
            tile.isBomb = false;
            renderer2D.SetQuadUV(tile.quadId, data.atlasRegions[0].uv00, data.atlasRegions[0].uv11);
        }

        Init();
    }
    
    protected override void Update()
    {
        if (pause)
            return;
        
        timer += Time.deltaTime;
        game.time.text = $"{(int)timer / 60:00}:{(int)timer % 60:00}";

        if (Input.mouseLeftDown)
        {
            var mousePosition = (Input.mouseWorldPosition - position).ToPoint();
            if (!tiles.CheckOutOfRangeArray(mousePosition.X, mousePosition.Y))
                DiscoverTile(mousePosition);
        }
        else if (Input.mouseRightDown)
        {
            var mousePosition = (Input.mouseWorldPosition - position).ToPoint();
            if (tiles.CheckOutOfRangeArray(mousePosition.X, mousePosition.Y))
                return;

            var tile = tiles[mousePosition.X, mousePosition.Y];

            var typeId = (int)tile.type;
            if (typeId < 3)
            {
                typeId = (typeId + 1) % 3;
                tile.type = (TileType)typeId;
                renderer2D.SetQuadUV(tile.quadId, data.atlasRegions[typeId].uv00, data.atlasRegions[typeId].uv11);
                
                // ajuste le nombre de bombes :
                if (tile.type == TileType.Marque)
                    nBomb--;
                else if (tile.type == TileType.Question)
                    nBomb++;
                game.bombs.text = nBomb.ToString();
            }
        }
    }

    private void Init()
    {
        nBomb = defaultNBomb;
        nLife = defaultNLife;
        
        // Création des bombes :
        var nTiles = tiles.width * tiles.height;
        for (var i = 0; i < nBomb; i++)
        {
            var position = tiles.IndexToPoint(Random.Shared.Next(nTiles));
            var tile = tiles[position.X, position.Y];

            if (tile.isBomb)
                i--;
            else
                tile.isBomb = true;
        }

        game.bombs.text = nBomb.ToString();
        game.life.text = nLife.ToString();
        pause = false;
    }

    private void DiscoverTile(Point position)
    {
        var currentTile = tiles[position.X, position.Y];
        if (currentTile.type == TileType.Visible)
            return;
        if (currentTile.type == TileType.Marque)
        {
            this.nBomb++;
            game.bombs.text = this.nBomb.ToString();
        }
            
        currentTile.type = TileType.Visible;

        if (currentTile.isBomb)
        {
            game.bombs.text = (--this.nBomb).ToString();
            game.life.text = (--nLife).ToString();
            renderer2D.SetQuadUV(currentTile.quadId, data.atlasRegions[12].uv00, data.atlasRegions[12].uv11);
            
            if (nLife <= 0)
                OnLoose();
            return;
        }
        
        nTileVisible++;
        if (nTileVisible == nTileVisibleTarget)
            OnWin();

        var nBomb = 0;
        var list = new List<Point>();
        for (var y = -1; y < 2; y++)
        for (var x = -1; x < 2; x++)
        {
            var cPosition = new Point(x + position.X, y + position.Y);
            if ((x == 0 && y == 0) || !tiles.TryGetObject(cPosition.X, cPosition.Y, out var tile) || tile.type == TileType.Visible && !tile.isBomb)
                continue;

            if (tile.isBomb)
                nBomb++;
            else
                list.Add(cPosition);
        }

        renderer2D.SetQuadUV(currentTile.quadId, data.atlasRegions[3 + nBomb].uv00, data.atlasRegions[3 + nBomb].uv11);
        if (nBomb == 0)
        {
            foreach (var cPosition in list)
                DiscoverTile(cPosition);
        }
    }

    private void OnLoose()
    {
        ((Label)game.popup.GetChild(0)).text = $"Vous avez perdu, {nTileVisible}/{nTileVisibleTarget} cases trouvées !";
        game.popup.active = true;
        
        Console.WriteLine("Game Loose !");
        pause = true;

        foreach (var tile in tiles) 
            if (tile.isBomb && tile.type != TileType.Visible)
                renderer2D.SetQuadUV(tile.quadId, data.atlasRegions[12].uv00, data.atlasRegions[12].uv11);
    }

    private void OnWin()
    {
        ((Label)game.popup.GetChild(0)).text = "Vous avez gagné, bravo !";
        game.popup.active = true;
        
        Console.WriteLine("Game Win !");
        pause = true;
    }
}