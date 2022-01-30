using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public enum Player
    {
        Anyone = 0,
        Player1 = 1,
        Player2 = 2,
    }

    [SerializeField] private Player _player = Player.Player1;
    public Player PlayerType => _player;
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _turnSmoothTime = 0.1f;
    [SerializeField] private float _jumpForce = 1f;
    [SerializeField] private float _dashDistance = 4f;

    [SerializeField] private AudioSource _audioJump = null;
    [SerializeField] private AudioSource _audioDash = null;

    private Transform m_camera;
    private Rigidbody m_rigidbody;


    private float m_turnSmoothVelocity = 0f;

    private bool m_isDashing = false;

    private bool m_isOnGround = false;
    private bool m_canCheckGround = true;

    private bool m_canPlay = false;
    private bool m_isAlive = true;

    private Vector3 m_move;
    private Vector3 m_directionKeyboard;
    private Vector3 m_directionGamepad;

    public static bool IsGamepad { get; private set; } = false;

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        _audioJump.Stop();

        _audioDash.Stop();
    }

    void Start()
    {
        m_camera = CameraController.MainCamera.transform;
    }

    void Update()
    {
        CheckDeath();

        if (!m_canPlay)
            return;

        FixRotation();

        CheckGround();
    }

    void FixedUpdate()
    {
        if (!m_canPlay)
        {
            m_rigidbody.angularVelocity = Vector3.zero;

            m_rigidbody.velocity = new Vector3(0f, m_rigidbody.velocity.y, 0f);

            return;
        }

        OnMove();
    }

    private void FixRotation()
    {
        if (m_isDashing)
            return;

        if (m_directionGamepad.magnitude >= 0.25f)
        {
            var targetAngle = Mathf.Atan2(m_directionGamepad.x, m_directionGamepad.z) * Mathf.Rad2Deg + m_camera.eulerAngles.y;

            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_turnSmoothVelocity, _turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            m_move = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            IsGamepad = true;
        }
        else if (m_directionKeyboard.magnitude >= 0.25f)
        {
            var targetAngle = Mathf.Atan2(m_directionKeyboard.x, m_directionKeyboard.z) * Mathf.Rad2Deg + m_camera.eulerAngles.y;

            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_turnSmoothVelocity, _turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            m_move = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            IsGamepad = false;
        }
        else
            m_move = Vector3.zero;
    }

    private void CheckGround()
    {
        if (_player != Player.Player1)
            return;

        if (m_isOnGround || !m_canCheckGround)
            return;

        if (Physics.Raycast(transform.position, Vector3.down, 0.51f))
            m_isOnGround = true;
    }

    private void CheckDeath()
    {
        if (m_isAlive && transform.position.y < -10f)
        {
            m_isAlive = false;

            Observer.GameManager.OnDeath.Notify();
        }
    }

    private void OnMove()
    {
        if (m_isDashing)
            return;

        var velocity = m_move * _speed;

        m_rigidbody.angularVelocity = Vector3.zero;

        m_rigidbody.velocity = new Vector3(velocity.x, m_rigidbody.velocity.y, velocity.z);
    }

    private void OnJump(float value)
    {
        if (m_isDashing)
            return;

        _audioJump.Play();

        m_rigidbody.AddForce(Vector3.up * value, ForceMode.Impulse);
        m_isOnGround = false;
        m_canCheckGround = false;

        StartCoroutine(CanCheckGround());
    }

    private IEnumerator CanCheckGround()
    {
        yield return new WaitForSeconds(0.25f);

        m_canCheckGround = true;
    }

    private void OnDash(float distance)
    {
        if (m_isDashing)
            return;

        _audioDash.Play();

        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);

        StartCoroutine(OnDash(0.25f, distance));
    }

    private IEnumerator OnDash(float duration, float distance)
    {
        m_isDashing = true;

        float currentTime = 0f;

        Vector3 startPosition = transform.position;

        Vector3 endPosition = transform.position + transform.forward * distance;

        while (currentTime < duration)
        {
            if (!m_isDashing)
                yield break;

            currentTime += Time.deltaTime;

            transform.position = Vector3.Lerp(startPosition, endPosition, currentTime / duration);

            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);

            yield return null;
        }

        transform.position = endPosition;

        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);

        yield return null;

        m_isDashing = false;
    }

    #region Player 1 Controller

    private void OnPlayer1Move(Vector2 value)
    {
        if (_player != Player.Player1)
            return;

        m_directionKeyboard = new Vector3(value.x, 0f, value.y);
    }

    private void OnLeftStick(Vector2 value)
    {
        if (_player != Player.Player1)
            return;

        m_directionGamepad = new Vector3(value.x, 0f, value.y);
    }

    private void OnInteract1()
    {
        if (_player != Player.Player1)
            return;

        var colliders = Physics.OverlapBox(transform.position, Vector3.one * 0.5f, Quaternion.identity, 1 << LayerMask.NameToLayer("WallTrigger"));
        if (colliders != null && colliders.Length > 0)
            foreach (var c in colliders)
                if (c.TryGetComponent<WallTrigger>(out WallTrigger trigger))
                {
                    trigger.TurnOn(_player);

                    break;
                }
    }

    private void OnLeftTrigger()
    {
        if (_player != Player.Player1 || !m_isOnGround)
            return;

        if (!m_canPlay)
            return;

        OnJump(_jumpForce);
    }

    #endregion

    #region Player 2 Controller

    private void OnPlayer2Move(Vector2 value)
    {
        if (_player != Player.Player2)
            return;

        m_directionKeyboard = new Vector3(value.x, 0f, value.y);
    }

    private void OnRightStick(Vector2 value)
    {
        if (_player != Player.Player2)
            return;

        m_directionGamepad = new Vector3(value.x, 0f, value.y);
    }

    private void OnInteract2()
    {
        if (_player != Player.Player2)
            return;

        var colliders = Physics.OverlapBox(transform.position, Vector3.one * 0.5f, Quaternion.identity, 1 << LayerMask.NameToLayer("WallTrigger"));
        if (colliders != null && colliders.Length > 0)
            foreach (var c in colliders)
                if (c.TryGetComponent<WallTrigger>(out WallTrigger trigger))
                {
                    trigger.TurnOn(_player);

                    break;
                }
    }

    private void OnRightTrigger()
    {
        if (_player != Player.Player2)
            return;
        
        if (!m_canPlay)
            return;

        OnDash(_dashDistance);
    }

    #endregion

    private void OnCollisionEnter(Collision other)
    {
        if (_player == Player.Player2)
            OnCollisionPlayer2(other);
    }

    private void OnCollisionStay(Collision other)
    {
        if (_player == Player.Player2)
            OnCollisionPlayer2(other);
    }

    private void OnCollisionPlayer2(Collision other)
    {
        if (other.collider.CompareTag("Ground") || other.collider.CompareTag("Elevator"))
            return;

        if (m_isDashing)
            m_isDashing = false;
    }

    private void OnCanPlay()
    {
        m_canPlay = true;
    }

    private void OnCantPlay()
    {
        m_canPlay = false;

        m_rigidbody.angularVelocity = Vector3.zero;

        m_rigidbody.velocity = Vector3.zero;
    }

    void OnEnable()
    {
        Observer.Player.OnPlayer1Move += OnPlayer1Move;
        Observer.Player.OnLeftStick += OnLeftStick;
        Observer.Player.OnInteract1 += OnInteract1;
        Observer.Player.OnLeftTrigger += OnLeftTrigger;

        Observer.Player.OnPlayer2Move += OnPlayer2Move;
        Observer.Player.OnRightStick += OnRightStick;
        Observer.Player.OnInteract2 += OnInteract2;
        Observer.Player.OnRightTrigger += OnRightTrigger;

        Observer.Player.OnCanPlay += OnCanPlay;
        Observer.Player.OnCantPlay += OnCantPlay;
    }

    void OnDisable()
    {
        Observer.Player.OnPlayer1Move -= OnPlayer1Move;
        Observer.Player.OnLeftStick -= OnLeftStick;
        Observer.Player.OnInteract1 -= OnInteract1;
        Observer.Player.OnLeftTrigger -= OnLeftTrigger;

        Observer.Player.OnPlayer2Move -= OnPlayer2Move;
        Observer.Player.OnRightStick -= OnRightStick;
        Observer.Player.OnInteract2 -= OnInteract2;
        Observer.Player.OnRightTrigger -= OnRightTrigger;
        
        Observer.Player.OnCanPlay -= OnCanPlay;
        Observer.Player.OnCantPlay -= OnCantPlay;
    }
}
