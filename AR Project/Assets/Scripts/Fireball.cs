using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    Vector3 velocity;

    public void Initialise(Vector3 startVelocity)
    {
        velocity = startVelocity;
    }

    private void Update()
    {
        velocity -= Physics.gravity;
        transform.position += velocity;
    }
}