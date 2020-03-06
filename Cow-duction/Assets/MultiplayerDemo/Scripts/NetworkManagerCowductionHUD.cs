using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/// <summary>
/// An extension for the NetworkManager that displays a default HUD for controlling the network state of the game.
/// <para>This component also shows useful internal state for the networking system in the inspector window of the editor. It allows users to view connections, networked objects, message handlers, and packet statistics. This information can be helpful when debugging networked games.</para>
/// </summary>
[RequireComponent(typeof(NetworkManagerCowduction))]
[EditorBrowsable(EditorBrowsableState.Never)]
public class NetworkManagerCowductionHUD : MonoBehaviour
{
    NetworkManagerCowduction manager;

    /// <summary>
    /// Whether to show the default control HUD at runtime.
    /// </summary>
    public bool showGUI = true;
    public bool showFPS = true;

    [Header("Custom GUI")]
    public GameObject mainMenu;
    public InputField networkAddressField;
    public GameObject pauseMenu;

    [SerializeField] private GameObject endScreen = null; // Set up in inspector
    [SerializeField] private AudioClip loseAudio = null; // Set up in inspector
    [SerializeField] private AudioClip winAudio = null; // Set up in inspector
    [SerializeField] private Text finalScoreText = null; // Set up in inspector

    private float deltaTime = 0f;

    void Awake()
    {
        manager = GetComponent<NetworkManagerCowduction>();
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);

        networkAddressField.text = "localhost";
    }

    void OnGUI()
    {
        if (showFPS)
        {
            int w = Screen.width, h = Screen.height;
    
            GUIStyle style = new GUIStyle();
    
            Rect rect = new Rect(0, 0, w, h * 4 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 4 / 100;
            style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }

        if (!showGUI)
            return;

        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                // LAN Host
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (GUILayout.Button("LAN Host"))
                    {
                        manager.StartHost();
                    }
                }

                // LAN Client + IP
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("LAN Client"))
                {
                    manager.StartClient();
                }
                manager.networkAddress = GUILayout.TextField(manager.networkAddress);
                GUILayout.EndHorizontal();

                // LAN Server Only
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    // cant be a server in webgl build
                    GUILayout.Box("(  WebGL cannot be server  )");
                }
                else
                {
                    if (GUILayout.Button("LAN Server Only")) manager.StartServer();
                }
            }
            else
            {
                // Connecting
                GUILayout.Label("Connecting to " + manager.networkAddress + "..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                {
                    manager.StopClient();
                }
            }
        }
        else
        {
            // server / client status message
            if (NetworkServer.active)
            {
                GUILayout.Label("Server: active. Transport: " + Transport.activeTransport);
            }
            if (NetworkClient.isConnected)
            {
                GUILayout.Label("Client: address=" + manager.networkAddress);
            }
        }

        // client ready
        if (NetworkClient.isConnected && !ClientScene.ready)
        {
            if (GUILayout.Button("Client Ready"))
            {
                ClientScene.Ready(NetworkClient.connection);

                if (ClientScene.localPlayer == null)
                {
                    ClientScene.AddPlayer();
                }
            }
        }

        // stop
        if (NetworkServer.active || NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop"))
            {
                manager.StopHost();
            }
        }

        GUILayout.EndArea();
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                if (!mainMenu.activeSelf)
                {
                    mainMenu.SetActive(true);
                }
                if (pauseMenu.activeSelf)
                {
                    pauseMenu.SetActive(false);
                }
            }
        }
        if (Input.GetButtonDown("Cancel") && !Input.GetKey("left shift"))
        {
            TogglePauseMenu();
        }
    }

    public void PlayAsSpaceShip()
    {
        manager.StartHost();
        mainMenu.SetActive(false);
    }

    public void PlayAsFarmer()
    {
        manager.networkAddress = networkAddressField.text;
        manager.StartClient();
        mainMenu.SetActive(false);
    }

    public void TogglePauseMenu()
    {
        if (mainMenu.activeSelf) return;

        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
        }

        else
        {
            pauseMenu.SetActive(true);
        }
    }

    public void DisplayEndScreen(int score, AudioSource ufoAudioSource, AudioClip loseAudio, AudioClip winAudio)
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

        //gameManager.SetMusicVolume(0.1f);

        finalScoreText.text = score + "\n\nRating: " + rating;
        endScreen.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        if (NetworkServer.active || NetworkClient.isConnected)
        {
            manager.StopHost();
        }
        pauseMenu.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
