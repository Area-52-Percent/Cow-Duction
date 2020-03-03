using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public int myid;
    public delegate void InputEventHandler(InputValue inputValue);

    public event InputEventHandler _OnMovement;
    public event InputEventHandler _OnShoot;

    private static int idCount = 0;

    private void Awake()
    {
        myid = idCount++;
        Debug.Log("myid = " + myid);
    }

    private void Start()
    {
        //InputMaster master = new InputMaster();
        //master.GetComponent<PlayerInput>().actions = master.asset;
    }

    //Vector 2 - x:Horizontal, y:Vertical
    private void OnMovement(InputValue inputValue)
    {
        string current = Gamepad.current.ToString();
        if (current == "XInputControllerWindows:/XInputControllerWindows")
        {
            Debug.Log(0);
            _OnMovement?.Invoke(inputValue);
        }
    }

    private void OnShoot(InputValue inputValue)
    {
        string current = Gamepad.current.ToString();
        if (current == "XInputControllerWindows:/XInputControllerWindows1")
        {
            Debug.Log(1);
            _OnShoot?.Invoke(inputValue);
        }
    }
}
