using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Referenced objects
    [SerializeField] private Rigidbody _rbUFO;
<<<<<<< Updated upstream
    // Raw data
    [SerializeField] private int score = 0;
=======
    [SerializeField] private MeshRenderer[] ufoMesh;
    [SerializeField] private GameObject CowSpawner;
    // Variables
    [SerializeField] private int score;
    public float scoreToFuelRatio = 10.0f;
>>>>>>> Stashed changes
    [SerializeField] private float fuel; // Percentage
    [SerializeField] private float abilityCooldown; // Percentage
    [SerializeField] private float ufoSpeed;
    [SerializeField] private float ufoAltitude;
    [SerializeField] private float timeRemaining; // Seconds
    // UI Elements
    public Text scoreText;
    public Text speedText;
    public Text altitudeText;
    public Text timeText;

    // Start is called before the first frame update
    void Start()
    {
        _rbUFO = GameObject.Find("UFO").GetComponent<Rigidbody>();
        fuel = 100.0f;
        abilityCooldown = 100.0f;
        timeRemaining = 240.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        speedText.text = _rbUFO.velocity.magnitude.ToString("F1");
        altitudeText.text = _rbUFO.transform.position.y.ToString("F1");
        
        if (timeRemaining > 0.0f)
        {
            timeRemaining -= Time.fixedDeltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60.0f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60.0f);
            timeText.text = minutes + ":" + seconds;
        }
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString("D2");
<<<<<<< Updated upstream
=======
        if (_rbUFO.GetComponent<SpaceshipMovement>())
            _rbUFO.GetComponent<SpaceshipMovement>().AllowMovement(true);
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
        endScreen.SetActive(true);
    }

    // Toggle the help screen
    public void ToggleHelpScreen()
    {
        if (helpScreen.activeSelf)
        {
            helpScreen.SetActive(false);
        }
        else
        {
            helpScreen.SetActive(true);
        }
    }

    // Toggle the parameter screen
    public void ToggleParameterScreen()
    {
        if (parameterScreen.activeSelf)
        {
            parameterScreen.SetActive(false);
        }
        else
        {
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
            case "MaxCowAmountSlider":
                CowSpawner.GetComponent<CowSpawner>().maxCowAmount = (int)value;
                break;
            case "CowSpawnRateSlider":
                CowSpawner.GetComponent<CowSpawner>().spawnRate = (int)value;
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
>>>>>>> Stashed changes
    }
}
