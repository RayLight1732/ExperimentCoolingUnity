using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPoseCaller : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private GameManager manager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        manager.ResetPose();
    }
}
