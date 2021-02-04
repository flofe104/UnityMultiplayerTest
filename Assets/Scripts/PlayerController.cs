using ServerData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        InputData inputData = InputData.GetCurrentInputData(transform);
        ClientSend.SendPlayerInput(inputData);
    }

}
