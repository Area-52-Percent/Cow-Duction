using UnityEngine;

public class MultiPlayerFarmerHUD : MonoBehaviour
{
    NetworkManagerCowductionHUD netHud;

    private void Awake()
    {
        netHud = NetworkManagerCowduction.singleton.GetComponent<NetworkManagerCowductionHUD>();
    }

    public void OpenPauseMenu()
    {
        netHud.TogglePauseMenu();
    }
}
