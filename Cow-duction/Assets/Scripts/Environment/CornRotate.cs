using System.Collections;
using UnityEngine;

public class CornRotate : MonoBehaviour
{
    public Color deadColor = new Color(.8f, .4f, .1f, .5f);
    public bool canDie = true;
    public bool animate = true;

    private Renderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        RandomizeRotation(transform, true);
    }

    public void RandomizeRotation(Transform tr, bool up)
    {
        if (transform.childCount < 1) return;

        foreach (Transform child in tr)
        {
            if (child.GetComponent<MeshRenderer>() == null)
            {
                RandomizeRotation(child, up);
            }
            else
            {
                if (up)
                {
                    tr.localEulerAngles = new Vector3(0f, Random.Range(-180f, 180f), 0f);
                }
                else
                {
                    if (animate)
                    {
                        StartCoroutine(AnimateRotation(tr, new Vector3(90f, tr.localEulerAngles.y, tr.localEulerAngles.z)));
                    }
                    else
                    {
                        tr.localEulerAngles = new Vector3(90f, tr.localEulerAngles.y, tr.localEulerAngles.z);
                    }
                }
            }
        }
    }

    private IEnumerator AnimateRotation(Transform tr, Vector3 angles)
    {
        while (tr.localEulerAngles != angles)
        {
            tr.localEulerAngles = Vector3.Lerp(tr.localEulerAngles, angles, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetColor(Color color)
    {
        foreach (Renderer renderer in renderers)
        {
            if (animate)
            {
                StartCoroutine(AnimateColor(renderer, color));
            }
            else
            {
                renderer.material.SetColor("_BaseColor", color);
            }
        }
    }

    private IEnumerator AnimateColor(Renderer renderer, Color color)
    {
        while (!renderer.material.GetColor("_BaseColor").Equals(color))
        {
            renderer.material.SetColor("_BaseColor", Color.Lerp(renderer.material.GetColor("_BaseColor"), color, Time.deltaTime));
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (canDie)
        {
            KnockDown();
        }
    }

    public void KnockDown()
    {
        RandomizeRotation(transform, false);
        SetColor(deadColor);
        canDie = false;
    }
}
