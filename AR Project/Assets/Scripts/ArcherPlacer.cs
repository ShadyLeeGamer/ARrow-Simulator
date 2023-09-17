using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ArcherPlacer : MonoBehaviour
{
    [SerializeField] Camera cam;

    [SerializeField] ArcherPainter ghost;
    [SerializeField] Archer archerPrefab;

    Vector3 placePos;
    Quaternion placeRot;
    ARPlane plane;
    Pose pose;

    Archer pendingArcher;

    public List<ARPlane> Planes { get; private set; }
    int bedrockIndex = -1;

    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] ARAnchorManager anchorManager;

    [SerializeField] Material platformMat, bedrockMat, wallCeilingMat;

    GameManager gameManager;

    public static ArcherPlacer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        planeManager.planesChanged += OnPlanesChanged;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        Planes = new List<ARPlane>(gameManager.Players.Length);
        planeManager.enabled = raycastManager.enabled = anchorManager.enabled = true;

        RefreshGhost();
    }

    private void Update()
    {
        ghost.gameObject.SetActive(pendingArcher == null);

        if (pendingArcher == null)
        {
            PreviewGhostOnPlane();
        }
    }

    void RefreshGhost()
    {
        ghost.PaintWhole(gameManager.CurrentPlayerIndex);
    }

    void PreviewGhostOnPlane()
    {
        Vector2 screenCenter = cam.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hits = new();
        if (raycastManager.Raycast(screenCenter, hits,
            TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];

            ARPlane plane = planeManager.GetPlane(hit.trackableId);
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                pose = hit.pose;
                this.plane = plane;

                ghost.transform.position = placePos = pose.position;

                Vector3 dirToCam = cam.transform.position - placePos;
                dirToCam.y = 0;
                placeRot = Quaternion.LookRotation(dirToCam);
                ghost.transform.rotation = placeRot;
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

    public void PendOnPlane()
    {
        pendingArcher = Instantiate(archerPrefab, placePos, placeRot);
        pendingArcher.Initialise(
            cam, gameManager.CurrentPlayer.Name, gameManager.CurrentPlayerIndex);
        pendingArcher.gameObject.AddComponent<ARAnchor>();
    }

    void TrackPlane(ARPlane archerPlane)
    {
        if (archerPlane.transform.position.y < PlanesTrackedMetrics.BedrockPosY)
        {
            if (bedrockIndex != -1)
            {
                Planes[bedrockIndex]
                    .GetComponent<MeshRenderer>().material = platformMat;
            }
            archerPlane.GetComponent<MeshRenderer>().material = bedrockMat;
            bedrockIndex = gameManager.CurrentPlayerIndex;
            PlanesTrackedMetrics.SetBedrockPosY(archerPlane.transform.position.y);
        }

        Planes.Add(archerPlane);
    }

    public void ConfirmOnPlane()
    {
        TrackPlane(plane);

        gameManager.EndPlaceTurn(pendingArcher);

        RefreshGhost();

        pendingArcher = null;
    }

    public void Replace()
    {
        Destroy(pendingArcher.gameObject);
    }

    public void OnDisable()
    {
/*        foreach (ARPlane plane in planeManager.trackables)*/
        foreach (ARPlane plane in Planes)
        {
            Destroy(plane.gameObject);
        }

        ARPlane[] planes = FindObjectsOfType<ARPlane>();
        foreach (ARPlane plane in planes)
        {
            Destroy(plane.gameObject);
        }

        planeManager.enabled = raycastManager.enabled = anchorManager.enabled = false;
    }
}