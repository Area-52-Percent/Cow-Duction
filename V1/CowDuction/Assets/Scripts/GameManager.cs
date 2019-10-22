using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject ufo;
    [SerializeField] UIManager uiManager;

    // Awake is called after all objects are initialized
    void Awake()
    {
        ufo = GameObject.Find("UFO");
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
    }

    // Load the active scene
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;

        // Reset gameplay parameters to default values
        uiManager.ResetGame();

        CowAbduction ufoCa = ufo.GetComponent<CowAbduction>();
        ufoCa.ResetGame();

        Rigidbody ufoRb = ufo.GetComponent<Rigidbody>();
        ufoRb.MovePosition(Vector3.up * 10.0f);
        ufoRb.MoveRotation(Quaternion.identity);
    }
}
