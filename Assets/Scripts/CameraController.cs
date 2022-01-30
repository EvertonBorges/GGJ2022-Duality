using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static Camera MainCamera { get; private set; } = null;

    [Header("Camera References")]
    [SerializeField] private CinemachineVirtualCamera _cam;
    [Range(1f, 50f)] [SerializeField] private float _distanceFactor = 10f;
    [SerializeField] private Vector3 _minOffset = default;
    [SerializeField] private Vector3 _maxOffset = default;

    private CinemachineTransposer m_transposer = null;


    [Header("Player References")]
    [SerializeField] private Transform _midPoint;
    [SerializeField] private PlayerController _player1;
    [SerializeField] private PlayerController _player2;

    void Awake()
    {
        MainCamera = Camera.main;
        m_transposer = _cam.GetCinemachineComponent<CinemachineTransposer>();
    }

    void Update()
    {
        _midPoint.position = GetPlayersMidPoint();
        m_transposer.m_FollowOffset = GetOffset();
    }

    private Vector3 GetPlayersMidPoint()
    {
        return _player1.transform.position + (_player2.transform.position - _player1.transform.position) / 2;
    }

    private Vector3 GetOffset()
    {
        var distance = Vector3.Distance(_player1.transform.position, _player2.transform.position);

        var distanceProportion = distance / _distanceFactor;

        return Vector3.Lerp(_minOffset, _maxOffset, distanceProportion);
    }
}
