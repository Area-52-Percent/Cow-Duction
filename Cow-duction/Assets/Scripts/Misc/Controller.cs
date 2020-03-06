﻿using System.Collections;
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
        if (current == "XInputControllerWindows:/XInputControllerWindows1")
        {
            _OnShoot?.Invoke(inputValue);
        }
    }
}