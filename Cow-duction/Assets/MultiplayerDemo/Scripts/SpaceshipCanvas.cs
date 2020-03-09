using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton canvas controller for the spaceship.
/// </summary>
public class SpaceshipCanvas : MonoBehaviour
{
    public static SpaceshipCanvas instance;
    public MultiPlayerSpaceshipController spaceshipController;
    public Rigidbody spaceshipRb;

    [Header("Reticle")]
    [Tooltip("An icon used to get the direction of raycasts from the main camera")]
    public RectTransform reticle;
    public Color reticleColorDefault = Color.white;
    public Color reticleColorCow = Color.green;
    public Color reticleColorThreat = Color.red;

    [Header("Fuel")]
    [Tooltip("A slider with min and max values set to 0 and 100 respectively")]
    public Slider fuelMeter;
    public Image fuelMeterFill;
    public float fuelDepletionRate = .5f;
    public float fuel { get; private set; }

    [Header("Ability")]
    [Tooltip("A slider with min and max values set to 0 and 100 respectively")]
    public Slider abilityMeter;
    public Image abilityMeterFill;
    public float abilityDuration = 3;
    public float abilityCooldown = 10;
    public bool abilityActive { get; private set; }
    public float cooldownTimer { get; private set; }
    public bool cooldownActive { get; private set; }

    [Header("Timer")]
    public Text timerText;
    public float timerStartValue = 270;
    public float timeScaleFactor = 1;
    public float timeRemaining { get; private set; }

    [Header("Flight Status")]
    public Text speedText;
    public float speed { get; private set; }
    public Text altitudeText;
    public float altitude { get; private set; }

    [Header("Score")]
    public Text scoreText;
    public Text multiplierText;
    public float score { get; private set; }
    public float multiplier { get; private set; }
    public float multiplierDuration = 10;
    public float multiplierTimer { get; private set; }
    public bool multiplierActive { get; private set; }

    [Header("Waypoint Icon")]
    [Tooltip("An icon that tracks a grappled entity")]
    public RectTransform waypointIcon;
    private Image waypointIconImage;
    public Sprite waypointIconCow, waypointIconThreat, waypointIconUnknown;

    [Header("Crop Splatter")]
    public Image cropSplatter;

    [Header("Animation")]
    public Animator spaceshipAnimator;

    private void OnValidate()
    {
        if (spaceshipController == null) spaceshipController = transform.root.GetComponent<MultiPlayerSpaceshipController>();
        if (spaceshipRb == null) spaceshipRb = transform.root.GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        // Only allow one instance of SpaceshipCanvas
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void Start()
    {
        if (waypointIcon != null) waypointIconImage = waypointIcon.GetComponent<Image>();
    }

    private void Update()
    {
        HandleFuel();

        HandleAbility();

        HandleTimer();

        HandleStatus();
    }

    private void HandleFuel()
    {
        if (fuel > 0)
        {
            fuel -= fuelDepletionRate * Time.deltaTime;

            if (fuelMeter != null) fuelMeter.value = fuel;
        }
        else
        {
            if (fuel < 0) fuel = 0;

            // Restrict spaceship air movement
        }
    }

    private void HandleAbility()
    {
        if (Input.GetButtonDown("Ability"))
        {
            if (!cooldownActive && !abilityActive) StartCoroutine(ActivateAbility());
        }

        if (cooldownActive && cooldownTimer < abilityCooldown)
        {
            cooldownTimer += Time.deltaTime;

            if (abilityMeter != null) abilityMeter.value = (cooldownTimer / abilityCooldown) * 100f;
        }
        else
        {
            if (cooldownTimer >= abilityCooldown) 
            {
                cooldownActive = false;
                cooldownTimer = 0;
            }
        }
    }

    private IEnumerator ActivateAbility()
    {
        abilityActive = true;

        yield return new WaitForSeconds(abilityDuration);

        abilityActive = false;
        cooldownActive = true;
    }

    private void HandleTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= timeScaleFactor * Time.deltaTime;
        }
        else
        {
            if (timeRemaining < 0) timeRemaining = 0;
        }

       UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null) return;

        int min = Mathf.FloorToInt(timeRemaining / 60f);
        int sec = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = min + ":" + sec.ToString("D2");
    }

    private void HandleStatus()
    {
        speed = spaceshipRb.velocity.magnitude;
        altitude = spaceshipRb.transform.position.y;

        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        if (speedText != null) speedText.text = speed.ToString("F1");
        if (altitudeText != null) altitudeText.text = altitude.ToString("F1");
    }

    public void IncreaseScore(int amount)
    {
        score += amount * multiplier;
        multiplier++;
        UpdateScoreText();

        multiplierTimer = 0;
        if (!multiplierActive) StartCoroutine(HandleMultiplier());
    }

    private void UpdateScoreText()
    {
        if (scoreText != null) scoreText.text = score.ToString("D");
        if (multiplierText != null) multiplierText.text = multiplier.ToString("D");
    }

    private IEnumerator HandleMultiplier()
    {
        multiplierActive = true;

        // Yield resetting multiplier until timer exceeds duration
        while (multiplierTimer < multiplierDuration)
        {
            multiplierTimer += Time.deltaTime;
            yield return null;
        }

        multiplier = 1;
        multiplierActive = false;
    }

    public Color GetReticleColor()
    {
        return reticle.GetComponent<Image>().color;
    }

    public void SetReticleColor(Color color)
    {
        reticle.GetComponent<Image>().color = color;
    }

    public void SetWaypointIconSprite(Sprite sprite)
    {
        waypointIconImage.sprite = sprite;
    }

    public IEnumerator ScreenSplatter(float stickDuration, float fadeDuration)
    {
        cropSplatter.gameObject.SetActive(true);

        // Reset transparency
        if (cropSplatter.color.a < 1)
        {
            Color splatterColor = cropSplatter.color;
            splatterColor.a = 1;
            cropSplatter.color = splatterColor;
        }

        yield return new WaitForSeconds(stickDuration);

        cropSplatter.CrossFadeAlpha(0, fadeDuration, false);

        yield return new WaitForSeconds(fadeDuration);

        cropSplatter.gameObject.SetActive(false);
    }
}
