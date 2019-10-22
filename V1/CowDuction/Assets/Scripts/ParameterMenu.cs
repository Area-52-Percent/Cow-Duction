using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParameterMenu : MonoBehaviour
{
    private bool MenuOpen;
    public GameObject Menu;
    // Start is called before the first frame update
    void Start()
    {
        MenuOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !MenuOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            MenuOpen = true;
            Menu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && MenuOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            MenuOpen = false;
            Menu.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            //Restart to Main Menu
            //SceneManager.LoadScene(0);
            //Restart Current Scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
