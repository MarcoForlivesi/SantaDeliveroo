using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftSelectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private Image[] giftImages;

    private ISelectable selectable;

    private void Start()
    {
        foreach(Image giftImage in giftImages)
        {
            giftImage.gameObject.SetActive(false);
        }
    }

    public void SetSelectable(ISelectable selectable)
    {
        this.selectable = selectable;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (selectable == null)
        {
            return;
        }

        type.text = selectable.GetSelectableType().ToString();

        foreach (Image giftImage in giftImages)
        {
            bool contains = Contains(giftImage);
            giftImage.gameObject.SetActive(contains);

        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private bool Contains(Image giftImage)
    {
        List<GiftType> giftTypes = selectable.GetGiftTypes();
        foreach (GiftType giftType in giftTypes)
        {
            if (giftType.ToString() == giftImage.name)
            {
                return true;
            }
        }
        return false;
    }
}
