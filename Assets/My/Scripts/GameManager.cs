using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEventProvider : MonoBehaviour
{
    public event Action Action;
    public void InvokeAction()
    {
        if (Action != null) Action.Invoke();
    }
}

public class GameManager : SendMessageEventProvider
{
    [SerializeField]
    private CinemachineDollyCart cart;
    [SerializeField]
    private int loopCount;
    [SerializeField]
    private float cartSpeed;
    [SerializeField]
    private StartEventProvider startEventHandler;
    [SerializeField]
    private float high;
    [SerializeField]
    private float low;

    private int currentLoopCount = 0;
    private bool started = false;
    private float lastPosition = 0;
    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        float currenPosition = cart.m_Position;
        if (currenPosition < lastPosition)
        {
            OnGoal();
        }
        if (lastPosition < high && high < currenPosition)
        {
            InvokeAction("high");
        }
        if (lastPosition < low && low < currenPosition)
        {
            InvokeAction("low");
        }
        lastPosition = currenPosition;
    }

    public void StartGame()
    {
        started = true;
        cart.m_Speed = cartSpeed;
        startEventHandler.Action -= StartGame;
        Debug.Log("Start game");
    }

    public void OnGoal()
    {
        if (started)
        {
            currentLoopCount++;
            Debug.Log("OnGoal" + currentLoopCount + "," + loopCount);
            if (currentLoopCount == loopCount)
            {
                OnEndGame();
            }
        }
    }


    public void Reset()
    {
        cart.m_Speed = 0;
        cart.m_Position = 0;
        currentLoopCount = 0;
        started = false;

        startEventHandler.Action += StartGame;
        lastPosition = 0;
    }

    private void OnEndGame()
    {
        Reset();
        InvokeAction("end");
        Debug.Log("End game");
    }
}
