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
    private GameObject ufo;
    private SC_AlienUIManager uiManager;

    // Awake is called after all objects are initialized
    void Awake()
    {
        ufo = GameObject.Find("UFO");
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
    }

    // Load the active scene
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Reset gameplay parameters to default values
        uiManager.ResetGame();

        SC_CowAbduction ufoCa = ufo.GetComponent<SC_CowAbduction>();
        ufoCa.ResetGame();

        SC_SpaceshipMovement ufoSM = ufo.GetComponent<SC_SpaceshipMovement>();
        ufoSM.ResetGame();
    }
}
