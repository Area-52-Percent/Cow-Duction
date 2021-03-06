﻿/* GameManager.cs
   
   Handles loading scenes and resetting game.

   Assumptions:
     There is a GameObject in the scene called "UFO" with a Rigidbody component.
     There is a GameObject in the scene with the "UIManager" tag and UIManager component.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameObject ufo;
    [SerializeField] private GameObject ufoStartLocation;
    [SerializeField] private Camera startCamera;
    [SerializeField] private GameObject startScreen;
    private Camera mainCamera;
    private SC_AlienUIManager uiManager;
    private AudioSource musicAudioSource;
    [SerializeField] private AudioClip startSFX = null;
    [SerializeField] private AudioClip menuMusic = null;
    [SerializeField] private AudioClip gameplayMusic = null;

    private bool gameStarting = false;
    private bool gameStarted = false;

    public bool GetGameStarted()
    {
        return gameStarted;
    }

    public bool GetGameStarting()
    {
        return gameStarting;
    }

    public void SetMusicVolume(float volume)
    {
        musicAudioSource.volume = volume;
    }

    // Awake is called after all objects are initialized
    void Start()
    {
        ufo = GameObject.Find("UFO");
        ufoStartLocation = GameObject.Find("UFOOriginalLoc");
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
        musicAudioSource = GetComponent<AudioSource>();
        SetMusicVolume(0.5f);

        mainCamera = Camera.main;

        SoftReset();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted && !gameStarting && Time.timeScale > Mathf.Epsilon)
        {
            if (Input.anyKey && !Input.GetKey(KeyCode.Escape))
            {
                musicAudioSource.PlayOneShot(startSFX);
                StartCoroutine(StartGame());
            }
        }
    }

    private IEnumerator StartGame()
    {
        gameStarting = true;

        if (SceneManager.GetActiveScene().name == "Start")
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        else
        {
            startScreen.SetActive(false);

            startCamera.GetComponent<CameraRotation>().enabled = false;

            do
            {
                startCamera.transform.position = Vector3.Lerp(startCamera.transform.position, mainCamera.transform.position, Time.deltaTime * 3f);
                startCamera.transform.rotation = Quaternion.Lerp(startCamera.transform.rotation, Quaternion.LookRotation(mainCamera.transform.forward), Time.deltaTime * 3f);
                yield return null;
            } while (Vector3.Distance(startCamera.transform.position, mainCamera.transform.position) > 0.1f);

            mainCamera.gameObject.SetActive(true);
            startCamera.gameObject.SetActive(false);
        }

        gameStarted = true;
        gameStarting = false;
        
        uiManager.StartGame();

        musicAudioSource.clip = gameplayMusic;
        musicAudioSource.Play();
    }

    public void ToggleStartScreen()
    {
        if (startScreen.activeSelf)
        {
            startScreen.SetActive(false);
        }
        else
        {
            startScreen.SetActive(true);
        }
    }

    // Load the active scene
    public IEnumerator ResetGame()
    {
        gameStarted = false;

        AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            while (!asyncSceneLoad.isDone)
                yield return null;

        SoftReset();

        // Reset gameplay parameters to default values
        uiManager.ResetGame();

        SC_CowAbduction ufoCa = ufo.GetComponent<SC_CowAbduction>();
        ufoCa.ResetGame();

        SC_SpaceshipMovement ufoSM = ufo.GetComponent<SC_SpaceshipMovement>();
        ufoSM.ResetGame();

        SetMusicVolume(0.5f);
    }

    // Reset scene without re-finding components attached to dont destroy objects
    private void SoftReset()
    {
        musicAudioSource.clip = menuMusic;
        musicAudioSource.Play();

        ufo.transform.localPosition = ufoStartLocation.transform.localPosition;

        startScreen = GameObject.Find("Start Screen");
        startCamera = GameObject.Find("Start Camera").GetComponent<Camera>();

        mainCamera.gameObject.SetActive(false);

        musicAudioSource.clip = menuMusic;

        gameStarted = false;
        gameStarting = false;

        GameObject[] farmers = GameObject.FindGameObjectsWithTag("Farmer");
        if (farmers.Length > 0)
        {
            foreach (GameObject farmer in farmers)
            {
                SC_FarmerBrain farmerBrain = farmer.GetComponent<SC_FarmerBrain>();
                if (farmerBrain)
                    farmerBrain.peaceful = true;
            }
        }
    }
}
