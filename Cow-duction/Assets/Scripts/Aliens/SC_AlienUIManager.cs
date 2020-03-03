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
    private GameManager gameManager;
    private Camera mainCamera;
    public Animator shipAnim;
    private SC_HudReticleFollowCursor reticleFollowCursor;
    private TransformWrapper transformWrapper;
    [SerializeField] private Rigidbody _rbUFO;
    private MeshRenderer[] ufoMesh;
    private AudioSource ufoAudioSource;
    private int score;
    private bool inScoreMultiplier = false;
    private int scoreMultiplier = 1;
    private float fuel; // Percentage of 100
    private float fuelDepletionRate = 0.5f;
    private float fuelWarnAmount = 25.0f;
    private float abilityActiveTime = 3.0f;
    private float abilityCooldown; // Percentage of 100
    private float cooldownRegenerationRate = 5.0f;
    private Coroutine co;
    private bool cooldownActive;
    private float timeRemaining; // In seconds
    private float timeScaleFactor;
    private bool paused;
    private bool effected;

    // Public variables
    public GameObject CowSpawner;
    public float playTime = 270.0f;
    public float effectCD = 2f;

    // Serialized private variables

    [SerializeField] private Camera topDownCamera = null; // Set up in inspector
    [SerializeField] private Color fuelStartColor = Color.white; // (Optional) Set up in inspector
    [SerializeField] private Color fuelDepletedColor = Color.red; // (Optional) Set up in inspector
    [SerializeField] private AudioClip activateAbility = null; // Set up in inspector
    [SerializeField] private Image hudDisplay = null; // Set up in inspector
    [SerializeField] private Slider milkSlide = null; // Set up in inspector
    [SerializeField] private Image cropSplatter = null; // Set up in inspector
    [Space] // Reticle icon
    [SerializeField] private Image reticle = null; // Set up in inspector
    [SerializeField] private Sprite normalReticle = null; // Set up in inspector
    [SerializeField] private Sprite disabledReticle = null; // Set up in inspector
    [SerializeField] private Image waypointIcon = null;
    [Space] // Flight status
    [SerializeField] private Text scoreText = null; // Set up in inspector
    [SerializeField] private Text mulitiplierText = null; // Set up in inspector
    [SerializeField] private Text speedText = null; // Set up in inspector
    [SerializeField] private Text altitudeText = null; // Set up in inspector
    [SerializeField] private Text timeText = null; // Set up in inspector
    [SerializeField] private Text achievementText = null; // Set up in inspector
    [SerializeField] private Text fuelWarnText = null; // Set up in inspector
    [SerializeField] private Slider fuelMeter = null; // Set up in inspector
    [SerializeField] private Image fuelMeterFill = null; // Set up in inspector
    [SerializeField] private Slider cooldownMeter = null; // Set up in inspector
    [SerializeField] private Image cooldownMeterFill = null; // Set up in inspector
    [SerializeField] private Text cooldownReadyText = null; // Set up in inspector
    [Space] // Screens
    [SerializeField] private GameObject gameplayScreen = null; // Set up in inspector
    [SerializeField] private GameObject endScreen = null; // Set up in inspector
    [SerializeField] private GameObject helpScreen = null; // Set up in inspector
    [SerializeField] private GameObject parameterScreen = null; // Set up in inspector
    [Space] // Intro
    [SerializeField] private GameObject holoCow = null; // Intro cow hologram
    [SerializeField] private GameObject holoFarmer = null; // Intro farmer hologram
    [SerializeField] private Text milkText = null;
    [SerializeField] private Image controllerScreen = null; // Intro controls
    [SerializeField] private bool playIntro = true;
    private bool playingIntro = false;
    [Space] // Outro
    [SerializeField] private AudioClip twoMinuteWarning = null; // Set up in inspector
    [SerializeField] private AudioClip thirtySecondWarning = null; // Set up in inspector
    [SerializeField] private AudioClip loseAudio = null; // Set up in inspector
    [SerializeField] private AudioClip winAudio = null; // Set up in inspector
    [SerializeField] private Text finalScoreText = null; // Set up in inspector

    public bool GetPaused()
    {
        return paused;
    }

    public int GetScore()
    {
        return score;
    }

    // Awake is called after all objects are initialized
    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        mainCamera = Camera.main;
        ufoMesh = _rbUFO.GetComponentsInChildren<MeshRenderer>();
        ufoAudioSource = _rbUFO.GetComponent<AudioSource>();
        transformWrapper = _rbUFO.GetComponent<TransformWrapper>();
        reticleFollowCursor = reticle.GetComponent<SC_HudReticleFollowCursor>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetGameStarting())
            return;

        // Toggle parameter screen
        if (Input.GetButtonDown("Cancel"))
        {
            ToggleParameterScreen();
        }

        if (!gameManager.GetGameStarted() && !gameplayScreen.activeSelf)
            return;

        if (playingIntro && Input.GetButtonDown("Skip"))
        {
            StopCoroutine(PlayIntro());
            SkipIntro();
        }

        // Display speed and altitude
        speedText.text = _rbUFO.velocity.magnitude.ToString("F1");
        altitudeText.text = _rbUFO.transform.position.y.ToString("F1");

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

            // Activate ability
            if (Input.GetButtonDown("Ability"))
            {
                StartCoroutine(UseAbility());
            }
        }

        if (timeRemaining < Mathf.Infinity)
        {
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

            // Update time (Unscaled)
            if (timeRemaining > 0.0f)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60.0f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60.0f);
                timeText.text = minutes + ":" + seconds.ToString("D2");

                if (minutes < 1 && seconds <= 30)
                {
                    if (timeText.color != Color.red)
                    {
                        timeText.color = Color.red;
                        ufoAudioSource.PlayOneShot(thirtySecondWarning);
                        gameManager.SetMusicVolume(0.1f);
                    }
                    else if (!ufoAudioSource.isPlaying)
                        gameManager.SetMusicVolume(0.25f);
                }
                else if (minutes < 2)
                {
                    if (timeText.color != Color.yellow)
                    {
                        timeText.color = Color.yellow;
                        ufoAudioSource.PlayOneShot(twoMinuteWarning);
                        gameManager.SetMusicVolume(0.1f);
                    }
                    else if (!ufoAudioSource.isPlaying)
                        gameManager.SetMusicVolume(0.25f);
                }
                else if (timeText.color != Color.white)
                    timeText.color = Color.white;

                timeRemaining -= Time.unscaledDeltaTime;
            }
            else if (!endScreen.activeSelf)
            {
                timeRemaining = 0.0f;
                timeText.text = "0:00";
                DisplayEndScreen();
            }
        }
        else
        {
            timeText.text = "--:--";
            if (fuelWarnText.enabled)
                fuelWarnText.enabled = false;
        }
    }

    // Toggle reticle size
    public void ToggleReticle()
    {
        if (reticle.rectTransform.localScale == Vector3.one)
            reticle.rectTransform.localScale = Vector3.one * 0.5f;
        else
            reticle.rectTransform.localScale = Vector3.one;
    }

    // Set sprite of waypoint icon
    public void SetWaypointIcon(Sprite icon)
    {
        waypointIcon.sprite = icon;
    }

    // Animate milk sliding effect
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

    private IEnumerator RunScoreMultiplier(int CowScore)
    {
        score = score + (scoreMultiplier * CowScore);
        scoreMultiplier = scoreMultiplier + 1;
        mulitiplierText.text = scoreMultiplier.ToString("D1");
        scoreText.text = score.ToString("D2");
        yield return new WaitForSeconds(10);
        inScoreMultiplier = false;
        scoreMultiplier = 1;
        mulitiplierText.text = scoreMultiplier.ToString("D1");
    }

    // Increase score and fuel
    public void IncreaseScore(float milk, GameObject cow)
    {
        StartCoroutine(AnimateIncreaseScore());
        int cowScore = 1;
        if (cow.GetComponent<SC_CowBrain>().cowType == "Normal")
        {
            cowScore = 1;
        }
        else if (cow.GetComponent<SC_CowBrain>().cowType == "Chocolate Cow")
        {
            cowScore = 2;
        }
        else if (cow.GetComponent<SC_CowBrain>().cowType == "Strawberry Cow")
        {
            cowScore = 3;
        }
        if (inScoreMultiplier)
        {
            StopCoroutine(co);
            co = StartCoroutine(RunScoreMultiplier(cowScore));
        }
        else
        {
            co = StartCoroutine(RunScoreMultiplier(cowScore));
            inScoreMultiplier = true;
        }

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
        float cloakedOpacity = 0.25f;

        abilityCooldown = 0.0f;
        cooldownActive = true;

        if (cooldownReadyText)
            cooldownReadyText.enabled = false;

        ufoAudioSource.PlayOneShot(activateAbility, 0.5f);

        // Fade out HUD 
        topDownCamera.cullingMask = (1 << LayerMask.NameToLayer("Radar"));
        hudDisplay.CrossFadeAlpha(cloakedOpacity, abilityActiveTime / 10f, false);
        scoreText.CrossFadeAlpha(cloakedOpacity, abilityActiveTime / 10f, false);
        speedText.CrossFadeAlpha(cloakedOpacity, abilityActiveTime / 10f, false);
        altitudeText.CrossFadeAlpha(cloakedOpacity, abilityActiveTime / 10f, false);
        timeText.CrossFadeAlpha(cloakedOpacity, abilityActiveTime / 10f, false);

        // Disable UFO mesh
        foreach (MeshRenderer mr in ufoMesh)
        {
            mr.enabled = false;
        }

        yield return new WaitForSeconds(abilityActiveTime);

        ufoAudioSource.PlayOneShot(activateAbility, 0.3f);

        // Fade in HUD elements
        topDownCamera.cullingMask = ~(1 << LayerMask.NameToLayer("Cow"));
        hudDisplay.CrossFadeAlpha(1f, abilityActiveTime / 10f, false);
        scoreText.CrossFadeAlpha(1f, abilityActiveTime / 10f, false);
        speedText.CrossFadeAlpha(1f, abilityActiveTime / 10f, false);
        altitudeText.CrossFadeAlpha(1f, abilityActiveTime / 10f, false);
        timeText.CrossFadeAlpha(1f, abilityActiveTime / 10f, false);

        // Enable UFO mesh
        foreach (MeshRenderer mr in ufoMesh)
        {
            mr.enabled = true;
        }
    }

    // Subtract fuel by amount of damage taken
    public void TakeDamage(float amount,char type /*, Vector3 position*/)
    {
        //StartCoroutine(AnimateDamage(position));
        if(type == 'p')
            StartCoroutine(PotatoEffect());
        if (type == 'r')
            StartCoroutine(CarrotEffect());
        if (type == 'c')
            StartCoroutine(CornEffect());
        fuel -= amount;
        fuelMeter.value = fuel;
    }
    public IEnumerator CarrotEffect()
    {
        effected = true;
        float curFuelRate = fuelDepletionRate;
        fuelDepletionRate = fuelDepletionRate * 2;
        yield return new WaitForSeconds(effectCD);
        fuelDepletionRate = curFuelRate;
        effected = false;
    }

    public IEnumerator PotatoEffect()
    {
        effected = true;
        cropSplatter.gameObject.SetActive(true);
        yield return new WaitForSeconds(effectCD);
        cropSplatter.gameObject.SetActive(false);
        effected = false;
    }

    public IEnumerator CornEffect()
    {
        effected = true;
        _rbUFO.GetComponent<SC_SpaceshipMovement>().AddImpulseForce(new Vector3(0, -1f, 0), 250f);
        yield return new WaitForSeconds(effectCD);
        effected = false;
    }
    // Visual indicator of taking damage
    private IEnumerator AnimateDamage(Vector3 position)
    {
        RectTransform splatterRectTransform = cropSplatter.rectTransform;
        Vector3 splatterPosition = RectTransformUtility.WorldToScreenPoint(mainCamera, position);
        if (splatterPosition.x < 0)
            splatterPosition.x = 0;
        else if (splatterPosition.x > Screen.width)
            splatterPosition.x = Screen.width;
        if (splatterPosition.y < 0)
            splatterPosition.y = 0;
        else if (splatterPosition.y > Screen.height)
            splatterPosition.y = Screen.height;

        cropSplatter.rectTransform.position = splatterPosition;

        cropSplatter.CrossFadeAlpha(1f, 0f, true);
        if (!cropSplatter.gameObject.activeSelf)
            cropSplatter.gameObject.SetActive(true);

        foreach (Image image in fuelMeter.GetComponentsInChildren<Image>())
        {
            image.color = Color.Lerp(Color.red, Color.white, Time.deltaTime);
        }

        yield return new WaitForSeconds(0.2f);

        foreach (Image image in fuelMeter.GetComponentsInChildren<Image>())
        {
            image.color = Color.Lerp(Color.white, Color.red, Time.deltaTime);
        }
        cropSplatter.CrossFadeAlpha(0f, 1f, true);
    }

    // Play intro audio, animate holograms and display controls screen
    private IEnumerator PlayIntro()
    {
        playingIntro = true;

        Vector3 holoScale = Vector3.one * 0.25f;

        gameManager.SetMusicVolume(0.05f);
        ufoAudioSource.Play();

        timeRemaining = Mathf.Infinity;

        // Hard-coded timing, seemingly no way around it for now
        while (playingIntro && ufoAudioSource.isPlaying)
        {
            if (ufoAudioSource.time > 20f)
            {
                if (!controllerScreen.gameObject.activeSelf)
                    controllerScreen.gameObject.SetActive(true);
                controllerScreen.CrossFadeAlpha(1f, 0.5f, false);
            }

            if (ufoAudioSource.time > 15f)
            {
                milkText.CrossFadeAlpha(0f, 1f, true);
                if (!holoFarmer.activeSelf)
                {
                    holoFarmer.SetActive(true);
                    holoFarmer.transform.localScale = Vector3.zero;
                }
                holoFarmer.transform.localScale = Vector3.Lerp(holoFarmer.transform.localScale, holoScale, Time.deltaTime);
            }
            // Not exact timings, may need a better solution
            else if (ufoAudioSource.time > 10.75f)
            {
                milkText.text = "MILK";
            }
            else if (ufoAudioSource.time > 10.25f)
            {
                milkText.text = "MIL";
            }
            else if (ufoAudioSource.time > 9.75f)
            {
                milkText.text = "MI";
            }
            else if (ufoAudioSource.time > 9.25f)
            {
                milkText.text = "M";
                milkText.CrossFadeAlpha(1f, 0f, true);
            }
            else
            {
                milkText.text = "";
            }

            if (ufoAudioSource.time > 0f)
            {
                if (!holoCow.activeSelf)
                {
                    holoCow.SetActive(true);
                    holoCow.transform.localScale = Vector3.zero;
                }
                holoCow.transform.localScale = Vector3.Lerp(holoCow.transform.localScale, holoScale, Time.deltaTime);
            }
            yield return null;
        }

        gameManager.SetMusicVolume(0.25f);
        SkipIntro();
    }

    // Set game state after intro sequence
    private void SkipIntro()
    {
        controllerScreen.CrossFadeAlpha(0f, 0.5f, false);
        holoCow.SetActive(false);
        holoFarmer.SetActive(false);
        milkText.CrossFadeAlpha(0f, 1f, true);

        if (ufoAudioSource.isPlaying)
            ufoAudioSource.Stop();


        GameObject[] farmers = GameObject.FindGameObjectsWithTag("Farmer");
        if (farmers.Length > 0)
        {
            foreach (GameObject farmer in farmers)
            {
                SC_FarmerBrain farmerBrain = farmer.GetComponent<SC_FarmerBrain>();
                if (farmerBrain)
                    farmerBrain.peaceful = false;
            }
        }
        StartCoroutine(playStartCutscene());
    }

    private IEnumerator playStartCutscene()
    {
        shipAnim.Play("UFO Intro");
        yield return new WaitForSeconds(3.1f);
        timeRemaining = playTime;
        shipAnim.enabled = false;
        playingIntro = true;
    }

    // Show the endscreen (TO-DO: Replace hard-coded values)
    public void DisplayEndScreen()
    {
        string rating = "";

        if (score > 25)
            rating = "SS";
        else if (score > 20)
            rating = "S";
        else if (score > 17)
            rating = "A";
        else if (score > 13)
            rating = "B";
        else if (score > 7)
            rating = "C";
        else
        {
            rating = "D";

            GameObject[] farmers = GameObject.FindGameObjectsWithTag("Farmer");
            if (farmers.Length > 0)
            {
                foreach (GameObject farmer in farmers)
                {
                    Animator farmerAnimator = farmer.GetComponentInChildren<Animator>();
                    if (farmerAnimator)
                    {
                        farmerAnimator.SetBool("celebrate", true);
                    }
                }
            }

            ufoAudioSource.PlayOneShot(loseAudio, 1f);
        }

        switch (rating)
        {
            case "SS":
            case "S":
            case "A":
                ufoAudioSource.PlayOneShot(winAudio, 1f);
                break;
            default:
                break;
        }

        gameManager.SetMusicVolume(0.1f);

        finalScoreText.text = score + "\n\nRating: " + rating;
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
            paused = false;
            Time.timeScale = timeScaleFactor;
            parameterScreen.SetActive(false);
        }
        else
        {
            paused = true;
            Time.timeScale = Mathf.Epsilon;
            parameterScreen.SetActive(true);
        }

        if (!gameManager.GetGameStarted())
        {
            gameManager.ToggleStartScreen();
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
        switch (name)
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
        if (playingIntro)
            StartGame();
        // Initialize values for private variables
        score = 0;
        scoreText.text = score.ToString("D2");
        reticle.sprite = normalReticle;
        fuel = 100.0f;
        fuelMeter.value = fuel;
        abilityCooldown = 100.0f;
        cooldownMeter.value = abilityCooldown;
        cooldownActive = false;
        if (timeScaleFactor <= Mathf.Epsilon)
            timeScaleFactor = 1.0f;
        Time.timeScale = timeScaleFactor;

        if (cropSplatter.gameObject.activeSelf)
            cropSplatter.gameObject.SetActive(false);

        // Deactivate non-gameplay menus
        endScreen.SetActive(false);
        parameterScreen.SetActive(false);
        helpScreen.SetActive(false);

        paused = false;

        if (controllerScreen.gameObject.activeSelf)
            controllerScreen.gameObject.SetActive(false);

        if (holoCow.activeSelf)
            holoCow.SetActive(false);
        if (holoFarmer.activeSelf)
            holoFarmer.SetActive(false);

        if (gameplayScreen.activeSelf)
            gameplayScreen.SetActive(false);
        topDownCamera.gameObject.SetActive(false);

        if (shipAnim.enabled != true)
        {
            shipAnim.enabled = true;
        }
        else
        {
            shipAnim.StopPlayback();
        }
        
        
    }

    public void StartGame()
    {
        if (!gameplayScreen.activeSelf)
            gameplayScreen.SetActive(true);
        if (!topDownCamera.gameObject.activeSelf)
            topDownCamera.gameObject.SetActive(true);
        if (playIntro)
            StartCoroutine(PlayIntro());
        else
        {
            SkipIntro();
        }
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
