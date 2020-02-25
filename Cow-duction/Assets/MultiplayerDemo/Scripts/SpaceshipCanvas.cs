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

    [Header("Waypoint Icon")]
    [Tooltip("An icon that tracks a grappled entity")]
    public RectTransform waypointIcon;
    private Image waypointIconImage;
    public Sprite waypointIconCow, waypointIconThreat, waypointIconUnknown;

    void Awake()
    {
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
        // TODO: Implement rest of UI elements
        // - Fuel
        // - Ability
        // - Timer
        // - Speed & Height
        // - Score & Multiplier
        // - Input Events
    }

    public Color GetReticleColor()
    {
        return reticle.GetComponent<Image>().color;
    }

    public void SetReticleColor(Color color)
    {
        reticle.GetComponent<Image>().color = color;
    }

    public void SetWaypointIconSprite(string iconName)
    {
        switch (iconName)
        {
            default:
            case "?": // Unknown
                waypointIconImage.sprite = waypointIconUnknown;
                break;
            case "Cow": // Cow
                waypointIconImage.sprite = waypointIconCow;
                break;
            case "!": // Threat
                waypointIconImage.sprite = waypointIconThreat;
                break;
        }
    }
}
