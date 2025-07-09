using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressEventHandler : MonoBehaviour
{

    [SerializeField]
    private InputActionReference input;

    public event Action action;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (action != null && input.action.WasPressedThisFrame())
        {
            action.Invoke();
        }
    }
}
