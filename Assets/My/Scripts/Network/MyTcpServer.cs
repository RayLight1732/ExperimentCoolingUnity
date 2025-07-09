using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using experiment;
using System;

public abstract class EndGameEventProvider : MonoBehaviour
{
    public event Action Action;
    public void InvokeAction()
    {
        if (Action != null) Action.Invoke();
    }
}

public class MyTcpServer : StartEventProvider
{
    [SerializeField]
    private int port = 51234;
    [SerializeField]
    private EndGameEventProvider provider;

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
        provider.Action += OnEndGame;
        
    }

    private void OnDisable()
    {   
        provider.Action -= OnEndGame;
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
                }
            }
        }
    }

    private void OnEndGame()
    {
        tcpServer.SendDataAll(new StringData("end"));
    }
}
