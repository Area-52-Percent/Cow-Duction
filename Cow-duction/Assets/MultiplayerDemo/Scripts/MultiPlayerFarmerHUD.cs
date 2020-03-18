using UnityEngine;

public class MultiPlayerFarmerHUD : MonoBehaviour
{
    NetworkManagerCowductionHUD netHud;
    public RectTransform spaceshipTracker;
    private Transform spaceshipTransform;

    private void Awake()
    {
        netHud = NetworkManagerCowduction.singleton.GetComponent<NetworkManagerCowductionHUD>();
    }

    private void Start()
    {
        spaceshipTransform = GameObject.FindGameObjectWithTag("UFO").transform;
    }

    private void Update()
    {
        TrackSpaceship();
    }

    public void OpenPauseMenu()
    {
        netHud.TogglePauseMenu();
    }

    public void TrackSpaceship()
    {
        Vector3 spaceshipTrackerPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, spaceshipTransform.position + spaceshipTransform.up * 3f);

        // Clamp x position
        if (spaceshipTrackerPosition.x < spaceshipTracker.rect.width / 2)
            spaceshipTrackerPosition.x = spaceshipTracker.rect.width / 2;
        else if (spaceshipTrackerPosition.x > Screen.width - (spaceshipTracker.rect.width / 2))
            spaceshipTrackerPosition.x = Screen.width - (spaceshipTracker.rect.width / 2);

        // Clamp y position
        if (spaceshipTrackerPosition.y < spaceshipTracker.rect.height / 2)
            spaceshipTrackerPosition.y = spaceshipTracker.rect.height / 2;
        else if (spaceshipTrackerPosition.y > Screen.height - (spaceshipTracker.rect.height / 2))
            spaceshipTrackerPosition.y = Screen.height - (spaceshipTracker.rect.height / 2);

        // Set position
        spaceshipTracker.position = spaceshipTrackerPosition;
    }
}
