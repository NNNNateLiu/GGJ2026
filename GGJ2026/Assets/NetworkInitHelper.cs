using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkInitHelper : MonoBehaviour
{
    public NetworkIdentity networkIdentity;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public Camera camera;
    public AudioListener audioListener;
    public PlayerInput playerInput;
    
    // Start is called before the first frame update
    void Start()
    {
        if (networkIdentity.isLocalPlayer)
        {
            cinemachineVirtualCamera.enabled = true;
            camera.enabled = true;
            audioListener.enabled = true;
            playerInput.enabled = true;
        }
    }
    
}
