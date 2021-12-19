using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UIController : MonoBehaviour, IOnEventCallback
{   
    // If you have multiple custom events, it is recommended to define them in the used class
    public const byte MoveUnitsToTargetPositionEvent = 1;

    private GameObject car;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (car == null)
        {
            car = GameObject.FindGameObjectWithTag("car");
        }
    }
    

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == MoveUnitsToTargetPositionEvent)
        {
            car.SetActive(false);
        }
    }
}
