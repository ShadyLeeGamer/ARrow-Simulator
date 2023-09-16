using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Item
{
    [SerializeField] int health;

    public override void Use()
    {
        gameManager.CurrentPlayer.Archer.AddHealth(health);
    }
}