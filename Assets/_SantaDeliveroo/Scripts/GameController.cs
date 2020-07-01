
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance => instance;
    public List<SantaUnit> SantaUnits => santaUnits;

    private static GameController instance;
    private List<SantaUnit> santaUnits;

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
    }
}
