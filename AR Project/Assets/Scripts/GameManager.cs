using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const int NUM_PLAYER = 2;

    Player[] players = new Player[NUM_PLAYER];
    public int CurrentPlayerIndex { get; private set; }
    public Player CurrentPlayer => players[CurrentPlayerIndex];

    public enum TurnTypes { Place, Battle }
    public TurnTypes TurnType { get; private set; }

    WizardPlacer wizardPlacer;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        wizardPlacer = WizardPlacer.Instance;

        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new();
        }

        StartPlaceTurns();
    }

    bool NextTurn()
    {
        if (CurrentPlayerIndex < NUM_PLAYER - 1)
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
        wizardPlacer.enabled = true;
    }

    void StartBattleTurns()
    {
        wizardPlacer.enabled = false;

        CurrentPlayerIndex = 0;
        TurnType = TurnTypes.Battle;
        players[CurrentPlayerIndex].Wizard.SetActive(true);
    }

    public void EndPlaceTurn(Wizard wizard)
    {
        players[CurrentPlayerIndex].SetWizard(wizard);

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
        }

        players[CurrentPlayerIndex].Wizard.SetActive(true);
    }
}

public struct Player
{
    public string name;
    public Wizard Wizard { get; private set; }

    public void SetWizard(Wizard wizard)
    {
        Wizard = wizard;
    }
}