using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Referenced objects
    [SerializeField] private Rigidbody _rbUFO;
    // Raw data
    [SerializeField] private int score;
    public float scoreToFuelRatio = 10.0f;
    [SerializeField] private float fuel; // Percentage
    public float fuelDepletionRate = 1.0f;
    [SerializeField] private float abilityCooldown; // Percentage
    [SerializeField] private bool abilityReady;
    [SerializeField] private bool cooldownActive;
    public float cooldownRegenerationRate = 5.0f;
    [SerializeField] private float timeRemaining; // Seconds
    // Gameplay UI Elements
    public Text scoreText;
    public Text speedText;
    public Text altitudeText;
    public Text timeText;
    public Slider fuelMeter;
    public Slider cooldownMeter;
    // EndScreen UI Elements
    public GameObject endScreen;
    public Text finalScoreText;
    // ParameterScreen UI Elements
    public GameObject parameterScreen;

    // Awake is called after all objects are initialized
    void Awake()
    {
        _rbUFO = GameObject.Find("UFO").GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        fuel = 100.0f;
        abilityCooldown = 100.0f;
        abilityReady = true;
        cooldownActive = false;
        timeRemaining = 240.0f;
        endScreen.SetActive(false);
        parameterScreen.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        speedText.text = _rbUFO.velocity.magnitude.ToString("F1");
        altitudeText.text = _rbUFO.transform.position.y.ToString("F1");
        
        // Update fuel meter display
        if (fuel > 0.0f)
        {
            fuel -= Time.fixedDeltaTime * fuelDepletionRate;
            fuelMeter.value = fuel;
        }
        else
        {
            fuel = 0.0f;
            DisplayEndScreen();
        }

        // Activate ability (WIP)
        if (Input.GetKeyDown(KeyCode.F))
        {
            UseAbility();
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
            abilityReady = true;
            cooldownActive = false;
        }

        // Update time display
        if (timeRemaining > 0.0f)
        {
            timeRemaining -= Time.fixedDeltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60.0f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60.0f);
            timeText.text = minutes + ":" + seconds.ToString("D2");
        }
        else
        {
            timeRemaining = 0.0f;
            DisplayEndScreen();
        }

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

    // Put ability on cooldown (WIP)
    public void UseAbility()
    {
        abilityCooldown = 0.0f;
        abilityReady = false;        
        cooldownActive = true;
    }

    // Show the endscreen
    public void DisplayEndScreen()
    {
        finalScoreText.text = "" + score;
        endScreen.SetActive(true);
    }

    // Toggle the parameter screen
    public void ToggleParameterScreen()
    {
        if (parameterScreen.activeSelf)
        {
            Time.timeScale = 1;
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
            case "HorizontalSpeedSlider": // Horizontal Speed
                _rbUFO.GetComponent<SpaceshipMovement>().horizontalSpeed = value;
                break;
            case "VerticalSpeedSlider": // Vertical Speed
                _rbUFO.GetComponent<SpaceshipMovement>().verticalSpeed = value;
                break;
            case "RotationForceSlider": // Rotation Force
                _rbUFO.GetComponent<SpaceshipMovement>().rotationForce = value;
                break;
            case "MainCameraFovSlider": // Main Camera FOV
                Camera.main.fieldOfView = value;
                break;
            case "RadarCameraFovSlider": // Radar Camera FOV
                GameObject.Find("TopDownCamera").GetComponent<Camera>().fieldOfView = value;
                break;
            case "MaxRotationSlider": // Max Rotation
                _rbUFO.GetComponent<SpaceshipMovement>().maxRotation = value;
                break;
            case "FuelDepletionSlider": // Fuel Depletion Rate
                fuelDepletionRate = value;
                break;
            case "CooldownRegenSlider": // Cooldown Regen Rate
                cooldownRegenerationRate = value;
                break;
            case "CaptureSpeedSlider": // Cow Capture Speed
                _rbUFO.GetComponent<CowAbduction>().captureSpeed = value;
                break;
            case "ScoreToFuelRatioSlider": // Score to Fuel Ratio
                scoreToFuelRatio = value;
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
}
