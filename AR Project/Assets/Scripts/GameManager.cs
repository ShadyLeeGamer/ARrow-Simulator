using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player[] Players { get; private set; }
    public int CurrentPlayerIndex { get; private set; }
    public Player CurrentPlayer => Players[CurrentPlayerIndex];

    public enum TurnTypes { Place, Battle }
    public TurnTypes TurnType { get; private set; }

    ArcherPlacer archerPlacer;
    ItemSpawner itemSpawner;

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

    public void StartGame(InputField[] playerInputFields)
    {
        Players = new Player[playerInputFields.Length];
        for (int i = 0; i < playerInputFields.Length; i++)
        {
            Players[i] = new Player(playerInputFields[i].text);
        }

        StartPlaceTurns();
    }

    bool NextTurn()
    {
        if (CurrentPlayerIndex < Players.Length - 1)
        {
            CurrentPlayerIndex++;
            return true;
        }
        else
            return false;
    }

    void StartPlaceTurns()
    {
        CurrentPlayerIndex = 0;
        TurnType = TurnTypes.Place;
        archerPlacer.enabled = true;
    }

    void StartBattleTurns()
    {
        itemSpawner.Initialise(archerPlacer.Planes.ToArray());
        archerPlacer.enabled = false;

        CurrentPlayerIndex = 0;
        TurnType = TurnTypes.Battle;
        Players[CurrentPlayerIndex].Archer.SetActive(true);
    }

    public void EndPlaceTurn(Archer archer)
    {
        Players[CurrentPlayerIndex].SetArcher(archer);

        if (!NextTurn())
        {
            StartBattleTurns();
        }
    }

    public void EndBattleTurn()
    {
        if (!NextTurn())
        {
            CurrentPlayerIndex = 0; // Wrap

            itemSpawner.Spawn();
        }

        Players[CurrentPlayerIndex].Archer.SetActive(true);
    }
}

public struct Player
{
    string name;
    public Archer Archer { get; private set; }

    public Player(string name)
    {
        this.name = name;
        Archer = null;
    }

    public void SetArcher(Archer archer)
    {
        Archer = archer;
    }
}