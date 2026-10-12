using System.Collections.Generic;
using UnityEngine;

public class Game2048 : MonoBehaviour
{
    [SerializeField] private Transform boardRoot;

    // The first index is the row. The second index is the column.
    private int[,] board = new int[4, 4];
    private TileView[,] cells = new TileView[4, 4];

    private void Start()
    {
        FindCells();
        StartNewGame();
    }

    private void FindCells()
    {
        // Grid Layout Group displays children from left to right,
        // then continues on the next row.
        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < 4; column++)
            {
                int childIndex = row * 4 + column;
                Transform cell = boardRoot.GetChild(childIndex);

                cells[row, column] = cell.GetComponent<TileView>();
            }
        }
    }

    public void StartNewGame()
    {
        // Clear all numbers before starting a new game.
        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < 4; column++)
            {
                board[row, column] = 0;
            }
        }

        SpawnNumber();
        SpawnNumber();
        RefreshBoard();
    }

    private void SpawnNumber()
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();

        // Collect every empty position.
        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < 4; column++)
            {
                if (board[row, column] == 0)
                {
                    emptyCells.Add(new Vector2Int(row, column));
                }
            }
        }

        if (emptyCells.Count == 0)
        {
            return;
        }

        // Pick one empty position at random.
        int randomIndex = Random.Range(0, emptyCells.Count);
        Vector2Int position = emptyCells[randomIndex];

        // A new number has a 90% chance of being 2.
        int number = 2;

        if (Random.Range(0, 10) == 0)
        {
            number = 4;
        }

        board[position.x, position.y] = number;
    }

    private void RefreshBoard()
    {
        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < 4; column++)
            {
                cells[row, column].SetNumber(board[row, column]);
            }
        }
    }
}