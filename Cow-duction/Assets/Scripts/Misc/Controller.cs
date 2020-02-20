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

    private void OnMovement(InputValue inputValue)
    {
        Debug.Log("moving");
        //_OnMovement?.Invoke(inputValue.GetComponents<InputValue>()[0]);
    }

    private void OnShoot(InputValue inputValue)
    {
        Debug.Log("shoot");
        Debug.Log(inputValue.Get());
        _OnShoot?.Invoke(inputValue);
    }
}
