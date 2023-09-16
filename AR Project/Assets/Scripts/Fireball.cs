using System;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] LayerMask hitMask;

    [SerializeField] float radius;

    Vector3 velocity;
    Vector3 lastPos;

    GameManager gameManager;

    public Action<Fireball> OnExplode;

    MeshRenderer renderer;
    TrailRenderer trailRenderer;

    public void Initialise(Vector3 startVelocity)
    {
        velocity = startVelocity;
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = true;
    }

    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();

        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        velocity += Physics.gravity * Time.deltaTime;
        lastPos = transform.position;
        transform.position += velocity * Time.deltaTime;

        float maxDistance = Vector3.Distance(transform.position, lastPos);
        if (Physics.SphereCast(lastPos, radius, velocity.normalized,
            out RaycastHit hit, maxDistance, hitMask))
        {
            if (hit.transform.TryGetComponent<Archer>(out var otherArcher))
            {
                if (otherArcher != gameManager.CurrentPlayer.Archer)
                {
                    otherArcher.TakeDamage(damage);
                    Explode();
                }
            }
            else if (hit.transform.TryGetComponent<Item>(out var item))
            {
                item.Use();
                Explode();
            }
        }
        else if (transform.position.y <= PlanesTrackedMetrics.BedrockPosY)
        {
            Explode();
        }
    }

    void Explode()
    {
        gameManager.CurrentPlayer.Archer.SaveLastTrajectory(trailRenderer);
        gameManager.EndBattleTurn();

        renderer.enabled = false;
        Destroy(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}