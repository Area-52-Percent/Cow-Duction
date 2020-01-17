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

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        rend = GetComponent<Renderer>();
        Debug.Log(rend);
        initColor = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Debug.Log("in if");
            if (hit.collider)
            {
                image.color = Color.green;
            }
        }
        else
        {
            image.color = Color.white;
        }
    }
}
