using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected GameManager gameManager;

    protected virtual void Start()
    {
        gameManager = GameManager.Instance;
    }

    public virtual void Use()
    {
        Destroy(gameObject);
    }
}