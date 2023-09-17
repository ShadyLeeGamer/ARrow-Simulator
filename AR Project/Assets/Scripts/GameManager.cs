using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public const int MAX_NUM_PLAYERS = 4;
    public const int MIN_NUM_PLAYERS = 2;

    public Player[] Players { get; private set; }
    public int CurrentPlayerIndex { get; private set; }
    public Player CurrentPlayer => Players[CurrentPlayerIndex];

    public enum TurnTypes { Place, Battle }
    public TurnTypes TurnType { get; private set; }

    ArcherPlacer archerPlacer;
    ItemSpawner itemSpawner;

    [SerializeField] GameCanvas canvas;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        archerPlacer = ArcherPlacer.Instance;
        itemSpawner = ItemSpawner.Instance;
    }

    public void StartGame(InputField[] playerInputFields, int numPlayers)
    {
        Players = new Player[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            Players[i] = new Player(playerInputFields[i].text);
        }

        StartPlaceTurns();
        canvas.gameObject.SetActive(true);
    }

    bool NextTurn()
    {
        if (CurrentPlayerIndex < Players.Length - 1)
        {
            CurrentPlayerIndex++;
            return true;
        }
        else
        {
            CurrentPlayerIndex = 0; // Wrap
            return false;
        }
    }

    void StartPlaceTurns()
    {
        TurnType = TurnTypes.Place;
        archerPlacer.enabled = true;
    }

    public void EndPlaceTurn(Archer archer)
    {
        CurrentPlayer.SetArcher(archer);

        if (!NextTurn())
        {
            StartBattleTurns();
        }
    }

    void StartBattleTurns()
    {
        itemSpawner.Initialise(archerPlacer.Planes);
        archerPlacer.enabled = false;
        TurnType = TurnTypes.Battle;

        CurrentPlayer.archer.SetActive(true);

    }

    public void EndBattleTurn()
    {
        if (!NextTurn())
        {
            itemSpawner.Spawn();
        }

        //canvas.SetPowerSlider(CurrentPlayer.archer.LastYRotation);
        CurrentPlayer.archer.SetActive(true);
    }
}

public class Player
{
    public string Name { get; set; }
    public Archer archer;

    public Player(string name)
    {
        Name = name;
    }

    public void SetArcher(Archer archer)
    {
        this.archer = archer;
    }
}