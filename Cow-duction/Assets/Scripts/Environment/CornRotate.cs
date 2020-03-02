using System.Collections;
using UnityEngine;

public class CornRotate : MonoBehaviour
{
    public Color deadColor = new Color(.8f, .4f, .1f, .5f);
    public bool canDie = true;
    public bool animate = true;

    // Start is called before the first frame update
    void Start()
    {
        RandomizeRotation(transform, true);
    }

    void RandomizeRotation(Transform tr, bool up)
    {
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
                    StartCoroutine(SetRotation(tr, new Vector3(90f, tr.localEulerAngles.y, tr.localEulerAngles.z)));
                }
            }
        }
    }

    IEnumerator SetRotation(Transform tr, Vector3 angles)
    {
        if (animate)
        {
            while (tr.localEulerAngles != angles)
            {
                tr.localEulerAngles = Vector3.Lerp(tr.localEulerAngles, angles, Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            tr.localEulerAngles = new Vector3(90f, tr.localEulerAngles.y, tr.localEulerAngles.z);
        }
    }

    IEnumerator SetColor(Color color)
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            if (animate)
            {
                while (renderer.material.GetColor("_BaseColor") != color)
                {
                    renderer.material.SetColor("_BaseColor", Color.Lerp(renderer.material.GetColor("_BaseColor"), color, Time.deltaTime));
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                renderer.material.SetColor("_BaseColor", color);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (canDie)
        {
            KnockDown();
        }
    }

    void KnockDown()
    {
        RandomizeRotation(transform, false);
        StartCoroutine(SetColor(deadColor));
        canDie = false;
    }
}
