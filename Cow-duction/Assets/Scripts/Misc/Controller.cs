using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public delegate void InputEventHandler(InputValue inputValue);

    public event InputEventHandler _OnMovement;
    public event InputEventHandler _OnShoot;
    public event InputEventHandler _OnTest;

    private void OnMovement(InputValue inputValue)
    {
        Debug.Log(inputValue);
        _OnMovement?.Invoke(inputValue);
    }

    private void OnShoot(InputValue inputValue)
    {
        Debug.Log("shoot");
        _OnShoot?.Invoke(inputValue);
    }

    private void OnTest(InputValue inputValue)
    {
        Debug.Log("OnTestEvent");
        _OnTest?.Invoke(inputValue);
    }
}
