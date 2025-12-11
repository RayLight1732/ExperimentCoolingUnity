using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using experiment;
using System;

public abstract class SendMessageEventProvider : MonoBehaviour
{
    public event Action<string> Action;
    public void InvokeAction(string message)
    {
        Debug.Log("invoke send message:"+message);
        if (Action != null) Action.Invoke(message);
    }
}

public class MyTcpServer : StartEventProvider
{
    [SerializeField]
    private int port = 51234;
    [SerializeField]
    private SendMessageEventProvider provider;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private AudioSource notice;

    private TcpServer<DecodedData> tcpServer;
    public TcpServer<DecodedData> TcpServer { get { return tcpServer; } }


    // Start is called before the first frame update
    private void Awake()
    {
        var decoderMap = new Dictionary<string, DataDecoder<DecodedData>>()
        {
            {StringDataConstants.STRING_DATA_TYPE, new StringDataDecoder() },
        };
        var decoder = new MultiTypeDataDecoder(decoderMap);
        tcpServer = new TcpServer<DecodedData>(decoder);
    }

    private void OnEnable()
    {
        tcpServer.StartConnection(System.Net.IPAddress.Any, port);
        provider.Action += HandleSendMessageEvent;
        
    }

    private void OnDisable()
    {   
        provider.Action -= HandleSendMessageEvent;
        tcpServer.CloseConnection();
    }

    // Update is called once per frame
    void Update()
    {
        DecodedData decodedData;
        while (tcpServer != null && tcpServer.GetCount() > 0)
        {
            tcpServer.TryDequeue(out decodedData);
            if (decodedData.DataType == StringDataConstants.STRING_DATA_TYPE)
            {
                string text = decodedData.GetData<string>();
                if (text == "start")
                {
                    tcpServer.SendDataAll(new StringData("started"));
                    InvokeAction();
                } else if(text == "reset")
                { 
                    gameManager.ResetPose();
                } else if (text == "sound")
                {
                    notice.Play();
                } else if (text == "stop")
                {
                    gameManager.StopGame();
                }
            }
        }
    }

    private void HandleSendMessageEvent(string message)
    {
        tcpServer.SendDataAll(new StringData(message));
    }
}
