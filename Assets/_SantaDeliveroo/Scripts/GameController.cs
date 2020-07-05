
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameController Instance => instance;
    public List<SantaUnit> SantaUnits => santaUnits;
    public List<Befana> Befanas => befanas;
    public List<House> HouseSlots => houseSlots;
    public List<Gift> Gifts => gifts;
    public Transform Honolulu => honolulu;
    public int DeliveredGiftToWin => deliveredGiftsToWin;
    public SelectionManager SelectionManager => SelectionManager.Instance;
    public MoveCommand MoveCommand => MoveCommand.Instance;

    [SerializeField] private Transform honolulu;
    [SerializeField] private float levelTimeInSecond;
    [SerializeField] private int deliveredGiftsToWin;
    [Header("UI - MenuStart")]
    [SerializeField] private Canvas startMenuCanvas;
    [SerializeField] private Button playButton;
    [Header("UI - Lost")]
    [SerializeField] private Canvas lostCanvas;
    [SerializeField] private Button retryButton;
    [Header("UI - Win")]
    [SerializeField] private Canvas winCanvas;
    [SerializeField] private Button nextButton;
    [Header("UI - Game")]
    //[SerializeField] private Canvas timeCanvas;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameInfoUI gameInfoUI;
    [SerializeField] private GiftSelectionUI giftSelectionUI;
    [SerializeField] private CommandUI commandUI;

    private static GameController instance;
    private GameState gameState;
    private float remainingTime;
    private List<SantaUnit> santaUnits;
    private List<Befana> befanas;
    private List<House> houseSlots;
    private List<Gift> gifts;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        santaUnits = GameObject.FindObjectsOfType<SantaUnit>().ToList();
        befanas = GameObject.FindObjectsOfType<Befana>().ToList();
        houseSlots = GameObject.FindObjectsOfType<House>().ToList();
        gifts = GameObject.FindObjectsOfType<Gift>().ToList();

        gameInfoUI.UpdateGiftLeft(deliveredGiftsToWin);
        gameInfoUI.UpdateSantaUnitLeft(santaUnits.Count);
        giftSelectionUI.Hide();
        commandUI.Hide();

        startMenuCanvas.gameObject.SetActive(true);
        lostCanvas.gameObject.SetActive(false);
        winCanvas.gameObject.SetActive(false);


        playButton.onClick.AddListener(OnPlayButton);
        retryButton.onClick.AddListener(OnRetryButton);
        nextButton.onClick.AddListener(OnNextButton);

        remainingTime = levelTimeInSecond;
    }

    private void EnableGameEvent()
    {
        foreach (SantaUnit santaUnit in santaUnits)
        {
            santaUnit.onGiftsCollected += OnGiftCollected;
            santaUnit.onGiftsDelivered += OnGifstDelivered;
            santaUnit.onKidnapped += OnKidnapped;
        }

        MoveCommand.Instance.onMoveSelected += OnMoveCommand;
        SelectionManager.Instance.onSelectionChange += OnSelectionChange;
    }

    private bool HasLost()
    {
        if (remainingTime < Mathf.Epsilon)
        {
            remainingTime = 0;
            return true;
        }

        foreach (SantaUnit santaUnit in santaUnits)
        {
            if(santaUnit.IsKidnapped() == false)
            {
                return false;
            }
        }

        return true;
    }

    private bool HasWin()
    {
        if (deliveredGiftsToWin == 0)
        {
            return true;
        }

        return false;
    }

    private void Update()
    {
        if (gameState == GameState.Playing)
        {
            PlayingUpdate();
        }
    }

    private void PlayingUpdate()
    {
        remainingTime -= Time.deltaTime;

        if (HasWin())
        {
            Win();
        }
        else if (HasLost())
        {
            Lost();
        }
        UpdateTimeText();
    }

    private void Lost()
    {
        lostCanvas.gameObject.SetActive(true);
    }

    private void Win()
    {
        winCanvas.gameObject.SetActive(true);
    }

    public void UpdateTimeText()
    {
        int minutes = (int) remainingTime / 60;
        int seconds = (int) remainingTime - minutes * 60;
        timeText.text = String.Format("{0}:{1}", minutes, seconds);
    }

    #region EVENTS
    private void OnPlayButton()
    {
        startMenuCanvas.gameObject.SetActive(false);
        gameState = GameState.Playing;

        foreach (Befana befana in befanas)
        {
            befana.StartPatrolling();
        }

        EnableGameEvent();
    }

    private void OnRetryButton()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex, LoadSceneMode.Single);
    }

    private void OnNextButton()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentScene.buildIndex + 1, LoadSceneMode.Single);
        }
    }

    private void OnGiftCollected(SantaUnit santaUnit, Gift gift)
    {
        ISelectable selectable = santaUnit.GetComponent<ISelectable>();
        giftSelectionUI.SetSelectable(selectable);
    }

    private void OnGifstDelivered(List<Gift> deliveredGifts)
    {
        deliveredGiftsToWin -= deliveredGifts.Count;

        deliveredGiftsToWin = Mathf.Max(0, deliveredGiftsToWin);

        gameInfoUI.UpdateGiftLeft(deliveredGiftsToWin);
    }

    private void OnKidnapped(SantaUnit santaUnit)
    {
        CameraController.Instance.OrbitCamera.SetTarget(null);

        int santaLeft = 0;
        foreach (SantaUnit item in santaUnits)
        {
            if (item.IsKidnapped() == false)
            {
                santaLeft++;
            }
        }
        gameInfoUI.UpdateSantaUnitLeft(santaLeft);
    }

    private void OnSelectionChange()
    {
        List<Transform> currentSelection = SelectionManager.CurrentSelection;

        if (currentSelection.Count > 0)
        {
            Transform targetItem = SelectionManager.GetTargetItem();
            if (targetItem == null)
            {
                return;
            }

            if (SelectionManager.GetSelectionType() == SelectableType.Santa)
            {
                commandUI.Show();
            }

            CameraController.Instance.OrbitCamera.SetTarget(targetItem);

            if (currentSelection.Count == 1)
            {
                ISelectable selectable = currentSelection[0].GetComponent<ISelectable>();
                giftSelectionUI.SetSelectable(selectable);
                giftSelectionUI.Show();
            }
        }
        else
        {
            commandUI.Hide();
            giftSelectionUI.Hide();
        }
    }


    private void OnMoveCommand(List<Vector3> path)
    {
        InputManager.Instance.SetCurrentHandler(SelectionManager.Instance);

        List<Transform> selectedUnit = SelectionManager.Instance.CurrentSelection;

        foreach (Transform selectedTransform in selectedUnit)
        {
            PathFollower pathFollower = selectedTransform.GetComponent<PathFollower>();
            if (pathFollower != null)
            {
                pathFollower.SetPath(path);
            }
        }

        Transform targetItem = SelectionManager.Instance.GetTargetItem();
        
        System.Action onSelectionChange = () =>
        {
            CameraController.Instance.OrbitCamera.SetTarget(targetItem);
        };

        SelectionManager.Instance.onSelectionChange += onSelectionChange;
        SelectionManager.Instance.DeselectAll();
        SelectionManager.Instance.onSelectionChange -= onSelectionChange;
    }


    #endregion
}
