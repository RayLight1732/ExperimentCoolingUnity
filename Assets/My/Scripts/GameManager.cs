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

public abstract class GameManager : SendMessageEventProvider
{

    [SerializeField]
    private StartEventProvider[] startEventProviders;
    [SerializeField]
    public bool debug = false;
    [SerializeField]
    protected GameObject camera_offset;
    [SerializeField]
    protected GameObject head;

    private bool started = false;
    public bool Started
    {
        get { return started; }
    }

    private void Start()
    {
        // StartEventProvider ‚ÌƒCƒxƒ“ƒg“o˜^
        foreach (var handler in startEventProviders)
        {
            handler.Action += StartGame;
        }
        start();
    }


    private void StartGame()
    {
        started = true;
        foreach (var handler in startEventProviders)
        {
            handler.Action -= StartGame;
        }
        InvokeAction("start");
        Debug.Log("Start game");
        startGame();
    }

    protected abstract void startGame();


    public virtual void ResetPose()
    {

        camera_offset.transform.localPosition = -1 * head.transform.localPosition;
        camera_offset.transform.localRotation = Quaternion.Inverse(head.transform.localRotation);


    }


    protected abstract void onEndGame();

    protected void OnEndGame()
    {
        started = false;
        onEndGame();
        foreach (var handler in startEventProviders)
        {
            handler.Action += StartGame;
        }
        InvokeAction("end");
        Debug.Log("End game");
    }

    public void StopGame()
    {
        OnEndGame();
    }

    protected virtual void start()
    {

    }
}
