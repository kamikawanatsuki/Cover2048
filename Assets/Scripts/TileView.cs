using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text numberText;

    // Set a number in the Inspector to preview this cell.
    [SerializeField] private int number;

    [System.Serializable]
    public class TileColor
    {
        public int number;
        public Color color;
    }

    [SerializeField] private Color emptyColor;
    [SerializeField] private TileColor[] tileColors;

    // Update the number, text, and colors together.
    public void SetNumber(int newNumber)
    {
        number = newNumber;

        if (number == 0)
        {
            numberText.text = "";
        }
        else
        {
            numberText.text = number.ToString();
        }

        background.color = GetBackgroundColor(number);

        if (number <= 4)
        {
            numberText.color = new Color32(119, 110, 101, 255);
        }
        else
        {
            numberText.color = new Color32(249, 246, 242, 255);
        }
    }

    private Color GetBackgroundColor(int value)
    {
        foreach (TileColor tileColor in tileColors)
        {
            if (tileColor.number == value)
            {
                return tileColor.color;
            }
        }

        return emptyColor;
    }
}