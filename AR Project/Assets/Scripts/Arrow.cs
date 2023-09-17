using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] LayerMask hitMask;

    [SerializeField] float radius;

    Vector3 velocity;
    Vector3 lastPos;

    GameManager gameManager;

    [SerializeField] MeshRenderer renderer;
    TrailRenderer trailRenderer;

    [SerializeField] Color[] colours;

    public void Initialise(Vector3 startVelocity, int colourIndex)
    {
        velocity = startVelocity;

        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.material.color = colours[colourIndex];
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        velocity += Physics.gravity * Time.deltaTime;
        lastPos = transform.position;
        transform.SetPositionAndRotation(
            transform.position + velocity * Time.deltaTime,
            Quaternion.LookRotation(velocity.normalized));

        float maxDistance = Vector3.Distance(transform.position, lastPos);
        if (Physics.SphereCast(lastPos, radius, velocity.normalized,
            out RaycastHit hit, maxDistance, hitMask))
        {
            if (hit.transform.TryGetComponent<Archer>(out var otherArcher))
            {
                if (otherArcher != gameManager.CurrentPlayer.archer)
                {
                    otherArcher.TakeDamage(damage);
                    Hit();
                }
            }
            else if (hit.transform.TryGetComponent<Item>(out var item))
            {
                item.Use();
                Hit();
            }
        }
        else if (transform.position.y <= PlanesTrackedMetrics.BedrockPosY)
        {
            Hit();
        }
    }

    void Hit()
    {
        gameManager.CurrentPlayer.archer.SaveLastTrajectory(trailRenderer);
        gameManager.EndBattleTurn();

        renderer.enabled = false;
        /*Destroy(this);*/
        enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}