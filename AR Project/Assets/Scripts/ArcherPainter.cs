using UnityEngine;

public class ArcherPainter : MonoBehaviour
{
    [System.Serializable]
    public class Part
    {
        public SkinnedMeshRenderer[] renderers;
        public Material[] colourMats;
    }

    [SerializeField] Part hair, face, clothes;

    public void PaintHair(int colourIndex)
    {
        Paint(hair, colourIndex);
    }

    public void PaintWhole(int colourIndex)
    {
        Paint(hair, colourIndex);
        Paint(face, colourIndex);
        Paint(clothes, colourIndex);
    }

    void Paint(Part part, int colourIndex)
    {
        Material colourMat = part.colourMats[colourIndex];

        foreach (SkinnedMeshRenderer renderer in part.renderers)
        {
            renderer.material = colourMat;
        }
    }
}