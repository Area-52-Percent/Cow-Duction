/*  ResetButton.cs

    Placed on a UI button to call ResetGame.
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    // Awake is called after all objects are initialized
    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    // Call reset game from game manager
    public void RestartGame()
    {
        if (gameManager.GetGameStarted())
        {
            Application.LoadLevel(0);
        }
            
        //StartCoroutine(gameManager.ResetGame());

    }
}
