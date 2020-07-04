
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
    public List<HouseSlot> HouseSlots => houseSlots;
    public List<Gift> Gifts => gifts;

    [SerializeField] private float levelTimeInSecond;
    [Header("UI - MenuStart")]
    [SerializeField] private Canvas startMenuCanvas;
    [SerializeField] private Button playButton;
    [Header("UI - Lost")]
    [SerializeField] private Canvas lostCanvas;
    [SerializeField] private Button retryButton;
    [Header("UI - Win")]
    [SerializeField] private Canvas winCanvas;
    [SerializeField] private Button nextButton;
    [Header("UI - Win")]
    //[SerializeField] private Canvas timeCanvas;
    [SerializeField] private TextMeshProUGUI timeText;

    private static GameController instance;
    private GameState gameState;
    private float remainingTime;
    private List<SantaUnit> santaUnits;
    private List<HouseSlot> houseSlots;
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
        houseSlots = GameObject.FindObjectsOfType<HouseSlot>().ToList();
        gifts = GameObject.FindObjectsOfType<Gift>().ToList();

        foreach (HouseSlot houseSlot in houseSlots)
        {
            //houseSlot.onGiftDelivered += OnGiftDelivered;
        }

        MoveCommand.Instance.onMoveSelected += OnMoveCommand;

        startMenuCanvas.gameObject.SetActive(true);
        lostCanvas.gameObject.SetActive(false);
        winCanvas.gameObject.SetActive(false);


        playButton.onClick.AddListener(OnPlayButton);
        retryButton.onClick.AddListener(OnRetryButton);
        nextButton.onClick.AddListener(OnNextButton);

        remainingTime = levelTimeInSecond;
    }

    private bool HasWin()
    {
        foreach (HouseSlot houseSlot in houseSlots)
        {
            if (houseSlot.IsDelivered() == false)
            {
                return false;
            }
        }

        return true;
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
        else if (remainingTime < Mathf.Epsilon)
        {
            remainingTime = 0;
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

    private void OnMoveCommand(List<Vector3> path)
    {
        List<Transform> selectedUnit = SelectionManager.Instance.CurrentSelection;

        foreach (Transform selectedTransform in selectedUnit)
        {
            PathFollower pathFollower = selectedTransform.GetComponent<PathFollower>();
            if (pathFollower != null)
            {
                pathFollower.SetPath(path);
            }
        }
    }

    private void OnPlayButton()
    {
        startMenuCanvas.gameObject.SetActive(false);
        gameState = GameState.Playing;
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
}
