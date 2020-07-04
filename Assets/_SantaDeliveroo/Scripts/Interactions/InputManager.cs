using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    static public InputManager Instance => instance;

    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private OrbitCamera orbitCamera;
    [SerializeField] private MoveCommand moveCommand;

    static private InputManager instance;
    private IMouseHandler activeMouseHandler;
    private IMouseHandler lastMouseHandler;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        activeMouseHandler = selectionManager;
        lastMouseHandler = selectionManager;
    }

    private void Start()
    {
        SelectionManager.Instance.onSelectionChange += () =>
        {
            if (SelectionManager.Instance.CurrentSelection.Count > 0)
            {
                InputManager.Instance.SetCurrentHandler(MoveCommand.Instance);
            }
        };

        MoveCommand.Instance.onMoveSelected += (list) =>
        {
            SelectionManager.Instance.DeselectAll();
            InputManager.Instance.SetCurrentHandler(SelectionManager.Instance);
        };
    }

    void Update()
    {
        

        if (activeMouseHandler == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                SetCurrentHandler(orbitCamera);
            }
            else
            {
                if (activeMouseHandler == orbitCamera)
                {
                    activeMouseHandler = lastMouseHandler;
                }
            }

            activeMouseHandler.OnMouseLeftClickDown();
        }
        if (Input.GetMouseButton(0))
        {
            activeMouseHandler.OnMouseLeftClick();
        }
        if (Input.GetMouseButtonUp(0))
        {
            activeMouseHandler.OnMouseLeftClickUp();
        }
    }

    public void SetCurrentHandler(IMouseHandler handler)
    {
        if (activeMouseHandler != orbitCamera)
        {
            lastMouseHandler = activeMouseHandler;
        }
        activeMouseHandler = handler;
    }

    public void SetMoveCommand()
    {
        SetCurrentHandler(moveCommand);
    }
}
