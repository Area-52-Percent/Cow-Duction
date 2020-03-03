using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton canvas controller for the spaceship.
/// </summary>
public class SpaceshipCanvas : MonoBehaviour
{
    public static SpaceshipCanvas instance;

    [Tooltip("An icon used to get the direction of raycasts from the main camera")]
    public RectTransform reticle;

    [Header("Fuel")]
    public Slider fuelMeter;
    public Image fuelMeterFill;
    public float fuelDepletionRate = .5f;
    public float fuel { get; private set; }

    [Header("Ability")]
    public Slider abilityMeter;
    public Image abilityMeterFill;
    public float abilityRegenRate = 5;
    public float abilityCooldown { get; private set; }
    private bool cooldownActive;

    [Header("Timer")]
    public Text timerText;
    public float timerStartValue = 270;
    public float timeScaleFactor = 1;
    public float timeRemaining { get; private set; }

    [Header("Waypoint Icon")]
    [Tooltip("An icon that tracks a grappled entity")]
    public RectTransform waypointIcon;
    private Image waypointIconImage;
    public Sprite waypointIconCow, waypointIconThreat, waypointIconUnknown;

    [Header("Crop Splatter")]
    public Image cropSplatter;

    [Header("Ammo Effects")]
    public MultiPlayerSpaceshipController ufoMovement;
    public GameObject fullscreenSplatter;
    public float effectCD = 2f;
    private bool effected = false;

    void Awake()
    {
        // Only allow one instance of SpaceshipCanvas
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    void Start()
    {
        if (waypointIcon != null) waypointIconImage = waypointIcon.GetComponent<Image>();
    }

    void Update()
    {
        HandleFuel();

        HandleAbility();

        HandleTimer();

        HandleStatus();

        // TODO:
        // - Score & Multiplier
        // - Input Events
    }

    private void HandleFuel()
    {
        if (!fuelMeter || !fuelMeterFill) return;

        if (fuel > 0)
        {
            fuel -= fuelDepletionRate * Time.deltaTime;
            fuelMeter.value = fuel;
        }
        else
        {
            if (fuel < 0) fuel = 0;

            // Restrict spaceship air movement
        }
    }

    private void HandleAbility()
    {
        if (!abilityMeter || !abilityMeterFill) return;

        if (cooldownActive && abilityCooldown < 100)
        {
            abilityCooldown += abilityRegenRate * Time.deltaTime;
            abilityMeter.value = abilityCooldown;
        }
    }

    private void HandleTimer()
    {
        // TODO
    }

    private void HandleStatus()
    {
        // TODO
    }

    public void IncreaseScore(int amount)
    {
        // TODO
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

    public void TakeDamage(float amount, char type)
    {
        //StartCoroutine(AnimateDamage(position));
        if (!effected)
        {
            if (type == 'p')
                StartCoroutine(PotatoEffect());
            if (type == 'r')
                StartCoroutine(CarrotEffect());
            if (type == 'c')
                StartCoroutine(CornEffect());
        }
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
        fullscreenSplatter.SetActive(true);
        yield return new WaitForSeconds(effectCD);
        fullscreenSplatter.SetActive(false);
        effected = false;
    }

    public IEnumerator CornEffect()
    {
        effected = true;
        ufoMovement.AddImpulseForce(new Vector3(0, -1f, 0), 250f);
        yield return new WaitForSeconds(effectCD);
        effected = false;
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
