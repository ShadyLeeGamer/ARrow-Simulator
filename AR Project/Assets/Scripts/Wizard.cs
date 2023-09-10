using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] float firePower;
    [SerializeField] Fireball fireballPrefab;

    ProjectionDrawer projectionDrawer;

    private void Awake()
    {
        projectionDrawer = GetComponent<ProjectionDrawer>();
    }

    private void Update()
    {
        projectionDrawer.Draw(firePoint.position, firePoint.up * firePower);
    }

    public void Fire()
    {
        Instantiate(fireballPrefab, firePoint.position, Quaternion.identity)
            .Initialise(firePoint.up * firePower);
    }
}