using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    [SerializeField] GameObject placeButtons;
    [SerializeField] GameObject placedButtons;
    [SerializeField] GameObject battleButtons;
    GameObject currentButtons;

    [SerializeField] FixedJoystick joystick;
    [SerializeField] Slider powerSlider;

    WizardPlacer wizardPlacer;
    GameManager gameManager;

    public Action<Vector2> OnJoystickInput;

    public Action<float> OnFireReleaseInput;
    public Action<float> OnFireHoldInput;

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

    private void Update()
    {
        if (gameManager.TurnType == GameManager.TurnTypes.Battle &&
            joystick.Direction != Vector2.zero)
        {
            OnJoystickInput?.Invoke(joystick.Direction);
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

    public void FirePowerSliderHold()
    {
        OnFireHoldInput?.Invoke(powerSlider.value);
    }

    public void FirePowerSliderRelease()
    {
        OnFireReleaseInput?.Invoke(powerSlider.value);
        powerSlider.value = 0;
    }
}