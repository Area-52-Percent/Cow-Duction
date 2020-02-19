using UnityEngine;

public class CornRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(0f, Random.Range(-180f, 180f), 0f));
    }

    void OnTriggerEnter(Collider collider)
    {
        KnockDown();
    }

    void KnockDown()
    {
        transform.localEulerAngles = new Vector3(90f, Random.Range(-180f, 180f), 0);
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.material.SetColor("_BaseColor", new Color(.8f, .1f, .1f, .5f));
        }
    }
}
