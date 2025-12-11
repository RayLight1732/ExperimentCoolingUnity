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
    [SerializeField]
    private AudioSource noise;
    [SerializeField]
    private AudioSource notice;
    private float stageTime;
    private int stage = 0;// 0:straight 1:curve 2:straight 3: curve
    private int currentCount = 0;


    private Vector3 startPosition;
    private Quaternion startRotation;

    private void ProcessStraight(float deltaTime)
    {
        target.transform.position += target.transform.forward * deltaTime * speed;
    }

    private void ProcessRotation(float deltaTime) {
        float angle = rotationAngleSpeed * deltaTime;
        float distance = deltaTime * speed;
        target.transform.Rotate(Vector3.up,angle,Space.World);
        target.transform.Rotate(Vector3.forward, rotationRollAngleSpeed * deltaTime,Space.Self);
        target.transform.position += target.transform.forward * distance;
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
                        stageTime = newTime-straightTime;
                        stage ++;
                        InvokeAction("high");
                    } else
                    {
                        ProcessStraight(Time.deltaTime);
                        stageTime = newTime;
                    }
                    break;
                }
            case 1:
            case 3:
                {
                    float rotationTime = 180f/rotationAngleSpeed;
                    if (newTime >= rotationTime)
                    {
                        ProcessRotation(rotationTime - time);
                        stage ++;
                        InvokeAction("low");
                        if (stage == 4)
                        {
                            stage = 0;
                            currentCount++;
                            InvokeAction("lapend"+currentCount);
                            notice.Play();
                            if (count == currentCount)
                            {
                                break;
                            }
                        }
                        ProcessStraight(newTime - rotationTime);
                        stageTime = newTime - rotationTime;
                    }
                    else
                    {
                        ProcessRotation(Time.deltaTime);
                        stageTime = newTime;
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
            update(stageTime);
            if (count ==  currentCount)
            {
                OnEndGame();
            }
        }
    }




    protected override void startGame()
    {
        ResetPose();
        stageTime = 0;
        currentCount = 0;
        stage = 0;
        noise.Play();
    }

    protected override void onEndGame()
    {
        ResetPose();
        stageTime = 0;
        noise.Stop();
    }

    protected override void start()
    {
        startPosition = target.gameObject.transform.localPosition;
        startRotation = target.gameObject.transform.rotation;
    }

    public override void ResetPose()
    {

        target.gameObject.transform.position = startPosition;
        target.gameObject.transform.rotation = startRotation;
        Matrix4x4 parentMatrix = target.transform.localToWorldMatrix;
        Matrix4x4 targetLocalMatrix = camera_offset.transform.worldToLocalMatrix * main_camera.transform.localToWorldMatrix;
        Matrix4x4 newOffsetMatrix = parentMatrix * targetLocalMatrix.inverse;
        camera_offset.transform.position = newOffsetMatrix.GetColumn(3);
        camera_offset.transform.rotation = Quaternion.LookRotation(newOffsetMatrix.GetColumn(2),newOffsetMatrix.GetColumn(1));
    }
}
