using UnityEngine;

public class Archer : MonoBehaviour
{
    int colourIndex;

    [SerializeField] float horizontalTurnSpeed;

    [SerializeField] Transform firePoint;
    [SerializeField] Arrow arrowPrefab;
    [SerializeField] float firePower;

    [SerializeField] Transform tiltX, tiltY;

    ArcherPainter painter;

    [SerializeField] Canvas UI;
    [SerializeField] UnityEngine.UI.Text nameLabel;

    [SerializeField] Bar healthBar;
    [SerializeField] int maxHealth;
    int health;

    Camera cam;

    [SerializeField] Animator animator;
    ProjectionDrawer projectionDrawer;

    TrailRenderer lastTrajectory;

    public float LastYRotation { get; private set; }

    public bool Active { get; set; }

    private void Awake()
    {
        projectionDrawer = GetComponent<ProjectionDrawer>();
        painter = GetComponent<ArcherPainter>();
    }

    public void Initialise(Camera cam, string name, int colourIndex)
    {
        this.cam = cam;

        nameLabel.text = name;

        this.colourIndex = colourIndex;
        painter.PaintHair(colourIndex);
        projectionDrawer.SetColour(colourIndex);
    }

    private void Start()
    {
        healthBar.SetMaxValue(health = maxHealth);
    }

    public void TurnHorizontally(float deltaX)
    {
        if (!Active)
            return;

        tiltX.Rotate(0f, deltaX * horizontalTurnSpeed, 0f);
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
    }

    Vector3 FireVelocity => firePoint.forward * firePower;

    public void FireHold(float powerPercent)
    {
        if (!Active)
            return;

        tiltY.eulerAngles = new Vector3(Mathf.Lerp(90, -90, powerPercent),
            tiltY.eulerAngles.y, tiltY.eulerAngles.z);

        projectionDrawer.DrawWhole(firePoint.position, FireVelocity);

        animator.Play("FireHold");
    }

    public void FireRelease(float powerPercent)
    {
        if (!Active)
            return;

        projectionDrawer.Clear();

        Instantiate(arrowPrefab, firePoint.position, Quaternion.identity)
            .Initialise(FireVelocity, colourIndex);
        LastYRotation = powerPercent;

        animator.Play("FireRelease");

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
    }
}