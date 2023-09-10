using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class InputManager : MonoBehaviour
{
    [SerializeField] Camera cam;

    [SerializeField] Transform ghost;
    [SerializeField] Wizard wizardPrefab;
    [SerializeField] LayerMask wizardLayerMask;

    private bool horizontalRotationInput = false;
    [SerializeField] float horizontalRotationSpeed;

    Vector3 wizardPlacePos;
    Quaternion wizardPlaceRot;
    ARPlane wizardPlane;
    Pose wizardPose;

    Wizard currentWizard;

    ARRaycastManager aRRaycastManager;
    ARPlaneManager aRPlaneManager;
    ARAnchorManager aRAnchorManager;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
        aRAnchorManager = GetComponent<ARAnchorManager>();
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable(); // Enable mouse input to simulate touch
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += OnFingerDown;
        EnhancedTouch.Touch.onFingerMove += OnFingerMove;
        EnhancedTouch.Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= OnFingerDown;
        EnhancedTouch.Touch.onFingerMove -= OnFingerMove;
        EnhancedTouch.Touch.onFingerUp -= OnFingerUp;
    }

    private void Update()
    {
        ghost.gameObject.SetActive(currentWizard == null);

        if (currentWizard == null)
        {
            GhostOnPlane();
        }
    }

    void GhostOnPlane()
    {
        Vector2 screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new();
        if (aRRaycastManager.Raycast(screenCenter, hits,
            TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];
            wizardPose = hit.pose;
            wizardPlane = aRPlaneManager.GetPlane(hit.trackableId);

            if (wizardPlane.alignment == PlaneAlignment.HorizontalUp)
            {
                ghost.position = wizardPlacePos = wizardPose.position;

                Vector3 dirToCam = cam.transform.position - wizardPlacePos;
                dirToCam.y = 0;
                wizardPlaceRot = Quaternion.LookRotation(dirToCam);
                ghost.rotation = wizardPlaceRot;
            }
        }
    }

    void OnFingerDown(EnhancedTouch.Finger finger)
    {
        // Ignore multi-touch
        if (finger.index != 0 ||
            IsPointerOverUI(finger.currentTouch.screenPosition))
            return;

        // Placing
        if (currentWizard == null)
        {
            WizardOnPlane();
        }
        else
        {
            Ray ray = cam.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, wizardLayerMask))
            {
                if (hit.transform.GetComponent<Wizard>())
                {
                    horizontalRotationInput = true;
                }
            }

            currentWizard.Fire();
        }
    }

    void WizardOnPlane()
    {
        currentWizard = Instantiate(wizardPrefab, wizardPlacePos, wizardPlaceRot);

        currentWizard.gameObject.AddComponent<ARAnchor>();
        //aRAnchorManager.AttachAnchor(wizardPlane, wizardPose);
    }

    void OnFingerMove(EnhancedTouch.Finger finger)
    {
        // Ignore multi-touch
        if (finger.index != 0 ||
            IsPointerOverUI(finger.currentTouch.screenPosition))
            return;

        if (currentWizard != null)
        {
            HorizontalRotation(finger.currentTouch.delta);
        }
    }

    void HorizontalRotation(Vector2 touchDeltaPosition)
    {
        if (horizontalRotationInput)
        {
            Touch touch = Input.GetTouch(0);
            currentWizard.transform.Rotate(0f,
                touch.deltaPosition.x * horizontalRotationSpeed, 0f);
        }
    }

    void OnFingerUp(EnhancedTouch.Finger finger)
    {
        horizontalRotationInput = false;
    }

    public void RemoveCurrentObj()
    {
        Destroy(currentWizard.gameObject);
    }

    bool IsPointerOverUI(Vector2 touchPosition)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}