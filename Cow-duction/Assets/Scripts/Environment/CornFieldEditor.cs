using UnityEngine;

public class CornFieldEditor : MonoBehaviour
{
    public int length, width = 1;
    public float cornSizeFactor, cornHeightFactor, pixelSize = 1;
    public GameObject cornstalk;
    public enum DrawMode {Never, Selected, Always};
    public DrawMode drawMode = DrawMode.Selected;

    private Vector3[,] cornPositions;
    private Vector3 lastAngle;

    private void OnValidate()
    {
        if (cornstalk == null) return;

        GeneratePositions();
    }

    private void OnDrawGizmos()
    {
        if (drawMode != DrawMode.Always) return;

        DrawGizmoLines();
    }

    private void OnDrawGizmosSelected()
    {
        if (drawMode == DrawMode.Never) return;

        DrawGizmoLines();
    }

    private void DrawGizmoLines()
    {
        if (cornPositions[0,0] != transform.position || transform.eulerAngles != lastAngle)
        {
            GeneratePositions();
        }

        Gizmos.color = Color.yellow;
        foreach (Vector3 pos in cornPositions)
        {
            Gizmos.DrawLine(pos, pos + transform.up * cornHeightFactor * 4f);
        }
    }

    private void Start()
    {
        foreach (Vector3 pos in cornPositions)
        {
            GameObject cornClone = Instantiate(cornstalk, pos, transform.rotation, transform);
            cornClone.transform.localScale = new Vector3(cornSizeFactor, cornHeightFactor, cornSizeFactor);
        }
    }

    public void GeneratePositions()
    {
        if (length < 1 || width < 1) return;

        cornPositions = new Vector3[length, width];

        for (int l = 0; l < length; l++)
        {
            for (int w = 0; w < width; w++)
            {
                cornPositions[l, w] = transform.localPosition + ((transform.right * w + transform.forward * l) * cornSizeFactor * pixelSize);
            }
        }

        lastAngle = transform.eulerAngles;
    }
}
