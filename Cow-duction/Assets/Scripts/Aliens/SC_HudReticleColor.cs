using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SC_HudReticleColor : MonoBehaviour
{
    private Color color;
    private Color initColor;
    private Image image;
    public Renderer rend;
    public Camera cam;
    public Transform reticle;
    public Transform grappleOrigin;
    private float viewLength;
    private float maxViewLength;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        rend = GetComponent<Renderer>();
        Debug.Log(rend);
        initColor = image.color;
        maxViewLength = 50;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 reticlePoint = RectTransformUtility.WorldToScreenPoint(null, reticle.GetComponent<RectTransform>().position);
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(reticlePoint);

        int layerMask = ~(1 << gameObject.layer);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
        {
            viewLength = Vector3.Distance(grappleOrigin.position, hit.transform.position);
            Vector3 hitPoint = hit.point;
            if (hit.distance < maxViewLength)
            {
                if (hit.collider)
                {
                    Rigidbody ob = hit.collider.attachedRigidbody;
                    if (ob != null)
                    {
                        if (ob.CompareTag("Cow"))
                        {
                            image.color = Color.green;
                        }
                        else if (ob.CompareTag("Farmer"))
                        {
                            image.color = Color.red;
                        }
                        else
                        {
                            image.color = Color.white;
                        }
                    }
                    else
                    {
                        image.color = Color.white;
                    }
                }
                else
                {
                    image.color = Color.white;
                }
            }
        }
        else
        {
            image.color = Color.white;
        }
    }
}
