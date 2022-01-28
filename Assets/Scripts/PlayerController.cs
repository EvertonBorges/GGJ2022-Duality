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

    [SerializeField] Player player = Player.Player1;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    private Transform _camera;

    private float _turnSmoothVelocity = 0f;

    private Vector3 _move;
    private Vector3 _direction;

    void Awake()
    {
        _camera = Camera.main.transform;
    }

    void Update()
    {
        OnMove();
    }

    private void OnMove()
    {
        if (_direction.magnitude >= 0.25f)
        {
            var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            _move = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        else
            _move = Vector3.zero;

        var magnitude = _direction.magnitude;
        if (magnitude > 1f)
            magnitude = 1f;
        
        transform.position += _move * speed * Time.deltaTime;
    }

    private void OnLeftStick(Vector2 value)
    {
        if (player == Player.Player1)
            _direction = new Vector3(value.x, 0f, value.y);
    }

    private void OnRightStick(Vector2 value)
    {
        if (player == Player.Player2)
            _direction = new Vector3(value.x, 0f, value.y);
    }

    void OnEnable()
    {
        Observer.Player.OnLeftStick += OnLeftStick;
        Observer.Player.OnRightStick += OnRightStick;
    }

    void OnDisable()
    {
        Observer.Player.OnLeftStick -= OnLeftStick;
        Observer.Player.OnRightStick -= OnRightStick;
    }
}
