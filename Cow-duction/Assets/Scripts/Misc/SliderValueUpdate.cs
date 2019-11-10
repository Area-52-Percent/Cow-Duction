/*  SliderValueUpdate.cs

    Updates text string to match slider value.
*/

using UnityEngine;
using UnityEngine.UI;

public class SliderValueUpdate : MonoBehaviour
{
    private Text myText;
    [SerializeField] private Slider mySlider;

    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<Text>();
    }

    // Update is called once per frame
    public void UpdateValue()
    {
        if(mySlider.wholeNumbers)
            myText.text = mySlider.value.ToString();
        else
            myText.text = mySlider.value.ToString("F2");
    }
}
