using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    void Select(bool value);

    SelectableType GetSelectableType();
    List<GiftType> GetGiftTypes();
}
