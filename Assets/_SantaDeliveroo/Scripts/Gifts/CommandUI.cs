using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandUI : MonoBehaviour
{
    public event System.Action onMoveButton;

    [SerializeField] private Button moveButton;

    private void Start()
    {
        moveButton.onClick.AddListener(OnMoveButton);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            OnMoveButton();
        }
    }

    private void OnMoveButton()
    {
        Transform targetItem = SelectionManager.Instance.GetTargetItem();
        if (targetItem == null)
        {
            return;
        }

        InputManager.Instance.SetCurrentHandler(MoveCommand.Instance);
        MoveCommand.Instance.SetStartPosition(targetItem.position);

        if (onMoveButton != null)
        {
            onMoveButton.Invoke();
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
}
