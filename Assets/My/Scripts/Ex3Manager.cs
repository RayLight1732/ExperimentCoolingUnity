
using UnityEngine;


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
    [SerializeField]
    private Transform rotationPivot;
    private float stageTime;
    private int stage = 0;// 0:straight 1:curve 2:straight 3: curve
    private int currentCount = 0;

    int pitchSign; // +1 or -1
    int yawSign;
    int rollSign;
    int fovSign;


    private Vector3 startPosition;
    private Quaternion startRotation;

    private bool lastIsRotation = false;
    private void ProcessStraight(float deltaTime)
    {
        if (lastIsRotation)
        {
            lastIsRotation = false;
            rotationPivot.transform.localRotation = Quaternion.identity;
        }
        target.transform.position += target.transform.forward * deltaTime * speed;
    }

    private float rotateT;

    private void ProcessRotation(float deltaTime) {
        if (!lastIsRotation)
        {
            rotateT = 0;
            lastIsRotation = true;
        }
        rotateT += deltaTime;
        // 親が進む距離
        float distance = deltaTime * speed;
        // 親のヨー (Y軸)
        float parentYaw = rotationAngleSpeed * deltaTime;
        target.transform.Rotate(Vector3.up,parentYaw,Space.World);
        target.transform.position += target.transform.forward * distance;

        //float sway = -0.05f * Mathf.Sin(rotateT * Mathf.PI * 2f / 10f);
        //Camera.main.transform.localPosition = new Vector3(sway, 0, 0);

        // カメラの回転リセット
        rotationPivot.transform.localRotation = Quaternion.identity;
        // ピッチ（X軸）：10秒で3回、±30°
        float s = pitchSign* Mathf.Sin(rotateT * Mathf.PI * 3f / 10f);
        float pitch = 30f * s * s * s;
        rotationPivot.transform.Rotate(Vector3.right, pitch, Space.Self);

        // ヨー（Y軸）：10秒で2回、±40°
        float s2 = yawSign* Mathf.Sin(rotateT * Mathf.PI * 2f / 10f);
        float yaw = 40f * s2 * s2 * s2;
        rotationPivot.transform.Rotate(Vector3.up, -yaw, Space.Self);

        // ロール（Z軸）：10秒で2回、±20°
        float s3 = rollSign *Mathf.Sin(rotateT * Mathf.PI * 2f / 10f);
        float roll = 20f * s3 * s3 * s3;
        rotationPivot.transform.Rotate(Vector3.forward, roll, Space.Self);

        Camera.main.fieldOfView = 60f + fovSign* 20f * Mathf.Sin(rotateT * Mathf.PI*2/10);
    }

    private void updateRotationSign(int phase)
    {
        Random.InitState(phase * 7919); // 再現性あり

        pitchSign = Random.value < 0.5f ? 1 : -1;
        yawSign = Random.value < 0.5f ? 1 : -1;
        rollSign = Random.value < 0.5f ? 1 : -1;
        fovSign = Random.value < 0.5f ? 1 : -1;
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
                        updateRotationSign(stage+currentCount*2);
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
        Matrix4x4 targetLocalMatrix = camera_offset.transform.worldToLocalMatrix * head.transform.localToWorldMatrix;
        Matrix4x4 newOffsetMatrix = parentMatrix * targetLocalMatrix.inverse;
        camera_offset.transform.position = newOffsetMatrix.GetColumn(3);
        camera_offset.transform.rotation = Quaternion.LookRotation(newOffsetMatrix.GetColumn(2),newOffsetMatrix.GetColumn(1));
    }
}
