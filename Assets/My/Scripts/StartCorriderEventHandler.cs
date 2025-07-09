using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCorriderEventHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    public event Action action;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            Debug.Log("OnTriggerEnter");
            action.Invoke();
        }
    }

}
