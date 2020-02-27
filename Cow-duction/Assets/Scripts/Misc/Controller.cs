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
        InputMaster master = new InputMaster();
        this.GetComponent<PlayerInput>().actions = master.asset;
    }

    private void OnMovement(InputValue inputValue)
    {
        if (myid == 0)
        {
            Debug.Log("moving");
            Debug.Log(myid);
            _OnMovement?.Invoke(inputValue);
        }
    }

    private void OnShoot(InputValue inputValue)
    {
        if (myid == 1)
        {
            Debug.Log("shoot");
            Debug.Log(myid);
            _OnShoot?.Invoke(inputValue);
        }
    }
}
