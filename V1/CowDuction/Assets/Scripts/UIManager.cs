using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Referenced objects
    [SerializeField] private Rigidbody _rbUFO;
    // Raw data
    [SerializeField] private int score = 0;
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
    }
}
