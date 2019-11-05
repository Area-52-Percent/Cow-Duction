using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    public Slider _Slider;
    public Text _Text;
    // Start is called before the first frame update
    void Start()
    {
        _Text.text = _Slider.value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        _Text.text = _Slider.value.ToString();
    }
}
