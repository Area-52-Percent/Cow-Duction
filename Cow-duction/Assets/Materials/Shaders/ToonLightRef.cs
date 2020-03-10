using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonLightRef : MonoBehaviour
{
    private Light Toonlight = null;

    private void OnEnable()
    {
        Toonlight = this.GetComponent<Light>();
    }

    private void Awake()
    {
        Shader.SetGlobalVector("_ToonLightDirection", -this.transform.forward);
    }
}
