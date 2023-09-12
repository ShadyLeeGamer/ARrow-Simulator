using UnityEngine;

public class Wizard : MonoBehaviour
{
    [SerializeField] float horizontalTurnSpeed;
    [SerializeField] float verticalTurnSpeed;

    [SerializeField] Transform firePoint;
    [SerializeField] Fireball fireballPrefab;
    [SerializeField] float firePowerMin, firePowerMax;

    [SerializeField] Transform neckRig;

    [SerializeField] Canvas UI;

    InputManager inputManager;
    GameCanvas gameCanvas;

    ProjectionDrawer projectionDrawer;

    Camera cam;

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
        inputManager = InputManager.Instance;
        inputManager.OnHorizontalRotateInput += TurnHorizontally;
        inputManager.OnVerticalRotateInput += TurnVertically;

        gameCanvas = GameCanvas.Instance;
        gameCanvas.OnFireHoldInput += FireHold;
        gameCanvas.OnFireReleaseInput += FireRelease;
    }

    void TurnHorizontally(float deltaX)
    {
        if (!Active)
            return;

        transform.Rotate(0f, deltaX * horizontalTurnSpeed, 0f);
    }

    void TurnVertically(float deltaY)
    {
        if (!Active)
            return;

        firePoint.Rotate(deltaY * verticalTurnSpeed, 0f, 0f);
        float xClamped = firePoint.eulerAngles.x;
        if (xClamped > 90)
            xClamped = 90;
        else if (xClamped < -90)
            xClamped = -90;
        firePoint.eulerAngles = new Vector3(xClamped,
            firePoint.eulerAngles.y, firePoint.eulerAngles.z);
        neckRig.localEulerAngles = new Vector3(0f, 0f, xClamped);
    }

    private void Update()
    {
        Vector3 dirToCam = cam.transform.position - transform.position;
        UI.transform.rotation = Quaternion.LookRotation(dirToCam);

        if (!Active)
            return;
    }

    Vector3 FireVelocity(float percent) => firePoint.forward *
        Mathf.Lerp(firePowerMin, firePowerMax, percent);

    public void FireHold(float powerPercent)
    {
        projectionDrawer.Draw(firePoint.position, FireVelocity(powerPercent));
    }

    public void FireRelease(float powerPercent)
    {
        if (!Active)
            return;

        Instantiate(fireballPrefab, firePoint.position, Quaternion.identity)
            .Initialise(FireVelocity(powerPercent));
    }

    public void SetActive(bool active)
    {
        Active = active;

        if (!active)
        {
            projectionDrawer.Clear();
        }
    }

    private void OnDestroy()
    {
        inputManager.OnHorizontalRotateInput -= TurnHorizontally;
        inputManager.OnHorizontalRotateInput -= TurnVertically;

        gameCanvas.OnFireHoldInput -= FireHold;
        gameCanvas.OnFireReleaseInput -= FireRelease;
    }
}