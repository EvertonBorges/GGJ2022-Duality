using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{

    [SerializeField] private PlayerController.Player _playerType;

    [SerializeField] private Material _player1Mat;
    [SerializeField] private Material _player2Mat;
    [SerializeField] private Material _defaultMat;

    [SerializeField] private AudioSource _audioOn;
    [SerializeField] private AudioSource _audioOff;

    [Header("Switch Infos")]
    [SerializeField] private Transform _switch;

    [Header("Door References")]
    [SerializeField] private Door _door;

    private bool m_isOn = false;
    
    public bool IsOn => m_isOn;

    private List<int> m_collisions = new List<int>();

    private Coroutine m_coroutine = null;

    void Awake()
    {
        ChangeSwitchMaterial();

        _audioOff.Stop();
        _audioOn.Stop();
    }

    private void ChangeSwitchMaterial()
    {
        var meshRenderer = _switch.GetComponent<MeshRenderer>();

        switch(_playerType)
        {
            case PlayerController.Player.Anyone: meshRenderer.material = _defaultMat; break;
            case PlayerController.Player.Player1: meshRenderer.material = _player1Mat; break;
            case PlayerController.Player.Player2: meshRenderer.material = _player2Mat; break;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerController>(out PlayerController player) || (_playerType != PlayerController.Player.Anyone && _playerType != player.PlayerType))
            return;

        m_collisions.Add(other.GetInstanceID());

        m_isOn = true;

        if (m_collisions.Count == 1)
        {
            CallSwitch();

            _door.Open();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<PlayerController>(out PlayerController player) || (_playerType != PlayerController.Player.Anyone && _playerType != player.PlayerType))
            return;

        m_collisions.Remove(other.GetInstanceID());

        if (m_collisions.Count <= 0)
        {
            m_isOn = false;

            CallSwitch();

            _door.Close();
        }
    }

    private void CallSwitch()
    {
        if (m_isOn)
            _audioOn.Play();
        else
            _audioOff.Play();

        if (m_coroutine != null)
            StopCoroutine(m_coroutine);

        m_coroutine = StartCoroutine(Switch(0.25f));
    }

    private IEnumerator Switch(float duration)
    {
        float currentTime = 0f;

        float startPoint = m_isOn ? 1f : 0.25f;

        float endPoint = m_isOn ? 0.25f : 1f;

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;

            var yPosition = Mathf.Lerp(startPoint, endPoint, currentTime / duration);

            _switch.localPosition = new Vector3(_switch.localPosition.x, yPosition, _switch.localPosition.z);

            yield return null;
        }

        _switch.localPosition = new Vector3(_switch.localPosition.x, endPoint, _switch.localPosition.z);

        yield return null;

        m_coroutine = null;
    }

}
