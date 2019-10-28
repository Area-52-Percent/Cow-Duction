/* GameManager.cs
   
   Handles loading scenes and resetting game.

   Assumptions:
     There is a GameObject in the scene called "UFO" with a Rigidbody component.
     There is a GameObject in the scene with the "UIManager" tag and UIManager component.
 */

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

        SpaceshipMovement ufoSM = ufo.GetComponent<SpaceshipMovement>();
        ufoSM.ResetGame();
    }
}
