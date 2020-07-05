using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameInfoUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI giftLeftText;
    [SerializeField] TextMeshProUGUI santaUnitLeftText;

    public void UpdateGiftLeft(int value)
    {
        giftLeftText.text = value.ToString();
    }

    public void UpdateSantaUnitLeft(int value)
    {
        santaUnitLeftText.text = value.ToString();
    }
}
