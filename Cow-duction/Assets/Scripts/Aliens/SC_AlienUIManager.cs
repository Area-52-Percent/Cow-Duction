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
    // Private variables
    private SC_HudReticleFollowCursor reticleFollowCursor;
    private TransformWrapper transformWrapper;
    private Rigidbody _rbUFO;
    private MeshRenderer[] ufoMesh;
    private AudioSource ufoAudioSource;
    private int score;
    private float fuel; // Percentage of 100
    private float fuelDepletionRate = 1.0f;
    private float fuelWarnAmount = 25.0f;
    private float abilityActiveTime = 3.0f;
    private float abilityCooldown; // Percentage of 100
    private float cooldownRegenerationRate = 5.0f;
    private bool cooldownActive;
    private float timeRemaining; // In seconds
    private float timeScaleFactor;

    // Public variables
    public GameObject CowSpawner;

    // Serialized private variables
    [SerializeField] private Camera topDownCamera = null; // Set up in inspector
    [SerializeField] private Color fuelStartColor = Color.white; // (Optional) Set up in inspector
    [SerializeField] private Color fuelDepletedColor = Color.red; // (Optional) Set up in inspector
    [SerializeField] private AudioClip activateAbility = null; // Set up in inspector
    [SerializeField] private Image hudDisplay = null; // Set up in inspector
    [SerializeField] private Slider milkSlide = null; // Set up in inspector
    [Space] // Reticle icon
    [SerializeField] private Image reticle = null; // Set up in inspector
    [SerializeField] private Sprite normalReticle = null; // Set up in inspector
    [SerializeField] private Sprite disabledReticle = null; // Set up in inspector
    [Space] // Flight status
    [SerializeField] private Text scoreText = null; // Set up in inspector
    [SerializeField] private Text speedText = null; // Set up in inspector
    [SerializeField] private Text altitudeText = null; // Set up in inspector
    [SerializeField] private Text timeText = null; // Set up in inspector
    [SerializeField] private Text fuelWarnText = null; // Set up in inspector
    [SerializeField] private Slider fuelMeter = null; // Set up in inspector
    [SerializeField] private Image fuelMeterFill = null; // Set up in inspector
    [SerializeField] private Slider cooldownMeter = null; // Set up in inspector
    [SerializeField] private Text cooldownReadyText = null; // Set up in inspector
    [Space] // Non Gameplay UI Elements
    [SerializeField] private GameObject endScreen = null; // Set up in inspector
    [SerializeField] private Text finalScoreText = null; // Set up in inspector
    [SerializeField] private GameObject helpScreen = null; // Set up in inspector
    [SerializeField] private GameObject parameterScreen = null; // Set up in inspector
    [SerializeField] private GameObject holoCow = null; // Intro cow hologram
    [SerializeField] private GameObject holoFarmer = null; // Intro farmer hologram
    [SerializeField] private float introTime = 45.0f;

    // Awake is called after all objects are initialized
    void Awake()
    {
        _rbUFO = GameObject.Find("UFO").GetComponent<Rigidbody>();
        ufoMesh = _rbUFO.GetComponentsInChildren<MeshRenderer>();
        ufoAudioSource = _rbUFO.GetComponent<AudioSource>();
        transformWrapper = _rbUFO.GetComponent<TransformWrapper>();
        reticleFollowCursor = reticle.GetComponent<SC_HudReticleFollowCursor>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize values for private variables
        score = 0;
        scoreText.text = score.ToString("D2");
        reticle.sprite = normalReticle;
        fuel = 100.0f;
        abilityCooldown = 100.0f;
        cooldownActive = false;
        timeRemaining = 240.0f;
        if (timeScaleFactor < Mathf.Epsilon)
            timeScaleFactor = 1.0f;
        Time.timeScale = timeScaleFactor;

        // Deactivate non-gameplay menus
        endScreen.SetActive(false);
        parameterScreen.SetActive(false);
        helpScreen.SetActive(false);

        StartCoroutine(PlayIntro());
    }

    // Update is called once per frame
    void Update()
    {
        // Display speed and altitude
        speedText.text = _rbUFO.velocity.magnitude.ToString("F1");
        altitudeText.text = _rbUFO.transform.position.y.ToString("F1");
        
        // Update fuel
        if (fuel > 0.0f)
        {
            if (!fuelWarnText.enabled && fuel < fuelWarnAmount)
            {
                fuelWarnText.text = "LOW";
                fuelWarnText.enabled = true;
            }
            else if (fuel > fuelWarnAmount)
                fuelWarnText.enabled = false;
            
            fuel -= Time.deltaTime * fuelDepletionRate;
            fuelMeterFill.color = Color.Lerp(fuelDepletedColor, fuelStartColor, fuel / 100f);
            fuelMeter.value = fuel;
        }
        else
        {
            fuel = 0.0f;
            fuelWarnText.text = "OUT";
            _rbUFO.GetComponent<SC_SpaceshipMovement>().AllowMovement(false);
        }

        // Update ability cooldown
        if (cooldownActive && abilityCooldown < 100.0f)
        {
            abilityCooldown += Time.deltaTime * cooldownRegenerationRate;
            cooldownMeter.value = abilityCooldown;
        }
        else
        {
            abilityCooldown = 100.0f;
            cooldownActive = false;
            if (cooldownReadyText)
                cooldownReadyText.enabled = true;
        }

        // Update time (Unscaled)
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

        // Activate ability
        if (Input.GetButtonDown("Ability") && !cooldownActive)
        {
            StartCoroutine(UseAbility());
        }

        // Toggle parameter screen
        if (Input.GetButtonDown("Cancel"))
        {
            ToggleParameterScreen();
        }
    }

    // Swap reticle sprites
    public void ToggleReticle()
    {
        if (reticle.sprite == normalReticle)
            reticle.sprite = disabledReticle;
        else
            reticle.sprite = normalReticle;
    }

    // Animate cow icon downwards
    private IEnumerator AnimateIncreaseScore()
    {
        milkSlide.direction = Slider.Direction.TopToBottom;
        while (milkSlide.value < 1.0f)
        {
            milkSlide.value += Time.deltaTime;
            yield return null;
        }
        milkSlide.direction = Slider.Direction.BottomToTop;
        while (milkSlide.value > 0.0f)
        {
            milkSlide.value -= Time.deltaTime;
            yield return null;
        }
    }

    // Increase score and fuel
    public void IncreaseScore(float milk)
    {
        StartCoroutine(AnimateIncreaseScore());

        score += 1;
        scoreText.text = score.ToString("D2");
        
        if (fuel <= 0.0f)
            fuelWarnText.enabled = false;

        fuel += milk;
        if (fuel > 100.0f)
            fuel = 100.0f;
        
        if (fuel > 0.0f)
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

        ufoAudioSource.PlayOneShot(activateAbility, 0.5f);

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
        
        ufoAudioSource.PlayOneShot(activateAbility, 0.3f);

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
        StartCoroutine(AnimateDamage());

        fuel -= amount;
        fuelMeter.value = fuel;
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

    private IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(introTime);

        holoCow.SetActive(false);
        holoFarmer.SetActive(false);
    }

    // Show the endscreen (TO-DO: Replace hard-coded values)
    public void DisplayEndScreen()
    {
        string rating = "";

        if (score > 27)
            rating = "SS";
        else if (score > 19)
            rating = "S";
        else if (score > 15)
            rating = "A";
        else if (score > 13)
            rating = "B";
        else if (score > 11)
            rating = "C";
        else if (score > 9)
            rating = "D";
        else
            rating = "F";
        
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

    // Sets parameter based on input slider name and value
    public void SetParameter(Slider slider)
    {
        float value = slider.value;

        SetParameter(slider.name, value);
    }

    // Set parameter using input name and value
    public void SetParameter(string name, float value)
    {
        switch(name)
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
            case "ReticleAimSensitivitySlider":
                reticleFollowCursor.joystickSensitivity = value;
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

    public int GetScore()
    {
        return score;
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
