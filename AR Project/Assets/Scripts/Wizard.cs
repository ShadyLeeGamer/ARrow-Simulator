using UnityEngine;

public class Wizard : MonoBehaviour
{
    [SerializeField] float horizontalTurnSpeed;
    [SerializeField] float verticalTurnSpeed;

    [SerializeField] Transform firePoint;
    [SerializeField] float firePower;
    [SerializeField] Fireball fireballPrefab;

    InputManager inputManager;

    ProjectionDrawer projectionDrawer;

    bool isRotating;

    public bool Active { get; set; }

    private void Awake()
    {
        projectionDrawer = GetComponent<ProjectionDrawer>();
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
        inputManager.OnHorizontalRotateInput += TurnHorizontally;
        inputManager.OnVerticalRotateInput += TurnVertically;
        inputManager.OnFireInput += Fire;
    }

    void TurnHorizontally(float deltaX)
    {
        if (!Active)
            return;

        transform.Rotate(0f, deltaX * horizontalTurnSpeed, 0f);

        isRotating = true;
    }

    void TurnVertically(float deltaY)
    {
        if (!Active)
            return;

        firePoint.Rotate(deltaY * verticalTurnSpeed, 0f, 0f);
        firePoint.localEulerAngles = new Vector3(
            Mathf.Clamp(firePoint.eulerAngles.x, 0f, 180f), 0f, 0f);

        isRotating = true;
    }

    private void Update()
    {
        if (!Active)
            return;

        if (isRotating)
        {
            projectionDrawer.Draw(firePoint.position, firePoint.up * firePower);
        }
        else
        {
            isRotating = false;
        }
    }

    public void Fire()
    {
        if (!Active)
            return;

/*        Instantiate(fireballPrefab, firePoint.position, Quaternion.identity)
            .Initialise(firePoint.up * firePower);*/
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
        inputManager.OnFireInput -= Fire;
    }
}