/*  ResetButton.cs

    Placed on a UI button to call ResetGame.
*/

using UnityEngine;

public class ResetButton : MonoBehaviour
{
    private MultiPlayerGameManager gameManager;

    void Start()
    {
        gameManager = MultiPlayerGameManager.instance;
    }

    // Call reset game from game manager
    public void ResetGame()
    {
        print("Reset");
        // if (gameManager.GetGameStarted())
            StartCoroutine(gameManager.ResetGame());
    }
}
