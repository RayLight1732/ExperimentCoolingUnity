using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;



public class Ex3Manager : GameManager
{
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float straightTime;
    [SerializeField]
    private float rotationAngleSpeed;
    [SerializeField]
    private float rotationRollAngleSpeed;
    [SerializeField]
    private int count;
    private float time;
    private int stage = 0;// 0:straight 1:curve 2:straight 3: curve
    private int currentCount = 0;

    private void ProcessStraight(float deltaTime)
    {
        target.transform.localPosition += new Vector3(deltaTime*speed, 0, 0);
    }

    private void ProcessRotation(float deltaTime) {
        target.transform.rotation = Quaternion.Euler(0, rotationAngleSpeed*deltaTime, rotationRollAngleSpeed*deltaTime) * target.transform.rotation;
        target.transform.localPosition += new Vector3(deltaTime * speed, 0, 0);
    }


    private void update(float time)
    {
        float newTime = time + Time.deltaTime;
        switch (stage)
        {
            case 0:
            case 2:
                {
                    if (newTime >= straightTime)
                    {
                        ProcessStraight(straightTime-time);
                        ProcessRotation(newTime-straightTime);
                        time = newTime-straightTime;
                        currentCount ++;
                        InvokeAction("high");
                    } else
                    {
                        ProcessStraight(Time.deltaTime);
                        time = newTime;
                    }
                    break;
                }
            case 1:
            case 3:
                {
                    float rotationTime = 180f/rotationAngleSpeed;
                    if (newTime >= rotationTime)
                    {
                        ProcessStraight(rotationTime - time);
                        stage += 1;
                        if (stage == 4)
                        {
                            stage = 0;
                            currentCount++;

                            InvokeAction("low");
                            if (count == currentCount)
                            {
                                break;
                            }
                        }
                        ProcessRotation(newTime - rotationTime);
                        time = newTime - rotationTime;
                    }
                    else
                    {
                        ProcessStraight(Time.deltaTime);
                        time = newTime;
                    }
                    break;
                }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Started)
        {
            time += Time.deltaTime;
            if (time > count * amplitudePeriod)
            {
                time = count * amplitudePeriod;
                update(time);
                OnEndGame();
            }
            else
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
