using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private CinemachineMixingCamera mixingCamera;
    void Awake()
    {
        mixingCamera.ChildCameras[0].enabled = false;
        mixingCamera.ChildCameras[2].enabled = false;
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        mixingCamera = GetComponent<CinemachineMixingCamera>();
        
    }

    void OnEnable()
    {
        if (_player != null)
        {
            _player.DanceEvent += DanceEvent;
            _player.AimEvent += AimEvent;
        }
    }

    void OnDisable()
    {
        if (_player != null)
        {
            _player.DanceEvent -= DanceEvent;
            _player.AimEvent -= AimEvent;
        }
    }


    public void DanceEvent(bool dancing)
    {
        if (mixingCamera == null || mixingCamera.ChildCameras == null || mixingCamera.ChildCameras.Count < 3)
            mixingCamera = GetComponent<CinemachineMixingCamera>();
        if (mixingCamera == null || mixingCamera.ChildCameras.Count < 3) return;

        if (dancing)
        {
            mixingCamera.ChildCameras[0].enabled = true;
            mixingCamera.ChildCameras[1].enabled = false;
            mixingCamera.ChildCameras[2].enabled = false;
        }
        else
        {
            mixingCamera.ChildCameras[1].enabled = true;
            mixingCamera.ChildCameras[0].enabled = false;
            mixingCamera.ChildCameras[2].enabled = false;
        }
    }

    public void AimEvent(bool aiming)
    {
        if (aiming)
        {
            Debug.Log("Camera: Player started aiming");
            mixingCamera.ChildCameras[2].enabled = true; // Aim View Camera
            mixingCamera.ChildCameras[1].enabled = false;
            mixingCamera.ChildCameras[0].enabled = false;
        }
        else
        {
            Debug.Log("Camera: Player stopped aiming");
            mixingCamera.ChildCameras[1].enabled = true; // 3 person View Camera
            mixingCamera.ChildCameras[0].enabled = false;
            mixingCamera.ChildCameras[2].enabled = false;
        }
    }
}
