using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Referenced objects
    [SerializeField] private Rigidbody _rbUFO;
    // Raw data
    [SerializeField] private int score = 0;
    public float scoreToFuelRatio = 10.0f;
    [SerializeField] private float fuel; // Percentage
    public float fuelDepletionRate = 1.0f;
    [SerializeField] private float abilityCooldown; // Percentage
    [SerializeField] private bool abilityReady;
    [SerializeField] private bool cooldownActive;
    public float cooldownRegenerationRate = 5.0f;
    [SerializeField] private float ufoSpeed;
    [SerializeField] private float ufoAltitude;
    [SerializeField] private float timeRemaining; // Seconds
    // Gameplay UI Elements
    public Text scoreText;
    public Text speedText;
    public Text altitudeText;
    public Text timeText;
    public Slider fuelMeter;
    public Slider cooldownMeter;
    // Endscreen UI Elements
    public GameObject endScreen;
    public Text finalScoreText;

    // Start is called before the first frame update
    void Start()
    {
        _rbUFO = GameObject.Find("UFO").GetComponent<Rigidbody>();
        fuel = 100.0f;
        abilityCooldown = 100.0f;
        abilityReady = true;
        cooldownActive = false;
        timeRemaining = 240.0f;
        endScreen.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        speedText.text = _rbUFO.velocity.magnitude.ToString("F1");
        altitudeText.text = _rbUFO.transform.position.y.ToString("F1");
        
        if (fuel > 0.0f)
        {
            fuel -= Time.fixedDeltaTime * fuelDepletionRate;
            fuelMeter.value = fuel;
        }
        else
        {
            fuel = 0.0f;
            DisplayEndScreen();
        }

        // Currently for testing purposes
        if (Input.GetKeyDown(KeyCode.F))
        {
            UseAbility();
        }

        if (cooldownActive && abilityCooldown < 100.0f)
        {
            abilityCooldown += Time.fixedDeltaTime * cooldownRegenerationRate;
            cooldownMeter.value = abilityCooldown;
        }
        else
        {
            abilityCooldown = 100.0f;
            abilityReady = true;
            cooldownActive = false;
        }

        if (timeRemaining > 0.0f)
        {
            timeRemaining -= Time.fixedDeltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60.0f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60.0f);
            timeText.text = minutes + ":" + seconds.ToString("D2");
        }
        else
        {
            timeRemaining = 0.0f;
            DisplayEndScreen();
        }
    }

    // Increase score and fuel
    public void IncreaseScore(int amount)
    {
        score += amount;
        fuel += amount * scoreToFuelRatio;
        scoreText.text = score.ToString("D2");
    }

    // Put ability on cooldown
    public void UseAbility()
    {
        abilityCooldown = 0.0f;
        abilityReady = false;        
        cooldownActive = true;
    }

    // Show the endscreen
    public void DisplayEndScreen()
    {
        finalScoreText.text = "" + score;
        endScreen.SetActive(true);
    }
}
