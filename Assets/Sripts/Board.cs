using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominos;
    public Vector3Int SpawnPositon;
    public Vector2Int boardSize = new Vector2Int(20, 20);
    public TextMeshProUGUI textMeshPro;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }
    private void Start()
    {
        SpawnPiece();
    }
    public void SpawnPiece()
    {
        int random = UnityEngine.Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];

        activePiece.Initialize(this, SpawnPositon, data);
        Set(activePiece);
    }
    public void AndroidMove(int dir)
    {
        if (dir == -1)
        {
            activePiece.AndroidMove(Vector2Int.left);
        }
        else if (dir == 1)
        {
            activePiece.AndroidMove(Vector2Int.right);
        }
        else if (dir == 2)
        {
            activePiece.AndroidMove(Vector2Int.down);
        }
        else if (dir == 3)
        {
            Clear(activePiece);
            activePiece.HardDrop();
            Set(activePiece);
        }
        else if (dir == 4)
        {
            Clear(activePiece);
            activePiece.Rotation(1);
            Set(activePiece);
        }
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i<piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length;i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;
            if (tilemap.HasTile(tilePosition)) return false;
            if (!bounds.Contains((Vector2Int)tilePosition)) return false;
        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        while (row < bounds.yMax)
        {
            if (IslineFull(row)) LineClear(row);
            else row++;
        }
    }
    public bool IslineFull(int row)
    {
        RectInt bounds = Bounds;
        for(int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position)) return false;
        }
        return true;
    }
    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }
        textMeshPro.text = (Convert.ToInt32(textMeshPro.text) + 100).ToString();

        while ( row < bounds.yMax )
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);

            }
            
            row++;
        }
    }
}
