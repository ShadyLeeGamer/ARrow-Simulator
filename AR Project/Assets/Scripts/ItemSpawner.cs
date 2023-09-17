using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    Bounds bounds;

    [SerializeField] Item[] itemPrefabs;

    public static ItemSpawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Initialise(List<UnityEngine.XR.ARFoundation.ARPlane> wizardPlanes)
    {
        bounds = wizardPlanes[0].GetComponent<MeshCollider>().bounds;
        foreach (var plane in wizardPlanes)
        {
            bounds.Encapsulate(plane.GetComponent<MeshCollider>().bounds);
        }
    }

    float GetRandomPos(float extent) => Random.Range(-extent, extent);

    public void Spawn()
    {
        Vector3 offset = new(
            GetRandomPos(bounds.extents.x),
            GetRandomPos(bounds.extents.y),
            GetRandomPos(bounds.extents.z));
        Item newItem =
            Instantiate(itemPrefabs[0], bounds.center + offset, itemPrefabs[0].transform.rotation);
    }
}