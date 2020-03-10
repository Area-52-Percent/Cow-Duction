using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public int myid;
    public GameObject app;
    public delegate void InputEventHandler(InputValue inputValue);

    public event InputEventHandler _OnMovement;
    public event InputEventHandler _OnShoot;
    public event InputEventHandler _OnAim;
    public event InputEventHandler _OnTurn;
    public event InputEventHandler _OnAscend;
    public event InputEventHandler _OnDescend;
    public event InputEventHandler _OnTiltRight;
    public event InputEventHandler _OnTiltLeft;
    public event InputEventHandler _OnCloak;
    public event InputEventHandler _OnRelease;
    public event InputEventHandler _OnPushPull;

    private static int idCount = 0;
    private List<string> controllerNames;

    private void Awake()
    {
        myid = idCount++;
    }

    private void Start()
    {
        //InputMaster master = new InputMaster();
        //master.GetComponent<PlayerInput>().actions = master.asset;
        controllerNames = app.GetComponent<Controllers>().controllerNames;
    }

    private void OnMovement(InputValue inputValue)
    {
        string current = Gamepad.current.ToString();
        if (controllerNames.Count > 2)
        {
            if (current == controllerNames[0])
            {
                _OnMovement?.Invoke(inputValue);
            }
        }
        else if (current == "XInputControllerWindows:/XInputControllerWindows")
        {
            _OnMovement?.Invoke(inputValue);
        }
    }

    private void OnShoot(InputValue inputValue)
    {
        string current = Gamepad.current.ToString();
        if (controllerNames.Count > 2)
        {
            if (current == controllerNames[1])
            {
                _OnShoot?.Invoke(inputValue);
            }
        }
        else if (current == "XInputControllerWindows:/XInputControllerWindows1")
        {
            _OnShoot?.Invoke(inputValue);
        }
    }

    private void OnAim(InputValue inputValue)
    {
        string current = Gamepad.current.ToString();
        if (controllerNames.Count > 2)
        {
            if (current == controllerNames[1])
            {
                _OnAim?.Invoke(inputValue);
            }
        }
        else if (current == "XInputControllerWindows:/XInputControllerWindows1")
        {
            _OnAim?.Invoke(inputValue);
        }
    }

    private void OnTurn(InputValue inputValue)
    {
        _OnTurn?.Invoke(inputValue);
    }
    private void OnAscend(InputValue inputValue)
    {
        _OnAscend?.Invoke(inputValue);
    }
    private void OnDescend(InputValue inputValue)
    {
        _OnDescend?.Invoke(inputValue);
    }
    private void OnTiltRight(InputValue inputValue)
    {
        _OnTiltRight?.Invoke(inputValue);
    }
    private void OnTiltLeft(InputValue inputValue)
    {
        _OnTiltLeft?.Invoke(inputValue);
    }
    private void OnCloak(InputValue inputValue)
    {
        _OnCloak?.Invoke(inputValue);
    }
    private void OnRelease(InputValue inputValue)
    {
        _OnRelease?.Invoke(inputValue);
    }
    private void OnPushPull(InputValue inputValue)
    {
        _OnPushPull?.Invoke(inputValue);
    }
}

