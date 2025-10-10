using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Ex1Manager : GameManager
{
    [SerializeField]
    private CinemachineDollyCart cart;
    [SerializeField]
    private int loopCount;
    [SerializeField]
    private float cartSpeed;
    [SerializeField]
    private float high;
    [SerializeField]
    private float low;

    private int currentLoopCount = 0;
    private float lastPosition = 0;


    // Update is called once per frame
    void Update()
    {
        if (Started)
        {
            float currenPosition = cart.m_Position;
            if (currenPosition < lastPosition)
            {
                OnGoal();
            }
            if (lastPosition < high && high < currenPosition)
            {
                InvokeAction("high");
                if (debug)
                {
                    string timeString = DateTime.Now.ToString("HH:mm:ss");
                    Debug.Log($"[{timeString}] Gamemanager:low");
                }
            }
            if (lastPosition < low && low < currenPosition)
            {
                InvokeAction("low");
                if (debug)
                {
                    string timeString = DateTime.Now.ToString("HH:mm:ss");
                    Debug.Log($"[{timeString}] Gamemanager:low");
                }
            }
            lastPosition = currenPosition;
        }
    }


    private void OnGoal()
    {
        currentLoopCount++;
        Debug.Log("OnGoal" + currentLoopCount + "," + loopCount);
        if (currentLoopCount == loopCount)
        {
            OnEndGame();
        }
    }


    public void ResetCart()
    {
        cart.m_Speed = 0;
        cart.m_Position = 0;
        currentLoopCount = 0;
        lastPosition = 0;
    }

    protected override void startGame()
    {
        ResetPose();
        ResetCart();
    }

    protected override void onEndGame()
    {
        ResetCart();
        ResetPose();
    }
}
