using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public int rotationIndex { get; private set; }


    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        rotationIndex = 0;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++) 
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }
    private void Update()
    {
        board.Clear(this);
        if (Input.GetKeyDown(KeyCode.Q)) Rotation(-1);
        else if (Input.GetKeyDown(KeyCode.E)) Rotation(1);
        if (Input.GetKeyDown(KeyCode.A)) Move(Vector2Int.left);
        else if (Input.GetKeyDown(KeyCode.D)) Move(Vector2Int.right);
        if (Input.GetKeyDown(KeyCode.S)) Move(Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.Space)) HardDrop();
        board.Set(this);
    }
    private void HardDrop()
    {
        while (Move(Vector2Int.down)) continue;
    }
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPositon = position;
        newPositon.x += translation.x;
        newPositon.y += translation.y;

        bool valid = board.IsValidPosition(this, newPositon);
        
        if (valid)
        {
            position = newPositon;
        }
        return valid;
    }
    private void Rotation(int direction)
    {
        rotationIndex += Wrap(rotationIndex + direction, 0, 4);
        
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                     x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                     y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }
            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min) 
        {
            return max - (min - input) % (max - min);
        }
        else 
        {
            return min + (input - min) % (max - min);
        }
    }
}
