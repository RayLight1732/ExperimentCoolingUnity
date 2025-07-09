using experiment;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class TestSender : MonoBehaviour
{
    [SerializeField]
    private MyTcpServer tcpServer;
    [SerializeField]
    private string message = "SendFromServer";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        tcpServer.TcpServer.SendDataAll(new StringData(message));
    }
}
