using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    [SerializeField] GameObject placeButtons;
    [SerializeField] GameObject placedButtons;
    [SerializeField] GameObject battleButtons;
    GameObject currentButtons;

    WizardPlacer wizardPlacer;
    GameManager gameManager;

    public static GameCanvas Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EnableButtons(placeButtons);

        wizardPlacer = WizardPlacer.Instance;
        gameManager = GameManager.Instance;
    }

    public void EnableButtons(GameObject buttons)
    {
        if (currentButtons != null)
        {
            currentButtons.SetActive(false);
        }
        buttons.gameObject.SetActive(true);
        currentButtons = buttons;
    }

    public void PlaceButton()
    {
        EnableButtons(placedButtons);
        wizardPlacer.PendWizardOnPlane();
    }

    public void ReplaceButton()
    {
        EnableButtons(placeButtons);
        wizardPlacer.ReplaceWizard();
    }

    public void EndTurnPlaceButton()
    {
        wizardPlacer.ConfirmWizardOnPlane();
        EnableButtons(gameManager.TurnType == GameManager.TurnTypes.Place
            ? placeButtons
            : battleButtons);
    }

    public void EndBattleTurnButton()
    {
        gameManager.EndBattleTurn();
    }
}