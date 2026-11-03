using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game2048Animation : MonoBehaviour
{
    [SerializeField] private float slideTime = 0.12f;
    [SerializeField] private float popTime = 0.12f;

    // One record remembers where an old number needs to slide.
    private class TileMove
    {
        public TileView from;
        public TileView to;
        public int number;
    }

    private List<TileMove> moves = new List<TileMove>();
    private List<TileView> movingTiles = new List<TileView>();
    private List<TileView> mergedTiles = new List<TileView>();

    public void PrepareMove()
    {
        moves.Clear();
        mergedTiles.Clear();
    }

    public void RecordMerge(TileView tile)
    {
        mergedTiles.Add(tile);
    }

    public void CancelAnimation(Game2048.CellRow[] rows)
    {
        ClearMovingTiles();
        ResetTileSizes(rows);
        PrepareMove();
    }

    public void RecordMove(TileView from, TileView to, int number)
    {
        TileMove move = new TileMove();
        move.from = from;
        move.to = to;
        move.number = number;
        moves.Add(move);
    }

    public IEnumerator Slide(Game2048.CellRow[] rows)
    {
        Canvas.ForceUpdateCanvases();

        // Copy the visible tiles. The copies can move freely above the grid.
        for (int i = 0; i < moves.Count; i++)
        {
            TileView copy = Instantiate(moves[i].from, transform);
            copy.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;

            RectTransform copyRect = (RectTransform)copy.transform;
            RectTransform sourceRect = (RectTransform)moves[i].from.transform;
            copyRect.anchorMin = new Vector2(0.5f, 0.5f);
            copyRect.anchorMax = new Vector2(0.5f, 0.5f);
            copyRect.sizeDelta = sourceRect.rect.size;
            copyRect.position = sourceRect.position;
            copy.SetNumber(moves[i].number);

            // These copies are only pictures, not buttons.
            foreach (Graphic graphic in copy.GetComponentsInChildren<Graphic>())
            {
                graphic.raycastTarget = false;
            }
            movingTiles.Add(copy);
        }

        // Leave the sixteen original cells in place as empty backgrounds.
        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < 4; column++)
            {
                rows[row].cells[column].SetNumber(0);
            }
        }

        float elapsed = 0;
        while (elapsed < slideTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / slideTime);
            for (int i = 0; i < movingTiles.Count; i++)
            {
                movingTiles[i].transform.position = Vector3.Lerp(
                    moves[i].from.transform.position,
                    moves[i].to.transform.position, progress);
            }
            yield return null; // Continue here on the next frame.
        }

        ClearMovingTiles();

    }

    public IEnumerator Pop(Game2048.CellRow[] rows, TileView spawnedTile)
    {
        // Merged tiles grow a little, then return to their normal size.
        // The new random tile grows from small to full size.
        float elapsed = 0;
        if (popTime > 0 && spawnedTile != null)
        {
            spawnedTile.transform.localScale = Vector3.zero;
        }
        while (elapsed < popTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / popTime);
            float scale;
            if (progress < 0.5f)
            {
                scale = Mathf.Lerp(1f, 1.18f, progress * 2);
            }
            else
            {
                scale = Mathf.Lerp(1.18f, 1f, (progress - 0.5f) * 2);
            }

            for (int i = 0; i < mergedTiles.Count; i++)
            {
                mergedTiles[i].transform.localScale = Vector3.one * scale;
            }
            if (spawnedTile != null)
            {
                spawnedTile.transform.localScale = Vector3.one * progress;
            }
            yield return null;
        }

        ResetTileSizes(rows);
        moves.Clear();
        mergedTiles.Clear();
    }

    private void ClearMovingTiles()
    {
        for (int i = 0; i < movingTiles.Count; i++)
        {
            // Destroy happens at the end of the frame, so hide the copy now.
            movingTiles[i].gameObject.SetActive(false);
            Destroy(movingTiles[i].gameObject);
        }
        movingTiles.Clear();
    }

    private void ResetTileSizes(Game2048.CellRow[] rows)
    {
        for (int row = 0; row < 4; row++)
        {
            for (int column = 0; column < 4; column++)
            {
                rows[row].cells[column].transform.localScale = Vector3.one;
            }
        }
    }

}
