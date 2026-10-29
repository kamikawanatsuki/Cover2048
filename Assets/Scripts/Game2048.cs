using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Game2048 : MonoBehaviour
{
    [System.Serializable]
    public class CellRow
    {
        public TileView[] cells = new TileView[4];
    }

    [SerializeField] private CellRow[] rows = new CellRow[4];

    [SerializeField] private TMP_Text scoreText;

    private int score;

    // 第一个index是row第二个是column
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
        else if (keyboard.rightArrowKey.wasPressedThisFrame ||
                 keyboard.dKey.wasPressedThisFrame)
        {
            MoveRight();
        }
        else if (keyboard.upArrowKey.wasPressedThisFrame ||
                 keyboard.wKey.wasPressedThisFrame)
        {
            MoveUp();
        }
        else if (keyboard.downArrowKey.wasPressedThisFrame ||
                 keyboard.sKey.wasPressedThisFrame)
        {
            MoveDown();
        }
    }

    public void StartNewGame()
    {
        score = 0;
        // 清空 刷新 board
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

        // 识别空着的cell并spawn一个数字
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

        // 随机选一个空着的位置
        int randomIndex = Random.Range(0, emptyCells.Count);
        Vector2Int position = emptyCells[randomIndex];

        //从十个数字里面抽一个 抽到0就spawn4
        int number = 2;

        if (Random.Range(0, 9) == 0)
            //random chance 拿出来单独放一个definition，为变量命名
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
                rows[row].cells[column].SetNumber(board[row, column]);
            }
        }

        scoreText.text = "Score: " + score;
    }

    private int[] MergeLine(int[] line)
    {
        List<int> numbers = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            if (line[i] != 0)
                //如果一个数不等于0
            {
                numbers.Add(line[i]);
                //把这个数加入列表
            }
        }

        // New arrays start with every element set to zero.
        int[] result = new int[4];
        int resultIndex = 0;

        for (int i = 0; i < numbers.Count; i++)
        {
            int number = numbers[i];

            // 如果和下一个数字相同，和列表里的下一个数字合成
            if (i + 1 < numbers.Count &&
                number == numbers[i + 1])
            {
                number = number * 2;

               
                score += number;

                //跳过下一个已合成的数字
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

            //在板子上显示合成后的board
            for (int column = 0; column < 4; column++)
            {
                if (board[row, column] != result[column])
                {
                    changed = true;
                }

                board[row, column] = result[column];
            }
        }

        
        if (changed)
        {
            SpawnNumber();
            RefreshBoard();
        }
    }

    private void MoveRight()
    {
        bool changed = false;

        for (int row = 0; row < 4; row++)
        {
            int[] line = new int[4];

            // Read one row from right to left.
            for (int i = 0; i < 4; i++)
            {
                int column = 3 - i;
                line[i] = board[row, column];
            }

            int[] result = MergeLine(line);

            // Write back
            for (int i = 0; i < 4; i++)
            {
                int column = 3 - i;

                if (board[row, column] != result[i])
                {
                    changed = true;
                }

                board[row, column] = result[i];
            }
        }

        if (changed)
        {
            SpawnNumber();
            RefreshBoard();
        }
    }

    private void MoveUp()
    {
        bool changed = false;

        for (int column = 0; column < 4; column++)
        {
            int[] line = new int[4];

            // Read one column from top to bottom.
            for (int row = 0; row < 4; row++)
            {
                line[row] = board[row, column];
            }

            int[] result = MergeLine(line);

            // Write back
            for (int row = 0; row < 4; row++)
            {
                if (board[row, column] != result[row])
                {
                    changed = true;
                }

                board[row, column] = result[row];
            }
        }

        if (changed)
        {
            SpawnNumber();
            RefreshBoard();
        }
    }

    private void MoveDown()
    {
        bool changed = false;

        for (int column = 0; column < 4; column++)
        {
            int[] line = new int[4];

            // Read one column from bottom to top.
            for (int i = 0; i < 4; i++)
            {
                int row = 3 - i;
                line[i] = board[row, column];
            }

            int[] result = MergeLine(line);

            // Write back
            for (int i = 0; i < 4; i++)
            {
                int row = 3 - i;

                if (board[row, column] != result[i])
                {
                    changed = true;
                }

                board[row, column] = result[i];
            }
        }

        if (changed)
        {
            SpawnNumber();
            RefreshBoard();
        }
    }
}