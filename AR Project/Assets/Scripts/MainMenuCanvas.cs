using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    const float MAX_NUM_PLAYERS = 4;
    const float MIN_NUM_PLAYERS = 2;

    [SerializeField] PlayerNameInput playerNameInputPrefab;
    [SerializeField] Transform playerNameInputLayerGroup;

    public List<InputField> PlayerNameInputs { get; private set; }

    GameManager gameManager;

    public static MainMenuCanvas Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerNameInputs = new List<InputField>();

        AddPlayerBtn();
        AddPlayerBtn();

        gameManager = GameManager.Instance;
    }

    public void AddPlayerBtn()
    {
        if (PlayerNameInputs.Count == MAX_NUM_PLAYERS)
            return;

        PlayerNameInput nameInput = Instantiate(playerNameInputPrefab, playerNameInputLayerGroup);
        PlayerNameInputs.Add(nameInput.Field);
        nameInput.RefershLabel(PlayerNameInputs.Count);
        
    }

    public void RemovePlayer(InputField playerNameInput)
    {
        if (PlayerNameInputs.Count == MIN_NUM_PLAYERS)
            return;

        Destroy(playerNameInput.gameObject);
        PlayerNameInputs.Remove(playerNameInput);
    }

    public void StartBtn()
    {
        gameManager.StartGame(PlayerNameInputs.ToArray());
        gameObject.SetActive(false);
    }
}