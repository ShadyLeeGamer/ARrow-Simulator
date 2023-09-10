using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlanesTrackedManager : MonoBehaviour
{
    [SerializeField] ARPlaneManager planeManager;

    [SerializeField] Material platformMat, bedrockMat, wallCeilingMat;

    ARPlane bedrock;

    public float BedrockPosY => bedrock.transform.position.y;

    public static ARPlanesTrackedManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        planeManager.planesChanged += OnPlanesChanged;
        InputManager.Instance.OnWizardPlaced += RemoveOtherPlanes;
    }

    public void OnPlanesChanged(ARPlanesChangedEventArgs planesChanged)
    {
        foreach (ARPlane plane in planesChanged.added)
        {
            switch (plane.alignment)
            {
                case PlaneAlignment.HorizontalUp:
                    if (bedrock == null || plane.transform.position.y < BedrockPosY)
                    {
                        if (bedrock != null)
                        {
                            bedrock.GetComponent<MeshRenderer>().material = platformMat;
                        }
                        bedrock = plane;
                        bedrock.GetComponent<MeshRenderer>().material = bedrockMat;
                    }
                    else if (plane.trackableId != bedrock.trackableId)
                    {
                        plane.GetComponent<MeshRenderer>().material = platformMat;
                    }
                    break;
                case PlaneAlignment.Vertical:
                case PlaneAlignment.HorizontalDown:
                    plane.GetComponent<MeshRenderer>().material = wallCeilingMat;
                    break;
            }
        }
    }

    public void RemoveOtherPlanes(ARPlane wizardPlane)
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            if (plane.trackableId != wizardPlane.trackableId &&
                plane.trackableId != bedrock.trackableId)
            {
                plane.gameObject.SetActive(false);
            }
        }

        planeManager.enabled = false;
    }
}