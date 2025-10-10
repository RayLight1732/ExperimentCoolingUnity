using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Ex2Manager : GameManager
{
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private float frequency = 2;
    [SerializeField]
    private int count = 5;
    [SerializeField]
    private float maxAmplitude = 15;
    [SerializeField]
    private float amplitudePeriod = 5;


    float time = 0;


    private void update(float time)
    {
        double amplitude = maxAmplitude * (1 - Math.Abs(time % (2 * amplitudePeriod) - amplitudePeriod) / amplitudePeriod);
        double pos = amplitude* Math.Sin(2*Math.PI*time*frequency);
        target.transform.localPosition = new Vector3((float)amplitude,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Started)
        {
            time += Time.deltaTime;
            if (time > count/frequency)
            {
                time = count/frequency;
                update(time);
                OnEndGame();
            } else
            {
                update(time);
            }
        }
    }




    protected override void startGame()
    {
        ResetPose();
        time = 0;
        target.gameObject.transform.localPosition = Vector3.zero;
    }

    protected override void onEndGame()
    {
        ResetPose();
        time = 0;
    }
}
