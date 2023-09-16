using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] Text label;
    public InputField Field { get; private set; }

    private void Awake()
    {
        Field = GetComponent<InputField>();
    }

    public void RefershLabel(int playerNo)
    {
        label.text = "Player " + playerNo;
    }

    public void RemoveBtn()
    {
        MainMenuCanvas.Instance.RemovePlayer(Field);
    }
}