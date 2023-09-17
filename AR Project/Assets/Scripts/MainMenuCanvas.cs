using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] PlayerNameInput playerNameInputPrefab;
    [SerializeField] Transform playerNameInputLayerGroup;

    InputField[] playerNameInputs;
    int playerNameInputsLength;

    GameManager gameManager;

    public static MainMenuCanvas Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerNameInputs = new InputField[GameManager.MAX_NUM_PLAYERS];

        AddPlayerBtn();
        AddPlayerBtn();

        gameManager = GameManager.Instance;
    }

    public void AddPlayerBtn()
    {
        if (playerNameInputsLength == GameManager.MAX_NUM_PLAYERS)
            return;

        PlayerNameInput nameInput =
            Instantiate(playerNameInputPrefab, playerNameInputLayerGroup);
        playerNameInputs[playerNameInputsLength] = nameInput.Field;
        playerNameInputsLength++;
        nameInput.RefershLabel(playerNameInputsLength);
        
    }

    public void RemovePlayer(InputField playerNameInput)
    {
        if (playerNameInputsLength == GameManager.MIN_NUM_PLAYERS)
            return;

        Destroy(playerNameInput.gameObject);
        playerNameInputsLength--;
    }

    public void StartBtn()
    {
        Debug.Log(playerNameInputsLength);
        gameManager.StartGame(playerNameInputs, playerNameInputsLength);
        gameObject.SetActive(false);
    }
}