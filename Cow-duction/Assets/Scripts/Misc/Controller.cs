using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public int myid;
    static int idCount = 0;
    public delegate void InputEventHandler(InputValue inputValue);

    public event InputEventHandler _OnMovement;
    public event InputEventHandler _OnShoot;

    private void Awake()
    {
        myid = idCount++;
        Debug.Log("myid = " + myid);
    }

    private void OnMovement(InputValue inputValue)
    {
        if (myid == 0)
        {
            Debug.Log("moving");
            _OnMovement?.Invoke(inputValue);
        }
    }

    private void OnShoot(InputValue inputValue)
    {
        if (myid == 1)
        {
            Debug.Log("shoot");
            Debug.Log(inputValue.Get());
            _OnShoot?.Invoke(inputValue);
        }
    }
}
