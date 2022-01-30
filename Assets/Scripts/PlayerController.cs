using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public enum Player
    {
        Player1 = 1,
        Player2 = 2,
    }

    [SerializeField] Player _player = Player.Player1;
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _turnSmoothTime = 0.1f;

    private Transform m_camera;
    private Rigidbody m_rigidbody;


    private float m_turnSmoothVelocity = 0f;

    private Vector3 m_move;
    private Vector3 m_direction;

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        m_camera = CameraController.MainCamera.transform;
    }

    void Update()
    {
        FixRotation();
    }

    void FixedUpdate()
    {
        OnMove();
    }

    private void FixRotation()
    {
        if (m_direction.magnitude >= 0.25f)
        {
            var targetAngle = Mathf.Atan2(m_direction.x, m_direction.z) * Mathf.Rad2Deg + m_camera.eulerAngles.y;

            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_turnSmoothVelocity, _turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            m_move = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        else
            m_move = Vector3.zero;
    }

    private void OnMove()
    {
        var velocity = m_move * _speed;

        m_rigidbody.velocity = new Vector3(velocity.x, m_rigidbody.velocity.y, velocity.z);
    }

    #region Player 1 Controller

    private void OnLeftStick(Vector2 value)
    {
        if (_player != Player.Player1)
            return;

        m_direction = new Vector3(value.x, 0f, value.y);
    }

    private void OnInteract1()
    {
        if (_player != Player.Player1)
            return;

        Debug.Log("OnInteract1");
    }

    private void OnLeftTrigger()
    {
        if (_player != Player.Player1)
            return;

        Debug.Log("OnLeftTrigger");
    }

    #endregion

    #region Player 2 Controller

    private void OnRightStick(Vector2 value)
    {
        if (_player != Player.Player2)
            return;

        m_direction = new Vector3(value.x, 0f, value.y);
    }

    private void OnInteract2()
    {
        if (_player != Player.Player2)
            return;

        Debug.Log("OnInteract2");
    }

    private void OnRightTrigger()
    {
        if (_player != Player.Player2)
            return;

        Debug.Log("OnRightTrigger");
    }

    #endregion

    void OnEnable()
    {
        Observer.Player.OnLeftStick += OnLeftStick;
        Observer.Player.OnInteract1 += OnInteract1;
        Observer.Player.OnLeftTrigger += OnLeftTrigger;

        Observer.Player.OnRightStick += OnRightStick;
        Observer.Player.OnInteract2 += OnInteract2;
        Observer.Player.OnRightTrigger += OnRightTrigger;
    }

    void OnDisable()
    {
        Observer.Player.OnLeftStick -= OnLeftStick;
        Observer.Player.OnInteract1 -= OnInteract1;
        Observer.Player.OnLeftTrigger -= OnLeftTrigger;

        Observer.Player.OnRightStick -= OnRightStick;
        Observer.Player.OnInteract2 -= OnInteract2;
        Observer.Player.OnRightTrigger -= OnRightTrigger;
    }
}
