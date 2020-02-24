/*  ResetButton.cs

    Placed on a UI button to call ResetGame.
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SC_AlienUIManager uiManager;

    // Awake is called after all objects are initialized
    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        uiManager = GameObject.Find("UI").GetComponent<SC_AlienUIManager>();
    }

    // Call reset game from game manager
    public void ResetGame()
    {
        if (gameManager.GetGameStarted())
        {
            uiManager.resetting = true;
            StartCoroutine(gameManager.ResetGame());
        }
            
        //SceneManager.LoadScene("Feb11_MergeScene", LoadSceneMode.Single);

    }
}
