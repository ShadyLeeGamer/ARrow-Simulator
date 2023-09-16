using UnityEngine;

public class Archer : MonoBehaviour
{
    [SerializeField] float horizontalTurnSpeed;

    [SerializeField] Transform firePoint;
    [SerializeField] Fireball fireballPrefab;
    [SerializeField] float firePowerMin, firePowerMax;
    [SerializeField] float firePower;

    [SerializeField] Transform neckRig;

    [SerializeField] Canvas UI;

    [SerializeField] Bar healthBar;
    [SerializeField] int maxHealth;
    int health;

    Camera cam;

    [SerializeField] Animator animator;
    ProjectionDrawer projectionDrawer;

    [SerializeField] float fireballPosAimOffset;
    TrailRenderer lastTrajectory;
    Fireball fireball;

    public bool Active { get; set; }

    public void Initialise(Camera cam)
    {
        this.cam = cam;
    }

    private void Awake()
    {
        projectionDrawer = GetComponent<ProjectionDrawer>();
    }

    private void Start()
    {
        healthBar.SetMaxValue(health = maxHealth);
    }

    public void TurnHorizontally(float deltaX)
    {
        if (!Active)
            return;

        transform.Rotate(0f, deltaX * horizontalTurnSpeed, 0f);
    }

    private void Update()
    {
        Vector3 dirToCam = cam.transform.position - transform.position;
        UI.transform.rotation = Quaternion.LookRotation(dirToCam);

        if (!Active)
            return;
    }

    public void FireBegin()
    {
        if (!Active)
            return;
        fireball =
            Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireball.enabled = false;
    }

    public void FireHold(float powerPercent)
    {
        if (!Active)
            return;

        firePoint.eulerAngles = new Vector3(Mathf.Lerp(90, -90, powerPercent),
            firePoint.eulerAngles.y, firePoint.eulerAngles.z);
        fireball.transform.position = firePoint.position
            + -firePoint.forward * fireballPosAimOffset;
    }

    public void FireRelease(float powerPercent)
    {
        if (!Active)
            return;

        Vector3 fireVelocity = firePoint.forward * firePower;
        fireball.enabled = true;
        fireball.Initialise(fireVelocity);
        //projectionDrawer.DrawWhole(firePoint.position, fireVelocity);

        animator.Play("Fire");

        SetActive(false);
    }

    public void SaveLastTrajectory(TrailRenderer lastTrajectory)
    {
        if (this.lastTrajectory != null)
        {
            Destroy(this.lastTrajectory.gameObject);
        }

        this.lastTrajectory = lastTrajectory;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health > 0)
        {
            healthBar.SetValue(health);
            animator.Play("Hurt");
        }
        else
        {
            Die();
        }
    }

    public void AddHealth(int add)
    {
        healthBar.SetValue(
            health = Mathf.Clamp(health + add, 0, maxHealth));
    }

    void Die()
    {
        animator.Play("Death");
        Destroy(lastTrajectory.gameObject);
        Destroy(gameObject);
    }

    public void SetActive(bool active)
    {
        Active = active;

        if (!active)
        {
            projectionDrawer.Clear();
        }
    }
}