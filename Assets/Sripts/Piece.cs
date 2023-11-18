using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;


    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        stepTime = Time.time + stepDelay;
        rotationIndex = 0;
        lockTime = 0f;

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
        lockTime += Time.deltaTime;
        board.Clear(this);
        //if (Input.GetKeyDown(KeyCode.Q)) Rotation(-1);
        //else if (Input.GetKeyDown(KeyCode.E)) Rotation(1);

        //if (Input.GetKeyDown(KeyCode.A)) Move(Vector2Int.left);
        //else if (Input.GetKeyDown(KeyCode.D)) Move(Vector2Int.right);

        //if (Input.GetKeyDown(KeyCode.S)) Move(Vector2Int.down);

        if (Input.GetKeyDown(KeyCode.Space)) HardDrop();

        if (Time.time >= stepTime) Step();

        board.Set(this);
    }
    public void AndroidMove(Vector2Int Dir) 
    {
        board.Clear(this);
        Move(Dir);
        board.Set(this);
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if (lockTime >= lockDelay) Lock();
    }

    private void Lock()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            if (tilePosition.y == 9)
            {
                board.TheEnd = true;
                board.End.enabled = true;
            }
        }
        board.Set(this);
        board.ClearLines();
        if (!board.TheEnd)
        board.SpawnPiece();
    }
    public void HardDrop()
    {
        while (Move(Vector2Int.down)) continue;
        Lock();
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
            lockTime = 0f;
        }
        return valid;
    }
    public void Rotation(int direction)
    {
        int orginalRotation = rotationIndex;
        rotationIndex += Wrap(rotationIndex + direction, 0, 4);

        DoRotationMatrix(direction);

        if(!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = orginalRotation;
            DoRotationMatrix(-direction);
        }
    }
    private void DoRotationMatrix(int direction)
    {
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

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKicksIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++) 
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }
        return false;
    }
    private int GetWallKicksIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;
        if(rotationDirection < 0) 
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
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
