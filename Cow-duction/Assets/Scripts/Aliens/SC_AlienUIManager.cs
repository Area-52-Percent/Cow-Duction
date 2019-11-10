/* SC_AlienUIManager.cs

   Controls UI elements for the Spaceship, including HUD, Parameter screen, and End screen.
   
   Assumptions:
     This component belongs to a GameObject named "UIManager" with the "UIManager" tag.
     All UI elements (Image, Slider, Text) and menu GameObjects exist in the scene and are referenced in the Inspector.
     There is a GameObject in the scene called "UFO" with Rigidbody and Mesh Renderer components.
     There is a GameObject in the scene called "CowSpawner" with the SC_CowSpawner component. It assigns itself as CowSpawner.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SC_AlienUIManager : MonoBehaviour
{
    // Referenced objects
    private TransformWrapper transformWrapper;
    private Rigidbody _rbUFO;
    private MeshRenderer[] ufoMesh;
    [SerializeField] private Camera topDownCamera = null; // Set up in inspector
    public GameObject CowSpawner;
    // Variables
    private int score;
    public float scoreToFuelRatio = 10.0f;
    private float fuel; // Percentage
    private float fuelDepletionRate = 1.0f;
    private float fuelWarnAmount = 25.0f;
    [SerializeField] private Color fuelStartColor = Color.white;
    [SerializeField] private Color fuelDepletedColor = Color.red;
    private float abilityActiveTime = 3.0f;
    private float abilityCooldown; // Percentage
    private bool cooldownActive;
    public float cooldownRegenerationRate = 5.0f;
    private float timeRemaining; // Seconds
    private float timeScaleFactor;
    // Gameplay UI Elements
    [SerializeField] private Image hudDisplay = null; // Set up in inspector
    public Image cowIcon;
    [SerializeField] private Text scoreText = null; // Set up in inspector
    [SerializeField] private Text speedText = null; // Set up in inspector
    [SerializeField] private Text altitudeText = null; // Set up in inspector
    [SerializeField] private Text timeText = null; // Set up in inspector
    [SerializeField] private Text fuelWarnText = null; // Set up in inspector
    [SerializeField] private Slider fuelMeter = null; // Set up in inspector
    [SerializeField] private Image fuelMeterFill = null; // Set up in inspector
    [SerializeField] private Slider cooldownMeter = null; // Set up in inspector
    [SerializeField] private Image cooldownMeterFill;
    [SerializeField] private Text cooldownReadyText = null; // Set up in inspector
    // Non Gameplay UI Elements
    [SerializeField] private GameObject endScreen = null; // Set up in inspector
    [SerializeField] private Text finalScoreText = null; // Set up in inspector
    [SerializeField] private GameObject helpScreen = null; // Set up in inspector
    [SerializeField] private GameObject parameterScreen = null; // Set up in inspector

    // Awake is called after all objects are initialized
    void Awake()
    {        
        // Retrieve UFO rigidbody
        _rbUFO = GameObject.Find("UFO").GetComponent<Rigidbody>();
        ufoMesh = _rbUFO.GetComponentsInChildren<MeshRenderer>();
        transformWrapper = _rbUFO.GetComponent<TransformWrapper>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize values for private variables
        score = 0;
        scoreText.text = score.ToString("D2");
        if (cowIcon)
            cowIcon.enabled = false;
        fuel = 100.0f;
        abilityCooldown = 100.0f;
        cooldownActive = false;
        timeRemaining = 240.0f;
        timeScaleFactor = 1.0f;
        // Deactivate non-gameplay menus
        endScreen.SetActive(false);
        parameterScreen.SetActive(false);
        helpScreen.SetActive(false);
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
            {
                fuelWarnText.text = "LOW";
                fuelWarnText.enabled = true;
            }
            else if (fuel > fuelWarnAmount)
                fuelWarnText.enabled = false;
            fuel -= Time.fixedDeltaTime * fuelDepletionRate;
            fuelMeterFill.color = Color.Lerp(fuelDepletedColor, fuelStartColor, fuel / 100f);
            fuelMeter.value = fuel;
        }
        else
        {
            fuel = 0.0f;
            fuelWarnText.text = "OUT";
            _rbUFO.GetComponent<SC_SpaceshipMovement>().AllowMovement(false);
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
            if (cooldownReadyText)
                cooldownReadyText.enabled = true;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        // Activate ability
        if (Input.GetKeyDown(KeyCode.F) && !cooldownActive)
        {
            StartCoroutine(UseAbility());
        }
        // Toggle parameter screen
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleParameterScreen();
        }

        // Update time display (Unscaled)
        if (timeRemaining > 0.0f && !endScreen.activeSelf)
        {
            timeRemaining -= Time.unscaledDeltaTime;
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

    // Enable or disable cow icon
    public void ToggleCowIcon()
    {
        if (cowIcon.enabled)
            cowIcon.enabled = false;
        else
            cowIcon.enabled = true;
    }

    // Add visual flair to score increase (TO DO: Replace hard-coded values)
    private IEnumerator AnimateIncreaseScore()
    {
        RectTransform cowIconTransform;
        if (cowIconTransform = cowIcon.GetComponent<RectTransform>())
        {
            while (cowIconTransform.anchoredPosition.y > 20)
            {
                cowIconTransform.anchoredPosition += Vector2.down * 5;
                yield return null;
            }
            ToggleCowIcon();
            cowIconTransform.anchoredPosition = Vector2.up * 80;
        }
    }

    // Increase score and fuel
    public void IncreaseScore(int amount)
    {
        StartCoroutine(AnimateIncreaseScore());
        score += amount;
        if (fuel <= 0.0f)
            fuelWarnText.enabled = false;
        fuel += amount * scoreToFuelRatio;
        if (fuel > 100.0f)
            fuel = 100.0f;
        scoreText.text = score.ToString("D2");
        if (_rbUFO.GetComponent<SC_SpaceshipMovement>())
            _rbUFO.GetComponent<SC_SpaceshipMovement>().AllowMovement(true);
    }  

    // Fade out HUD and disable mesh for <abilityActiveTime> seconds then fade HUD back in and re-enable mesh
    public IEnumerator UseAbility()
    {        
        abilityCooldown = 0.0f;
        topDownCamera.enabled = false;
        cooldownActive = true;
        if (cooldownReadyText)
            cooldownReadyText.enabled = false;
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

        yield return new WaitForSeconds(abilityActiveTime);
        
        // Fade in HUD elements
        topDownCamera.enabled = true;
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

    // Subtract fuel by amount of damage taken
    public void TakeDamage(float amount)
    {
        fuel -= amount;
        fuelMeter.value = fuel;
        StartCoroutine(AnimateDamage());
    }

    // Visual indicator of taking damage
    private IEnumerator AnimateDamage()
    {
        foreach (Image image in fuelMeter.GetComponentsInChildren<Image>())
        {
            image.color = Color.Lerp(Color.red, Color.white, Time.deltaTime);
        }
        yield return new WaitForEndOfFrame();
        foreach (Image image in fuelMeter.GetComponentsInChildren<Image>())
        {
            image.color = Color.Lerp(Color.white, Color.red, Time.deltaTime);
        }
    }

    // Show the endscreen (TO-DO: Replace hard-coded values)
    public void DisplayEndScreen()
    {
        string rating = "";
        if (score < 10)
            rating = "F";
        else if (score < 13)
            rating = "C";
        else if (score < 16)
            rating = "B";
        else if (score < 20)
            rating = "A";
        else if (score < 25)
            rating = "S";
        else
            rating = "SS";
        finalScoreText.text = score + "\n\nRating: " + rating;
        Time.timeScale = Mathf.Epsilon;
        endScreen.SetActive(true);
    }

    // Toggle the help screen
    public void ToggleHelpScreen()
    {
        if (helpScreen.activeSelf)
            helpScreen.SetActive(false);
        else
            helpScreen.SetActive(true);
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
            Time.timeScale = Mathf.Epsilon;
            parameterScreen.SetActive(true);
        }
    }

    // Sets parameter based on input slider name
    public void SetParameter(Slider slider)
    {
        float value = slider.value;
        switch(slider.name)
        {            
            case "HorizontalSpeedSlider":
                _rbUFO.GetComponent<SC_SpaceshipMovement>().horizontalSpeed = value;
                break;
            case "VerticalSpeedSlider":
                _rbUFO.GetComponent<SC_SpaceshipMovement>().verticalSpeed = value;
                break;
            case "MaxHeightSlider":
                _rbUFO.GetComponent<SC_SpaceshipMovement>().maxHeight = value;
                break;
            case "MinHeightSlider":
                _rbUFO.GetComponent<SC_SpaceshipMovement>().minHeight = value;
                break;
            case "RotationForceSlider":
                _rbUFO.GetComponent<SC_SpaceshipMovement>().rotationForce = value;
                break;
            case "MainCameraFovSlider": 
                Camera.main.fieldOfView = value;
                break;
            case "RadarCameraFovSlider":
                GameObject.Find("TopDownCamera").GetComponent<Camera>().fieldOfView = value;
                break;
            case "MaxRotationSlider":
                _rbUFO.GetComponent<SC_SpaceshipMovement>().maxRotation = value;
                break;
            case "FuelDepletionSlider":
                fuelDepletionRate = value;
                break;
            case "CooldownRegenSlider":
                cooldownRegenerationRate = value;
                break;
            case "CaptureSpeedSlider":
                _rbUFO.GetComponent<SC_CowAbduction>().captureSpeed = value;
                break;
            case "MaxCaptureLengthSlider":
                _rbUFO.GetComponent<SC_CowAbduction>().maxCaptureLength = value;
                break;
            case "NumberOfJointsSlider":
                _rbUFO.GetComponent<SC_CowAbduction>().numberOfJoints = (int)value;
                break;
            case "GrappleTimeSlider":
                _rbUFO.GetComponent<SC_CowAbduction>().grappleTime = value;
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
            case "MaxCowAmountSlider":
                CowSpawner.GetComponent<SC_CowSpawner>().maxCowAmount = (int)value;
                break;
            case "CowSpawnRateSlider":
                CowSpawner.GetComponent<SC_CowSpawner>().spawnRate = (int)value;
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
