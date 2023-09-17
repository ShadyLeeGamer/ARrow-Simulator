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
            gameManager.CurrentPlayer.archer.TurnHorizontally(joystick.Direction.x);
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
        gameManager.CurrentPlayer.archer.FireBegin();
    }

    public void FirePowerSliderHold()
    {
        gameManager.CurrentPlayer.archer.FireHold(powerSlider.value);
    }

    public void FirePowerSliderRelease()
    {
        gameManager.CurrentPlayer.archer.FireRelease(powerSlider.value);
        powerSlider.value = 0;
    }

    public void SetPowerSlider(float value)
    {
        powerSlider.value = value;
    }
}