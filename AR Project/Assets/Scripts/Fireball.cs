using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] LayerMask wizardMask;

    float radius;

    Vector3 velocity;
    Vector3 lastPos;

    GameManager gameManager;

    public void Initialise(Vector3 startVelocity)
    {
        velocity = startVelocity;
    }

    private void Start()
    {
        radius = GetComponent<SphereCollider>().radius;

        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        velocity += Physics.gravity * Time.deltaTime;
        lastPos = transform.position;
        transform.position += velocity * Time.deltaTime;

        float maxDistance = Vector2.Distance(transform.position, lastPos);
        if (Physics.SphereCast(lastPos, radius, velocity.normalized,
            out RaycastHit hit, maxDistance, wizardMask))
        {
            var otherWizard = hit.transform.GetComponent<Wizard>();
            if (otherWizard != null && otherWizard != gameManager.CurrentPlayer.Wizard)
            {
                otherWizard.TakeDamage(damage);
                Explode();
            }
        }

        if (transform.position.y <= PlanesTrackedMetrics.BedrockPosY)
        {
            Explode();
        }
    }

    void Explode()
    {
        gameManager.EndBattleTurn();
        Destroy(gameObject);
    }
}