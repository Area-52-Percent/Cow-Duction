using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterSetting : MonoBehaviour
{
    public Slider SpeedSlider;
    public Text SpeedText;
    public Slider SensitivitySlider;
    public Text SensitivityText;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Speed"))
        {
            SpeedSlider.value = PlayerPrefs.GetFloat("Speed");
            SpeedText.text = PlayerPrefs.GetFloat("Speed").ToString();
        }
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            SensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
            SensitivityText.text = PlayerPrefs.GetFloat("Sensitivity").ToString();
        }
    }
    public void SetCows(float f)
    {
        PlayerPrefs.SetFloat("CowSpawnRate", f);
    }
    public void SetSpeed(float f)
    {
        PlayerPrefs.SetFloat("Speed", f);
    }
    public void SetAbductSpeed(float f)
    {
        PlayerPrefs.SetFloat("AbductionSpeed", f);
    }
    public void SetSens(float f)
    {
        PlayerPrefs.SetFloat("Sensitivity", f);
    }
    public void SetFocalLength(float f)
    {
        PlayerPrefs.SetFloat("FocalLength", f);
    }
    public void SetFuel(float f)
    {
        PlayerPrefs.SetFloat("FuelDepletion", f);
    }
    public void SetCow2MilkRatio(float f)
    {
        PlayerPrefs.SetFloat("CowToMilkRatio", f);
    }
}
