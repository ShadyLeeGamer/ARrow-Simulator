using System;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    [SerializeField] GameObject placeButtons;
    [SerializeField] GameObject placedButtons;
    [SerializeField] GameObject battleButtons;
    GameObject currentButtons;

    [SerializeField] FixedJoystick joystick;
    [SerializeField] Slider powerSlider;

    ArcherPlacer archerPlacer;
    GameManager gameManager;

    public static GameCanvas Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EnableButtons(placeButtons);

        archerPlacer = ArcherPlacer.Instance;
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (gameManager.TurnType == GameManager.TurnTypes.Battle &&
            joystick.Direction != Vector2.zero)
        {
            gameManager.CurrentPlayer.Archer.TurnHorizontally(joystick.Direction.x);
        }
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
        archerPlacer.PendOnPlane();
    }

    public void ReplaceButton()
    {
        EnableButtons(placeButtons);
        archerPlacer.Replace();
    }

    public void EndTurnPlaceButton()
    {
        archerPlacer.ConfirmOnPlane();
        EnableButtons(gameManager.TurnType == GameManager.TurnTypes.Place
            ? placeButtons
            : battleButtons);
    }

    public void FirePowerSliderBegin()
    {
        gameManager.CurrentPlayer.Archer.FireBegin();
    }

    public void FirePowerSliderHold()
    {
        gameManager.CurrentPlayer.Archer.FireHold(powerSlider.value);
    }

    public void FirePowerSliderRelease()
    {
        gameManager.CurrentPlayer.Archer.FireRelease(powerSlider.value);
        powerSlider.value = 0;
    }
}