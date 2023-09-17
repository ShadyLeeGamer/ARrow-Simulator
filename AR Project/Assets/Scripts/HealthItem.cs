using UnityEngine;

public class HealthItem : Item
{
    [SerializeField] int health;

    public override void Use()
    {
        gameManager.CurrentPlayer.archer.AddHealth(health);

        base.Use(); // Destroy
    }
}