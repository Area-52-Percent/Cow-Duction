/* UIManager.cs

   Controls UI elements for the Spaceship, including HUD, Parameter screen, and End screen.
   
   Assumptions:
     This component belongs to a GameObject named "UIManager" with the "UIManager" tag.
     All UI elements (Image, Slider, Text) and menu GameObjects exist in the scene and are referenced in the Inspector.
     There is a GameObject in the scene called "UFO" with Rigidbody and Mesh Renderer components.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Referenced objects
    [SerializeField] private Rigidbody _rbUFO;
    [SerializeField] private MeshRenderer[] ufoMesh;
    // Variables
    [SerializeField] private int score;
    public float scoreToFuelRatio = 10.0f;
    [SerializeField] private float fuel; // Percentage
    public float fuelDepletionRate = 1.0f;
    public float fuelWarnAmount = 25.0f;
    public float abilityActiveTime = 3.0f;
    [SerializeField] private float abilityCooldown; // Percentage
    [SerializeField] private bool cooldownActive;
    public float cooldownRegenerationRate = 5.0f;
    [SerializeField] private float timeRemaining; // Seconds
    [SerializeField] private float timeScaleFactor;
    // Gameplay UI Elements
    public Image hudDisplay;
    public Text scoreText;
    public Text speedText;
    public Text altitudeText;
    public Text timeText;
    public Text fuelWarnText;
    public Slider fuelMeter;
    public Image fuelMeterFill;
    public Slider cooldownMeter;
    public Image cooldownMeterFill;
    // EndScreen UI Elements
    public GameObject endScreen;
    public Text finalScoreText;
    // ParameterScreen UI Elements
    public GameObject parameterScreen;

    // Awake is called after all objects are initialized
    void Awake()
    {
        // Retrieve UFO rigidbody
        _rbUFO = GameObject.Find("UFO").GetComponent<Rigidbody>();
        ufoMesh = _rbUFO.GetComponentsInChildren<MeshRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize values for private variables
        score = 0;
        fuel = 100.0f;
        abilityCooldown = 100.0f;
        cooldownActive = false;
        timeRemaining = 240.0f;
        timeScaleFactor = 1.0f;
        // Deactivate non-gameplay menus
        endScreen.SetActive(false);
        parameterScreen.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Display speed and altitude
        speedText.text = _rbUFO.velocity.magnitude.ToString("F1");
        altitudeText.text = _rbUFO.transform.position.y.ToString("F1");
        
        // Update fuel meter display
        if (fuel > 0.0f)
        {
            if (!fuelWarnText.enabled && fuel < fuelWarnAmount)
                fuelWarnText.enabled = true;
            else if (fuel > fuelWarnAmount)
                fuelWarnText.enabled = false;
            fuel -= Time.fixedDeltaTime * fuelDepletionRate;
            fuelMeterFill.color = new Color(Mathf.Lerp(1f, 0f, fuel / 100f), Mathf.Lerp(0f, 1f, fuel / fuelWarnAmount), 0);
            fuelMeter.value = fuel;
        }
        else
        {
            fuel = 0.0f;
            fuelWarnText.text = "OUT OF FUEL";
            _rbUFO.GetComponent<SpaceshipMovement>().DisableMovement();
        }

        // Update cooldown meter display
        if (cooldownActive && abilityCooldown < 100.0f)
        {
            abilityCooldown += Time.fixedDeltaTime * cooldownRegenerationRate;
            cooldownMeter.value = abilityCooldown;
        }
        else
        {
            abilityCooldown = 100.0f;
            cooldownActive = false;
        }

        // Update time display
        if (timeRemaining > 0.0f && !endScreen.activeSelf)
        {
            timeRemaining -= Time.fixedDeltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60.0f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60.0f);
            timeText.text = minutes + ":" + seconds.ToString("D2");
        }
        else
        {
            timeRemaining = 0.0f;
            timeText.text = "0:00";
            DisplayEndScreen();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Activate ability
        if (Input.GetKeyDown(KeyCode.F) && !cooldownActive)
        {
            UseAbility();
        }
        // Toggle parameter screen
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleParameterScreen();
        }
    }

    // Increase score and fuel
    public void IncreaseScore(int amount)
    {
        score += amount;
        fuel += amount * scoreToFuelRatio;
        if (fuel > 100.0f)
            fuel = 100.0f;
        scoreText.text = score.ToString("D2");
    }

    // Put ability on cooldown and disable ufo mesh
    public void UseAbility()
    {
        abilityCooldown = 0.0f;        
        cooldownActive = true;        
        // Fade out HUD elements
        hudDisplay.CrossFadeAlpha(0f, abilityActiveTime / 10f, false);
        scoreText.enabled = false;
        speedText.enabled = false;
        altitudeText.enabled = false;
        timeText.enabled = false;
        // Disable UFO mesh
        foreach(MeshRenderer mr in ufoMesh)
        {
            mr.enabled = false;
        }

        StartCoroutine(EndAbility());
    }

    public IEnumerator EndAbility()
    {        
        yield return new WaitForSeconds(abilityActiveTime);
        
        // Fade in HUD elements
        hudDisplay.CrossFadeAlpha(1f, abilityActiveTime / 10f, false);
        scoreText.enabled = true;
        speedText.enabled = true;
        altitudeText.enabled = true;
        timeText.enabled = true;
        // Enable UFO mesh
        foreach(MeshRenderer mr in ufoMesh)
        {
            mr.enabled = true;
        }
    }

    // Show the endscreen
    public void DisplayEndScreen()
    {
        finalScoreText.text = "" + score;
        Time.timeScale = 0;
        endScreen.SetActive(true);
    }

    // Toggle the parameter screen
    public void ToggleParameterScreen()
    {
        if (parameterScreen.activeSelf)
        {
            Time.timeScale = timeScaleFactor;
            parameterScreen.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            parameterScreen.SetActive(true);
        }
    }

    public void SetParameter(Slider slider)
    {
        float value = slider.value;
        switch(slider.name)
        {            
            case "HorizontalSpeedSlider":
                _rbUFO.GetComponent<SpaceshipMovement>().horizontalSpeed = value;
                break;
            case "VerticalSpeedSlider":
                _rbUFO.GetComponent<SpaceshipMovement>().verticalSpeed = value;
                break;
            case "MaxHeightSlider":
                _rbUFO.GetComponent<SpaceshipMovement>().maxHeight = value;
                break;
            case "MinHeightSlider":
                _rbUFO.GetComponent<SpaceshipMovement>().minHeight = value;
                break;
            case "RotationForceSlider":
                _rbUFO.GetComponent<SpaceshipMovement>().rotationForce = value;
                break;
            case "MainCameraFovSlider": 
                Camera.main.fieldOfView = value;
                break;
            case "RadarCameraFovSlider":
                GameObject.Find("TopDownCamera").GetComponent<Camera>().fieldOfView = value;
                break;
            case "MaxRotationSlider":
                _rbUFO.GetComponent<SpaceshipMovement>().maxRotation = value;
                break;
            case "FuelDepletionSlider":
                fuelDepletionRate = value;
                break;
            case "CooldownRegenSlider":
                cooldownRegenerationRate = value;
                break;
            case "CaptureSpeedSlider":
                _rbUFO.GetComponent<CowAbduction>().captureSpeed = value;
                break;
            case "MaxCaptureLengthSlider":
                _rbUFO.GetComponent<CowAbduction>().maxCaptureLength = value;
                break;
            case "NumberOfJointsSlider":
                _rbUFO.GetComponent<CowAbduction>().numberOfJoints = (int)value;
                break;
            case "GrappleTimeSlider":
                _rbUFO.GetComponent<CowAbduction>().grappleTime = value;
                break;
            case "ScoreToFuelRatioSlider":
                scoreToFuelRatio = value;
                break;
            case "FuelWarnAmountSlider":
                fuelWarnAmount = value;
                break;
            case "AbilityActiveTimeSlider":
                abilityActiveTime = value;
                break;
            case "TimeScaleSlider":
                timeScaleFactor = value;
                break;
            default:
                break;
        }
    }

    // Reset gameplay variables to starting values
    public void ResetGame()
    {
        Start();
    }

    // Exit the game or editor play session
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
