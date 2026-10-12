using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game2048 : MonoBehaviour
{
    [System.Serializable]
    public class CellRow
    {
        public TileView[] cells = new TileView[4];
    }

    [SerializeField] private CellRow[] rows = new CellRow[4];

    // The first index is the row. The second index is the column.
    private int[,] board = new int[4, 4];

    private void Start()
    {
        StartNewGame();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.leftArrowKey.wasPressedThisFrame ||
            keyboard.aKey.wasPressedThisFrame)
        {
            MoveLeft();
        }
    }

    public void StartNewGame()
    {
        // Clear the board.
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

        // Spawn 2 with a 90% chance, or 4 with a 10% chance.
        int number = 2;

        if (Random.Range(0, 10) == 0)
        {
            number = 4;
        }

        board[position.x, position.y] = number;
    }

    private void RefreshBoard()
    {
        // Display the number stored at each board position.
        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < 4; column++)
            {
                rows[row].cells[column].SetNumber(board[row, column]);
            }
        }
    }

    private int[] MergeLine(int[] line)
    {
        List<int> numbers = new List<int>();

        // Collect the numbers and skip empty cells.
        for (int i = 0; i < 4; i++)
        {
            if (line[i] != 0)
            {
                numbers.Add(line[i]);
            }
        }

        // New arrays start with every element set to zero.
        int[] result = new int[4];
        int resultIndex = 0;

        for (int i = 0; i < numbers.Count; i++)
        {
            int number = numbers[i];

            // Merge with the next number if they match.
            if (i + 1 < numbers.Count &&
                number == numbers[i + 1])
            {
                number = number * 2;

                // Skip the number that was already merged.
                i++;
            }

            result[resultIndex] = number;
            resultIndex++;
        }

        return result;
    }

    private void MoveLeft()
    {
        bool changed = false;

        for (int row = 0; row < 4; row++)
        {
            int[] line = new int[4];

            // Read one row from left to right.
            for (int column = 0; column < 4; column++)
            {
                line[column] = board[row, column];
            }

            int[] result = MergeLine(line);

            // Write the merged row back to the board.
            for (int column = 0; column < 4; column++)
            {
                if (board[row, column] != result[column])
                {
                    changed = true;
                }

                board[row, column] = result[column];
            }
        }

        // Spawn a new number only when the board changed.
        if (changed)
        {
            SpawnNumber();
            RefreshBoard();
        }
    }
}