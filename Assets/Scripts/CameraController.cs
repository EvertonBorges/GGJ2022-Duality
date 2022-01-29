using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Camera References")]
    [SerializeField] private CinemachineVirtualCamera _cam;
    [Range(1f, 20f)] [SerializeField] private float _distanceFactor = 10f;
    
    [Header("Player References")]
    [SerializeField] private Transform _midPoint;
    [SerializeField] private PlayerController _player1;
    [SerializeField] private PlayerController _player2;

    private float minFov = 40f;
    private float maxFov = 100f;

    void Update()
    {
        _midPoint.position = GetPlayersMidPoint();
        _cam.m_Lens.FieldOfView = GetFovValue();
    }

    private Vector3 GetPlayersMidPoint()
    {
        return _player1.transform.position + (_player2.transform.position - _player1.transform.position) / 2;
    }

    private float GetFovValue()
    {
        return Mathf.Lerp(minFov, maxFov, GetPlayersDistance() / _distanceFactor);
    }

    private float GetPlayersDistance()
    {
        return Vector3.Distance(_player1.transform.position, _player2.transform.position);
    }
}
