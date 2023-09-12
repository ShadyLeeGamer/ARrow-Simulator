using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class WizardPlacer : MonoBehaviour
{
    [SerializeField] Camera cam;

    [SerializeField] Transform ghost;
    [SerializeField] Wizard wizardPrefab;

    Vector3 wizardPlacePos;
    Quaternion wizardPlaceRot;
    ARPlane wizardPlane;
    Pose wizardPose;

    Wizard pendingWizard;
    List<ARPlane> wizardPlanes;
    int bedrockIndex = -1;

    ARRaycastManager raycastManager;
    ARPlaneManager planeManager;
    ARAnchorManager anchorManager;

    [SerializeField] Material platformMat, bedrockMat, wallCeilingMat;

    GameManager gameManager;

    public static WizardPlacer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        anchorManager = GetComponent<ARAnchorManager>();
    }

    private void Start()
    {
        wizardPlanes = new (GameManager.NUM_PLAYER);
        gameManager = GameManager.Instance;

        planeManager.planesChanged += OnPlanesChanged;
    }

    private void Update()
    {
        ghost.gameObject.SetActive(pendingWizard == null);

        if (pendingWizard == null)
        {
            PreviewGhostOnPlane();
        }
    }

    void PreviewGhostOnPlane()
    {
        Vector2 screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new();
        if (raycastManager.Raycast(screenCenter, hits,
            TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];

            ARPlane plane = planeManager.GetPlane(hit.trackableId);
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                wizardPose = hit.pose;
                wizardPlane = plane;

                ghost.position = wizardPlacePos = wizardPose.position;

                Vector3 dirToCam = cam.transform.position - wizardPlacePos;
                dirToCam.y = 0;
                wizardPlaceRot = Quaternion.LookRotation(dirToCam);
                ghost.rotation = wizardPlaceRot;
            }
        }
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs planesChanged)
    {
        foreach (ARPlane plane in planesChanged.added)
        {
            MeshRenderer planeMeshRenderer = plane.GetComponent<MeshRenderer>();
            switch (plane.alignment)
            {
                case PlaneAlignment.HorizontalUp:
                    planeMeshRenderer.material = platformMat;
                    break;
                case PlaneAlignment.Vertical:
                case PlaneAlignment.HorizontalDown:
                    planeMeshRenderer.material = wallCeilingMat;
                    break;
            }
        }
    }

    public void PendWizardOnPlane()
    {
        pendingWizard = Instantiate(wizardPrefab, wizardPlacePos, wizardPlaceRot);
        pendingWizard.Initialise(cam);
        pendingWizard.gameObject.AddComponent<ARAnchor>();
    }

    void TrackWizardPlane(ARPlane wizardPlane)
    {
        if (wizardPlane.transform.position.y < PlanesTrackedMetrics.BedrockPosY)
        {
            if (bedrockIndex != -1)
            {
                wizardPlanes[bedrockIndex]
                    .GetComponent<MeshRenderer>().material = platformMat;
            }
            wizardPlane.GetComponent<MeshRenderer>().material = bedrockMat;
            bedrockIndex = gameManager.CurrentPlayerIndex;
            PlanesTrackedMetrics.SetBedrockPosY(wizardPlane.transform.position.y);
        }

        wizardPlanes.Add(wizardPlane);
    }

    public void ConfirmWizardOnPlane()
    {
        TrackWizardPlane(wizardPlane);

        gameManager.EndPlaceTurn(pendingWizard);
        pendingWizard = null;
    }

    public void ReplaceWizard()
    {
        Destroy(pendingWizard.gameObject);
    }

    public void OnDisable()
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            if (!wizardPlanes.Contains(plane)) // Remove other planes
            {
                plane.gameObject.SetActive(false);
            }
            else
            {
                plane.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        planeManager.enabled = false;
    }
}