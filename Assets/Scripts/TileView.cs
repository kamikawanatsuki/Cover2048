using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    [System.Serializable]
    public class TileColor
    {
        public int number;
        public Color color;
        public Color textColor;
    }

    [SerializeField] private Image background;
    [SerializeField] private TMP_Text numberText;

    [SerializeField] private Color emptyColor;
    [SerializeField] private TileColor[] tileColors;

    public void SetNumber(int number)
    {
        background.color = emptyColor;
        numberText.text = "";

        if (number == 0)
        {
            return;
        }

        numberText.text = number.ToString();
        numberText.color = Color.white;

        foreach (TileColor tileColor in tileColors)
        {
            if (tileColor.number == number)
            {
                background.color = tileColor.color;
                numberText.color = tileColor.textColor;
                return;
            }
        }
    }
}