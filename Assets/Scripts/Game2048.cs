using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Game2048Animation))]
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

    private Game2048Animation tileAnimation;
    private bool isAnimating;
    private TileView spawnedTile;

    private void Awake()
    {
        tileAnimation = GetComponent<Game2048Animation>();
        // Also support an older scene that has not added the component yet.
        if (tileAnimation == null)
        {
            tileAnimation = gameObject.AddComponent<Game2048Animation>();
        }
    }

    // 第一个index是row第二个是column
    private int[,] board = new int[4, 4];

    private void Start()
    {
        StartNewGame();
    }

    private void Update()
    {
        // Wait until the previous move has finished.
        if (isAnimating)
        {
            return;
        }

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
        StopAllCoroutines();
        tileAnimation.CancelAnimation(rows);
        isAnimating = false;
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
        spawnedTile = null;
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

        if (Random.Range(0, 10) == 0)
            //random chance 拿出来单独放一个definition，为变量命名
        {
            number = 4;
        }

        board[position.x, position.y] = number;
        spawnedTile = rows[position.x].cells[position.y];
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

    private int[] MergeLine(int[] line, TileView[] lineViews)
    {
        List<int> numbers = new List<int>();
        // Keep the original positions even after removing the zeros.
        List<int> positions = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            if (line[i] != 0)
                //如果一个数不等于0
            {
                numbers.Add(line[i]);
                positions.Add(i);
                //把这个数加入列表
            }
        }

        // New arrays start with every element set to zero.
        int[] result = new int[4];
        int resultIndex = 0;

        for (int i = 0; i < numbers.Count; i++)
        {
            int number = numbers[i];
            tileAnimation.RecordMove(lineViews[positions[i]], lineViews[resultIndex], number);

            // 如果和下一个数字相同，和列表里的下一个数字合成
            if (i + 1 < numbers.Count &&
                number == numbers[i + 1])
            {
                tileAnimation.RecordMove(lineViews[positions[i + 1]], lineViews[resultIndex], numbers[i + 1]);
                number = number * 2;
                tileAnimation.RecordMerge(lineViews[resultIndex], number);
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
        tileAnimation.PrepareMove();
        bool changed = false;

        for (int row = 0; row < 4; row++)
        {
            int[] line = new int[4];
            TileView[] lineViews = new TileView[4];

            // Read one row from left to right.
            for (int column = 0; column < 4; column++)
            {
                line[column] = board[row, column];
                lineViews[column] = rows[row].cells[column];
            }

            int[] result = MergeLine(line, lineViews);

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
            StartCoroutine(AnimateMove());
        }
    }

    private void MoveRight()
    {
        tileAnimation.PrepareMove();
        bool changed = false;

        for (int row = 0; row < 4; row++)
        {
            int[] line = new int[4];
            TileView[] lineViews = new TileView[4];

            // Read one row from right to left.
            for (int i = 0; i < 4; i++)
            {
                int column = 3 - i;
                line[i] = board[row, column];
                lineViews[i] = rows[row].cells[column];
            }

            int[] result = MergeLine(line, lineViews);

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
            StartCoroutine(AnimateMove());
        }
    }

    private void MoveUp()
    {
        tileAnimation.PrepareMove();
        bool changed = false;

        for (int column = 0; column < 4; column++)
        {
            int[] line = new int[4];
            TileView[] lineViews = new TileView[4];

            // Read one column from top to bottom.
            for (int row = 0; row < 4; row++)
            {
                line[row] = board[row, column];
                lineViews[row] = rows[row].cells[column];
            }

            int[] result = MergeLine(line, lineViews);

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
            StartCoroutine(AnimateMove());
        }
    }

    private void MoveDown()
    {
        tileAnimation.PrepareMove();
        bool changed = false;

        for (int column = 0; column < 4; column++)
        {
            int[] line = new int[4];
            TileView[] lineViews = new TileView[4];

            // Read one column from bottom to top.
            for (int i = 0; i < 4; i++)
            {
                int row = 3 - i;
                line[i] = board[row, column];
                lineViews[i] = rows[row].cells[column];
            }

            int[] result = MergeLine(line, lineViews);

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
            StartCoroutine(AnimateMove());
        }
    }

    // The game decides the order; Game2048Animation draws the movement.
    private IEnumerator AnimateMove()
    {
        isAnimating = true;
        yield return tileAnimation.Slide(rows);
        SpawnNumber();
        RefreshBoard();
        yield return tileAnimation.Pop(rows, spawnedTile);
        isAnimating = false;
    }

    private void OnDisable()
    {
        if (isAnimating)
        {
            StopAllCoroutines();
            tileAnimation.CancelAnimation(rows);
            RefreshBoard();
            isAnimating = false;
        }
    }
}
