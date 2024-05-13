using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    public int baseValue;
    public Text countText;

    public void Show(int baseValue)
    {
        if (baseValue == -1)
        {
            countText.text = "";
            return;
        }

        this.baseValue = baseValue;
        countText.text = "+" + baseValue.ToString();
    }
}
