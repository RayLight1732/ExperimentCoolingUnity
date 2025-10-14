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
    private GameObject camera_offset;
    [SerializeField]
    private GameObject main_camera;

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
    }


    private void StartGame()
    {
        started = true;
        foreach (var handler in startEventProviders)
        {
            handler.Action -= StartGame;
        }
        Debug.Log("Start game");
        startGame();
    }

    protected abstract void startGame();


    public void ResetPose()
    {
        float rot_y = main_camera.transform.localEulerAngles.y;
        camera_offset.transform.localPosition = -1 * main_camera.transform.localPosition;
        camera_offset.transform.localRotation = Quaternion.Euler(0, -1 * rot_y, 0);

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
}
