using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WallTrigger : MonoBehaviour
{
    [SerializeField] private PlayerController.Player _playerType;

    [SerializeField] private Material _player1Mat;
    [SerializeField] private Material _player2Mat;
    [SerializeField] private Material _defaultMat;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private Text _tutorial;

    [Header("Switch Infos")]
    [SerializeField] private Transform _switch;

    private Transform m_cameraTransform;
    private MeshRenderer m_meshRenderer = null;

    private bool m_isOn = false;
    public bool IsOn => m_isOn;

    private bool m_tutorial = false;

    private List<int> m_collisions = new List<int>();

    private Coroutine m_coroutine = null;
    private Coroutine m_coroutineTutorial = null;

    private Gamepad m_gamepad = null;

    void Awake()
    {
        ChangeSwitchMaterial();

        CallSwitch(0f);
    }

    void Start()
    {
        m_cameraTransform = CameraController.MainCamera.transform;

        ConfigureTutorial();
    }

    void Update()
    {
        FixCanvasRotation();

        UpdateTutorialText();
    }

    private void FixCanvasRotation()
    {
        if (m_tutorial)
            _canvas.transform.LookAt(m_cameraTransform, Vector3.up);
    }

    private void UpdateTutorialText()
    {
        if (m_gamepad == null && Gamepad.current != null)
        {
            m_gamepad = Gamepad.current;
            _tutorial.text = _playerType == PlayerController.Player.Player1 ? "L1" : "R1";
        }
        else if (m_gamepad != null && Gamepad.current == null)
        {
            m_gamepad = null;
            _tutorial.text = _playerType == PlayerController.Player.Player1 ? "E" : "U";
        }
    }

    private void ChangeSwitchMaterial()
    {
        m_meshRenderer = _switch.GetComponent<MeshRenderer>();

        switch(_playerType)
        {
            case PlayerController.Player.Anyone: m_meshRenderer.material = _defaultMat; break;
            case PlayerController.Player.Player1: m_meshRenderer.material = _player1Mat; break;
            case PlayerController.Player.Player2: m_meshRenderer.material = _player2Mat; break;
        }
    }

    private void ConfigureTutorial()
    {
        CallTutorial(0f);
    }

    public void ForceToTurnOff()
    {
        m_isOn = false;
        CallSwitch(0f);
    }

    public void TurnOn(PlayerController.Player playerType)
    {
        if (_playerType != playerType)
            return;

        m_isOn = true;
        CallSwitch();
        Observer.GameManager.TurnOnOff.Notify(playerType);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerController>(out PlayerController player) || (_playerType != PlayerController.Player.Anyone && _playerType != player.PlayerType))
            return;

        if (m_isOn)
            return;

        m_collisions.Add(other.GetInstanceID());

        m_tutorial = true;

        if (m_collisions.Count == 1)
            CallTutorial();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<PlayerController>(out PlayerController player) || (_playerType != PlayerController.Player.Anyone && _playerType != player.PlayerType))
            return;

        if (m_isOn && !m_tutorial)
            return;

        m_collisions.Remove(other.GetInstanceID());

        if (m_collisions.Count <= 0)
        {
            m_tutorial = false;

            CallTutorial();
        }
    }

    private void CallSwitch(float duration = 0.125f)
    {
        if (m_coroutine != null)
            StopCoroutine(m_coroutine);

        m_coroutine = StartCoroutine(Switch(duration));
    }

    private IEnumerator Switch(float duration)
    {
        float currentTime = 0f;

        float startPoint = m_isOn ? 1f : 0.25f;

        float endPoint = m_isOn ? 0.25f : 1f;

        Color startColor = m_isOn ? (_playerType == PlayerController.Player.Player1 ? _player1Mat.color : _player2Mat.color) : Color.green;

        Color endColor = m_isOn ? Color.green : (_playerType == PlayerController.Player.Player1 ? _player1Mat.color : _player2Mat.color);

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;

            var yPosition = Mathf.Lerp(startPoint, endPoint, currentTime / duration);

            var color = Color.Lerp(startColor, endColor, currentTime / duration);

            _switch.localPosition = new Vector3(_switch.localPosition.x, yPosition, _switch.localPosition.z);

            m_meshRenderer.material.color = color;

            yield return null;
        }

        _switch.localPosition = new Vector3(_switch.localPosition.x, endPoint, _switch.localPosition.z);

        m_meshRenderer.material.color = endColor;

        yield return null;

        m_coroutine = null;
    }

    private void CallTutorial(float duration = 0.25f)
    {
        if (m_coroutineTutorial != null)
            StopCoroutine(m_coroutineTutorial);

        m_coroutineTutorial = StartCoroutine(Tutorial(duration));
    }

    private IEnumerator Tutorial(float duration)
    {
        float currentTime = 0f;

        float startScale = m_tutorial ? 0f : 1f;

        float endScale = m_tutorial ? 1f : 0f;

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;

            var scale = Mathf.Lerp(startScale, endScale, currentTime / duration);

            _canvas.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        _canvas.transform.localScale = Vector3.one * endScale;

        yield return null;

        m_coroutineTutorial = null;
    }

}
